using System;
using System.Drawing;
using System.Web.Security;

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
                labMessage.ForeColor = Color.ForestGreen;
                Session["NewUser"] = null;
            }
            else labMessage.ForeColor = Color.Red;

            //if (User.Identity.Name == "" && Membership.GetNumberOfUsersOnline() > 0)
            //{
            //    FormsAuthentication.SignOut();
            //    Response.Redirect("~/NotAuthenticated/NewUser.aspx");
            //}

            if (Membership.GetUser(tbLogin.Text) != null) return;

            if (Roles.IsUserInRole("Admin")) Response.Redirect("~/Admin/ProductManagement.aspx");
            else if (Roles.IsUserInRole("User")) Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();

            labMessage.Text = "";
            Page.Validate();
            if (Page.IsValid && Membership.ValidateUser(tbLogin.Text, tbPassword.Text))
                FormsAuthentication.RedirectFromLoginPage(tbLogin.Text, false);
            else labMessage.Text = ValidateError;
        }
    }
}