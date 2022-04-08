using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoinMarketCap.Dal;
using CoinMarketCap.WebApi.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CoinMarketCap.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly AuthenticationSettings _atuhenticationSettings;
        private readonly ICoinMarketCapContext _context;

        public AuthController(
            IOptions<AuthenticationSettings> settings,
            ICoinMarketCapContext context)
        {
            _atuhenticationSettings = settings.Value;
            _context = context;
        }

        [HttpPost]
        public async Task Login([FromBody]User user)
        {
            if (IsBlocked(user.login))
            {
                await WriteError("You entered wrong key too many times. Try again tomorrow or contact administrator.");
                return;
            }

            var identity = GetIdentity(user.login.ToLower(), user.key);
            if (identity == null)
            {
                await WriteError("Invalid login or key.");
                return;
            }

            if(_context.Apps.FirstOrDefault(x => x.Login == user.login.ToLower()) == null)
            {
                AddLogEntry(LogEntryTypes.LoginNotAllowedUser, login: user.login, saveChanges: true);
                await WriteError("You are not allowed. Please, contact administrator if you need to be.");
                return;
            }

            AddLogEntry(LogEntryTypes.Login, login: user.login, saveChanges: true);

            

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                AuthenticationOptions.Issuer,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(AuthenticationOptions.Lifetime),
                signingCredentials: new SigningCredentials(AuthenticationOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            Response.ContentType = "application/json";
            await Response.WriteAsync(JsonConvert.SerializeObject(new { access_token = encodedJwt },
                new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        [HttpGet]
        [Authorize]
        public IActionResult CurrentUser()
        {
            return Ok(User.Identity.Name);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            AddLogEntry(LogEntryTypes.Logout, saveChanges: true);
            return Ok();
        }

        private void AddLogEntry(string type, object value = null, bool saveChanges = false, string login = null)
        {
            _context.LogEntries.Add(new LogEntry
            {
                Date = DateTime.Now,
                Login = login ?? User.Identity?.Name,
                Type = type,
                Value = value == null ? null : JsonConvert.SerializeObject(value),
                UserIp = $"{HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}"
            });

            if (saveChanges)
            {
                _context.SaveChanges();
            }
        }

        private async Task WriteError(string error)
        {
            Response.StatusCode = 400;
            await Response.WriteAsync(JsonConvert.ToString(error));
        }

        private bool IsBlocked(string login)
        {
            var logForLastDay = _context.LogEntries.Where(e => e.Login == login
                                                              && e.Date > DateTime.Now.AddDays(-1)).ToList();

            if (logForLastDay.Any(e => e.Type == LogEntryTypes.LoginBlocked))
            {
                return true;
            }

            if (logForLastDay.Count(e => e.Type == LogEntryTypes.LoginWrongPassword) >
                _atuhenticationSettings.WrongPasswordMaxCount)
            {
                AddLogEntry(LogEntryTypes.LoginBlocked, login: login, saveChanges: true);
                return true;
            }

            return false;
        }

        private ClaimsIdentity GetIdentity(string login, string key)
        {
            var appRecord = _context.Apps.FirstOrDefault(x => x.Login == login);

            var result = new VerifyResponse()
            {
                IsExist = appRecord != null,
                IsLoginExist = appRecord != null && appRecord.Login == login,
                KeyValid = appRecord != null && appRecord.Key == key
            };

            if (!result.IsLoginExist)
            {
                AddLogEntry(LogEntryTypes.LoginWrongEmail, new { password = key }, true, login);
                return null;
            }

            if (!result.IsExist)
            {
                AddLogEntry(LogEntryTypes.LoginNotAllowedUser, new { password = key }, true, login);
                return null;
            }

            if (!result.KeyValid)
            {
                AddLogEntry(LogEntryTypes.LoginWrongPassword, new { password = key }, true, login);
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, login),
                new Claim(ClaimTypes.Email, login),
                new Claim(ClaimTypes.Sid, key),
            };

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
