using StoreSolution.BusinessLogic.Currency;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Realizations;
using StoreSolution.BusinessLogic.Lang;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Mail;
using StoreSolution.BusinessLogic.Mail.Contracts;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StoreSolution.BusinessLogic.StructureMap
{
    public class AllRegistry : Registry
    {
        public AllRegistry()
        {
            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
            });

            // Currency
            For<ICurrencyConverter>().Use<CurrencyConverter>().Singleton();
            For<ICurrencyService>().Use<CurrencyService>().Singleton();
            For<IRateService>().Use<YahooRateService>().Singleton();

            // Mail
            For<IMailSender>().Use<MailSender>().AlwaysUnique();
            For<IMailService>().Use<MailService>().Singleton();

            // Repository
            For<IEfPersonRepository>().Use<EfPersonRepository>().Singleton();
            For<IEfProductRepository>().Use<EfProductRepository>().Singleton();
            For<IEfOrderHistoryRepository>().Use<EfOrderHistoryRepository>().Singleton();

            // Other
            For<ILangSetter>().Use<LangSetter>().Singleton();
        }
    }
}