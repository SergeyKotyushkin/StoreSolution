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
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });

            For<ICurrencyConverter>().Use<CurrencyConverter>().Singleton();
            For<ICurrencyService>().Use<CurrencyService>().Singleton();
            For<IRateService>().Use<YahooRateService>().Singleton();
        }
    }
}