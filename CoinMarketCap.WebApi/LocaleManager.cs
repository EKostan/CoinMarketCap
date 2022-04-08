using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Options;

namespace CoinMarketCap.WebApi
{
    public class LocaleManager
    {
        private readonly Settings _settings;
        private static Dictionary<string, string> LocaleCulture = new Dictionary<string, string>();

        public LocaleManager(IOptions<Settings> settings)
        {
            _settings = settings.Value;

            foreach (var item in _settings.LocaleList)
            {
                LocaleCulture[item.Locale] = item.Culture;
            }
        }

        public void SetCulture(string locale)
        {
            var culture = string.IsNullOrEmpty(locale) || !LocaleCulture.ContainsKey(locale.ToLower())
                ? LocaleCulture["en"]
                : LocaleCulture[locale.ToLower()];
            var cultureInfo = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
    }
}