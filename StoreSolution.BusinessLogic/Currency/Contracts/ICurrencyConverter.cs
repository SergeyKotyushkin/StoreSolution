using System;
using System.Globalization;

namespace StoreSolution.BusinessLogic.Currency.Contracts
{
    public interface ICurrencyConverter
    {
        decimal Convert(CultureInfo cultureFrom, CultureInfo cultureTo, decimal value, DateTime dateTimeNow);

        decimal ConvertByRate(decimal value, decimal rate);

        decimal ConvertFromRubles(CultureInfo cultureTo, decimal value, DateTime dateTimeNow);

        decimal ConvertToRubles(CultureInfo cultureFrom, decimal value, DateTime dateTimeNow);

        decimal GetRate(CultureInfo cultureFrom, CultureInfo cultureTo, DateTime dateTimeNow);
    }
}
