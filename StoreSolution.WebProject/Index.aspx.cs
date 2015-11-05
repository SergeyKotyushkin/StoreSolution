using System;
using System.Drawing;
using System.Web.Security;
using StoreSolution.WebProject.Log4net;

namespace StoreSolution.WebProject
{
    public partial class Index : System.Web.UI.Page
    {
        private const string ValidateError = "Credentials are wrong!";
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["NewUser"] != null)
            {
                labMessage.Text = "Successfully user creation.";
                Logger.Log.Info("Successfully user creation.");
                labMessage.ForeColor = Color.ForestGreen;
                Session["NewUser"] = null;
            }
            else labMessage.ForeColor = Color.Red;

            var user = Membership.GetUser();
            if (user == null) return;

            if (Roles.IsUserInRole("Admin"))
            {
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

            labMessage.Text = "";
            Page.Validate();
            if (Page.IsValid && Membership.ValidateUser(tbLogin.Text, tbPassword.Text))
            {
                Logger.Log.Info("User " + tbLogin.Text + " logged in.");
                FormsAuthentication.RedirectFromLoginPage(tbLogin.Text, false);
            }
            else
            {
                Logger.Log.Error("User " + tbLogin.Text + " didn't log in.");
                labMessage.Text = ValidateError;
            }
        }
    }
}