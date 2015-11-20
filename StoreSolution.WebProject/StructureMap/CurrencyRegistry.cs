using StoreSolution.WebProject.Currency;
using StoreSolution.WebProject.Currency.Contracts;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StoreSolution.WebProject.StructureMap
{
    public class CurrencyRegistry : Registry
    {
        public CurrencyRegistry()
        {
            this.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });

            this.For<ICurrencyConverterBetter>().Use<CurrencyConverterBetter>().Singleton();
            this.For<ICurrencyService>().Use<CurrencyService>().Singleton();
            this.For<IRateService>().Use<YahooRateService>().Singleton();
        }
    }
}