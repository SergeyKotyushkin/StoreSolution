using System;

namespace StoreSolution.WebProject.Currency
{
    public class Rate
    {
        public string CultureName;
        public string CurrencyName;
        public decimal? CurrencyRate;
        public DateTime? LastUpdateTime;
    }
}