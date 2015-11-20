using System;
using System.Globalization;
using StoreSolution.WebProject.Currency.Contracts;

namespace StoreSolution.WebProject.Currency
{
    public class CurrencyConverterBetter : ICurrencyConverterBetter
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyConverterBetter(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public decimal Convert(CultureInfo cultureFrom, CultureInfo cultureTo, decimal value, DateTime dateTimeNow)
        {
            var rate = GetRate(cultureFrom, cultureTo, dateTimeNow);

            return ConvertByRate(value, rate);
        }

        public decimal ConvertByRate(decimal value, decimal rate)
        {
            return decimal.Round(value * rate, 2);
        }

        public decimal ConvertFromRubles(CultureInfo cultureTo, decimal value, DateTime dateTimeNow)
        {
            return Convert(new CultureInfo("ru-Ru"), cultureTo, value, dateTimeNow);
        }

        public decimal ConvertToRubles(CultureInfo cultureFrom, decimal value, DateTime dateTimeNow)
        {
            return Convert(cultureFrom, new CultureInfo("ru-Ru"), value, dateTimeNow);
        }

        public decimal GetRate(CultureInfo cultureFrom, CultureInfo cultureTo, DateTime dateTimeNow)
        {
            return !_currencyService.CheckIsRateActual(cultureFrom, cultureTo, dateTimeNow)
                ? _currencyService.GetRealTimeRate(cultureFrom, cultureTo, dateTimeNow)
                : _currencyService.GetRate(cultureFrom, cultureTo);
        }
    }
}