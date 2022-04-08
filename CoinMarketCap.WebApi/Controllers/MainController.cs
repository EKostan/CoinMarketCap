using System;
using System.Linq;
using CoinMarketCap.Dal;
using CoinMarketCap.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoinMarketCap.WebApi.Controllers
{
    [Route("")]
    [ApiController]
    [Authorize]
    public class MainController : ControllerBase
    {
        private readonly Settings _settings;
        private readonly CoinMarketCapManager _coinMarketCapManager;
        private readonly ILogger<MainController> _logger;

        public MainController(
            CoinMarketCapManager coinMarketCapManager,
            IOptions<Settings> settings,
            ILogger<MainController> logger)
        {
            _settings = settings.Value;
            _coinMarketCapManager = coinMarketCapManager;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("alive")]
        public IActionResult Alive()
        {
            return Ok("Im alive");
        }

        [HttpGet("quotes")]
        public IActionResult Quotes(string symbols, string currency, long timestamp)
        {
            try
            {
                if (string.IsNullOrEmpty(symbols) || string.IsNullOrEmpty(currency))
                {
                    return BadRequest(ErrorOutput.CreateError("Symbols or currency not defined"));
                }

                var quote = _coinMarketCapManager.GetQuotes(symbols, currency, timestamp);
                return Ok(quote);
            }
            catch (Exception e)
            {
                _logger.LogError($"{ControllerContext.ActionDescriptor.ActionName} error: {e}");
                return BadRequest(ErrorOutput.CreateError(e.Message));
            }
        }

        [HttpGet("quotesByPairs")]
        public IActionResult QuotesByPairs(string pairs, long timestamp)
        {
            try
            {
                if (string.IsNullOrEmpty(pairs))
                {
                    return BadRequest(ErrorOutput.CreateError("Pairs or currency not defined"));
                }

                var quote = _coinMarketCapManager.GetQuotes(pairs.Split(",", StringSplitOptions.RemoveEmptyEntries), timestamp);
                return Ok(quote);
            }
            catch (Exception e)
            {
                _logger.LogError($"{ControllerContext.ActionDescriptor.ActionName} error: {e}");
                return BadRequest(ErrorOutput.CreateError(e.Message));
            }
        }

        
    }
}

