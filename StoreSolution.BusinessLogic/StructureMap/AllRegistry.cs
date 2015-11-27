using System.Web;
using System.Web.SessionState;
using StoreSolution.BusinessLogic.Currency;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Realizations;
using StoreSolution.BusinessLogic.GridViewManager;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Mail;
using StoreSolution.BusinessLogic.Mail.Contracts;
using StoreSolution.BusinessLogic.OrderRepository;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;
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

            // GridViews
            For<IGridViewProductCatalogManager<HttpSessionState>>().Use<GridViewProductCatalogAgent>();
            For<IGridViewBasketManager<HttpSessionState>>().Use<GridViewBasketAgent>();
            For<IGridViewProductManagementManager<HttpSessionState>>().Use<GridViewProductManagementAgent>();

            // Other
            For<ILangSetter>().Use<LangSetter>().Singleton();
            For<ICurrencyCultureService<HttpCookieCollection>>().Use<CurrencyCultureCookieService>();
            For<IStorageService<HttpSessionState>>().Use<StorageSessionService>();
            For<IOrderRepository<HttpSessionState>>().Use<OrderSessionRepository>();
        }
    }
}