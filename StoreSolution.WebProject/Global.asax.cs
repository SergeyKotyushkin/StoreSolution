using System;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Realizations;
using StoreSolution.MyIoC;
using StoreSolution.WebProject.Log4net;

namespace StoreSolution.WebProject
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            #region Log4Net
            Logger.Log.Info("Log4Net is ready.");
            #endregion

            #region IoC
            SimpleContainer.Register<IPersonRepository>(typeof(EfPersonRepository));
            SimpleContainer.Register<IProductRepository>(typeof(EfProductRepository));
            SimpleContainer.Register<IOrderHistoryRepository>(typeof(EfOrderHistoryRepository));
            Logger.Log.Info("IoC is ready.");
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