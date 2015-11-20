using System;
using System.Globalization;

namespace StoreSolution.WebProject.Currency
{
    public class RateScheme
    {
        public CultureInfo From;
        public CultureInfo To;
        public decimal RateDirect;
        public decimal RateInverse;
        public DateTime Updated;
    }
}