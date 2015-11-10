using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace StoreSolution.WebProject.Currency
{
    public static class CurrencyConverter
    {
        private const int Rub = 0;

        private static readonly List<Rate> Rates = new List<Rate>
        {
            new Rate {CultureName = "ru-RU", CurrencyName = "RUB", CurrencyRate = 1},
            new Rate {CultureName = "en-US", CurrencyName = "USD"},
            new Rate {CultureName = "en-GB", CurrencyName = "GBP"}
        };

        private static decimal? RefreshRate(string currencyFrom, string currencyTo)
        {
            var currentRateFrom = Rates.Where(r => r.CultureName == currencyFrom).Select(r => r).FirstOrDefault();
            var currentRateTo = Rates.Where(r => r.CultureName == currencyTo).Select(r => r).FirstOrDefault();
            if (currentRateFrom == null || currentRateTo == null) return null;
            if (currentRateFrom.CurrencyRate != null && currentRateFrom.LastUpdateTime != null)
                if (currentRateFrom.LastUpdateTime.Value.AddMinutes(30) > DateTime.Now)
                    return currentRateFrom.CurrencyRate;

            var url = string.Format(@"http://download.finance.yahoo.com/d/quotes.csv?s={0}{1}=X&f=sl1&e=.csv",
                currentRateFrom.CurrencyName, currentRateTo.CurrencyName);
            string rate;
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(url);
                rate = System.Text.Encoding.Default.GetString(data);
                rate = rate.Substring(0, rate.Length - 1).Substring(rate.IndexOf(',') + 1);
            }

            decimal result;
            if (!decimal.TryParse(rate, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result)) return null;
            
            currentRateFrom.LastUpdateTime = DateTime.Now;
            currentRateFrom.CurrencyRate = result;
            return currentRateFrom.CurrencyRate;
        }

        public static decimal ConvertFromRu(decimal value, CultureInfo ci)
        {
            if (ci.Name == Rates[Rub].CultureName) return value;
            var result = RefreshRate(ci.Name, Rates[Rub].CultureName);
            return result != null ? decimal.Round(value / result.Value, 2) : decimal.Zero;
        }

        public static decimal ConvertFromRu(decimal value, decimal? rate)
        {
            return rate != null ? decimal.Round(value / rate.Value, 2) : decimal.Zero;
        }

        public static decimal ConvertToRu(decimal value, CultureInfo ci)
        {
            if (ci.Name == Rates[Rub].CultureName) return value;
            var result = RefreshRate(ci.Name, Rates[Rub].CultureName);
            return result != null ? decimal.Round(value*result.Value, 2) : decimal.Zero;
        }

        public static decimal GetRate(CultureInfo ci)
        {
            return ConvertToRu(1, ci);
        }
    }
}