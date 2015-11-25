using System;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.StructureMap;

namespace StoreSolution.WebProject
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            #region Log4Net
            Logger.Init();
            Logger.Log.Info("Log4Net is ready.");
            #endregion

            #region StructureMap DI\IoC
            StructureMapFactory.Init();
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