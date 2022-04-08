using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CoinMarketCap.WebApi.Auth
{
    public class AuthenticationOptions
    {
        public const string Issuer = "CrowdFilming.WebApi.Administration";
        public static TimeSpan Lifetime = TimeSpan.FromDays(1);

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes("There is no way but the Administration way."));
        }
    }
}