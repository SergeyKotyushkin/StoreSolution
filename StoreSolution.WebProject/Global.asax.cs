using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Security;
using System.Web.SessionState;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Realizations;
using StoreSolution.MyIoC;

namespace StoreSolution.WebProject
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            #region IoC
            SimpleContainer.Register<IPersonRepository>(typeof(EfPersonRepository));
            SimpleContainer.Register<IProductRepository>(typeof(EfProductRepository));
            #endregion

        }

        private void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            var httpException = ex as HttpException ?? ex.InnerException as HttpException;
            if (httpException == null) return;

            if (httpException.WebEventCode == WebEventCodes.RuntimeErrorPostTooLarge)
            {
                //handle the error
                Response.Write("Too big a file."); //for example
                
            }
        }
    }
}