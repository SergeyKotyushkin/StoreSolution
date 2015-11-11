using System;
using System.Drawing;
using System.Web;
using System.Web.Security;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject
{
    public partial class Index : System.Web.UI.Page
    {
        private StoreMaster _master;

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (null == cookie) return;
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null)
                throw new HttpUnhandledException("Wrong master page.");

            _master.BtnBackVisibility = false;
            _master.BtnSignOutVisibility = false;
            if (Session["NewUser"] != null)
            {
                _master.LabMessageText = (string)HttpContext.GetGlobalResourceObject("Lang", "Index_SuccessfullyUserCreation");
                Logger.Log.Info("Successfully user creation.");
                _master.LabMessageForeColor = Color.ForestGreen;
                Session["NewUser"] = null;
            }
            else _master.LabMessageForeColor = Color.Red;

            //if (Request.QueryString["ReturnUrl"] != null)
            //    Response.Redirect(Request.Url.GetLeftPart(UriPartial.Path));

            var user = Membership.GetUser();
            if (user == null) return;

            if (Roles.IsUserInRole("Admin"))            {

                Logger.Log.Debug("Admin " + user.UserName + " redirected to ProductManagement page.");
                Response.Redirect("~/Admin/ProductManagement.aspx");
            }
            else if (Roles.IsUserInRole("User"))
            {
                Logger.Log.Debug("User " + user.UserName + " redirected to ProductCatalog page.");
                Response.Redirect("~/User/ProductCatalog.aspx");
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();

            Page.Validate();
            if (Page.IsValid && Membership.ValidateUser(tbLogin.Text, tbPassword.Text))
            {
                Logger.Log.Info("User " + tbLogin.Text + " logged in.");
                FormsAuthentication.RedirectFromLoginPage(tbLogin.Text, false);
            }
            else
            {
                Logger.Log.Error("User " + tbLogin.Text + " didn't log in.");
                _master.LabMessageText = (string)HttpContext.GetGlobalResourceObject("Lang", "Index_ValidateError");
            }
        }
    }
}