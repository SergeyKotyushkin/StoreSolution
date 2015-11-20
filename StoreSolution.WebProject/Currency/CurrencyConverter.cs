using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using StoreSolution.WebProject.Currency.Contracts;

namespace StoreSolution.WebProject.Currency
{
    

    public class CurrencyConverter : ICurrencyConverter
    {
        private const int Rub = 0;

        private static readonly List<Rate> Rates = new List<Rate>
        {
            new Rate {CultureName = "ru-RU", CurrencyRate = 1},
            new Rate {CultureName = "en-US"},
            new Rate {CultureName = "en-GB"}
        };

        private static decimal? RefreshRate(string currencyFrom, string currencyTo, DateTime dateTimeNow)
        {
            var currentRateFrom = Rates.Where(r => r.CultureName == currencyFrom).Select(r => r).FirstOrDefault();
            var currentRateTo = Rates.Where(r => r.CultureName == currencyTo).Select(r => r).FirstOrDefault();
            if (currentRateFrom == null || currentRateTo == null) return null;
            if (currentRateFrom.CurrencyRate != null && currentRateFrom.LastUpdateTime != null)
                if (currentRateFrom.LastUpdateTime.Value.AddMinutes(30) > dateTimeNow)
                    return currentRateFrom.CurrencyRate;

            var currentFrom = new RegionInfo(new CultureInfo(currentRateFrom.CultureName).LCID).ISOCurrencySymbol;
            var currentTo = new RegionInfo(new CultureInfo(currentRateTo.CultureName).LCID).ISOCurrencySymbol;
            var rate = GetRealTimeRate(currentFrom, currentTo);

            decimal result;
            if (!decimal.TryParse(rate, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result)) return null;
            
            currentRateFrom.LastUpdateTime = DateTime.Now;
            currentRateFrom.CurrencyRate = result;
            return currentRateFrom.CurrencyRate;
        }

        private static string GetRealTimeRate(string currencyFromName, string currencyToName)
        {
            var url = string.Format(@"http://download.finance.yahoo.com/d/quotes.csv?s={0}{1}=X&f=sl1&e=.csv",
                currencyFromName, currencyToName);
            string rate;
            using (var wc = new WebClient())
            {
                var data = wc.DownloadData(url);
                rate = System.Text.Encoding.Default.GetString(data);
                rate = rate.Substring(0, rate.Length - 1).Substring(rate.IndexOf(',') + 1);
            }
            return rate;
        }

        public decimal ConvertFromRu(decimal value, string cultureName)
        {
            if (cultureName == Rates[Rub].CultureName) return value;
            var result = RefreshRate(cultureName, Rates[Rub].CultureName, DateTime.Now);
            return result != null ? decimal.Round(value / result.Value, 2) : decimal.Zero;
        }

        public decimal ConvertFromRu(decimal value, decimal? rate)
        {
            return rate != null ? decimal.Round(value / rate.Value, 2) : decimal.Zero;
        }

        public decimal ConvertToRu(decimal value, string cultureName)
        {
            if (cultureName == Rates[Rub].CultureName) return value;
            var result = RefreshRate(cultureName, Rates[Rub].CultureName, DateTime.Now);
            return result != null ? decimal.Round(value*result.Value, 2) : decimal.Zero;
        }

        public decimal GetRate(string cultureName)
        {
            return ConvertToRu(1, cultureName);
        }

        public string GetCultureNameForCurrency(string currency)
        {
            return
                Rates.Where(r => new RegionInfo(new CultureInfo(r.CultureName).LCID).ISOCurrencySymbol == currency)
                    .Select(r => r.CultureName)
                    .FirstOrDefault() ?? "en-US";
        }

    }
}