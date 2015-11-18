using System;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Realizations;
using StoreSolution.WebProject.Currency;
using StoreSolution.WebProject.Currency.Contracts;
using StoreSolution.WebProject.Log4net;
using StructureMap;

namespace StoreSolution.WebProject
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            #region Log4Net
            Logger.Log.Info("Log4Net is ready.");
            #endregion

            #region StructureMap DI\IoC
            
            ObjectFactory.Initialize(x =>
            {
                x.For<IPersonRepository>().Use<EfPersonRepository>();
                x.For<IProductRepository>().Use<EfProductRepository>();
                x.For<IOrderHistoryRepository>().Use<EfOrderHistoryRepository>();
                x.For<ICurrencyConverter>().Use<CurrencyConverter>();
            });
            Logger.Log.Info("StructureMap DI\\IoC is ready.");
            #endregion

            Logger.Log.Info("Application successfully started.");
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Logger.Log.Error("Application ended.");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            Logger.Log.Error("Application error: " + error.Message);
        }
    }
}