using System;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject
{
    public partial class Index : Page
    {
        private StoreMaster _master;

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Set(cookie);
            }
            Page.Culture = Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            _master.HiddenMoney();
            _master.BtnBackVisibility = false;
            _master.BtnSignOutVisibility = false;

            if (Session["NewUser"] != null)
            {
                _master.LabMessageForeColor = Color.ForestGreen;
                _master.LabMessageText = LangSetter.Set("Index_SuccessfullyUserCreation");
                Logger.Log.Info("Successfully user creation.");
                Session["NewUser"] = null;
            }

            var user = Membership.GetUser();
            if (user == null) return;

            RedirectFromIndexByRole(user.UserName);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid && Membership.ValidateUser(tbLogin.Text, tbPassword.Text))
            {
                Logger.Log.Info(string.Format("User {0} logged in.", tbLogin.Text));
                FormsAuthentication.SetAuthCookie(tbLogin.Text, false);
                RedirectFromIndexByRole(tbLogin.Text);
            }
            else
            {
                Logger.Log.Error(string.Format("User {0} didn't log in.", tbLogin.Text));
                _master.LabMessageText = LangSetter.Set("Index_ValidateError");
            }
        }


        private void RedirectFromIndexByRole(string userName)
        {
            if (Roles.IsUserInRole(userName, "Admin"))
            {
                Logger.Log.Debug(string.Format("Admin {0} redirected to ProductManagement page.", userName));
                Response.Redirect("~/Admin/ProductManagement.aspx");
            }
            else if (Roles.IsUserInRole(userName, "User"))
            {
                Logger.Log.Debug(string.Format("User {0} redirected to ProductCatalog page.", userName));
                Response.Redirect("~/User/ProductCatalog.aspx");
            }
        }
    }
}