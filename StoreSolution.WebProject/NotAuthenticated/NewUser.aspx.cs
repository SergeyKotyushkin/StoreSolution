using System;
using System.Text.RegularExpressions;
using System.Web.Security;
using StoreSolution.WebProject.Log4net;

namespace StoreSolution.WebProject.NotAuthenticated
{
    public partial class NewUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            labMessage.Text = "";
            var rgxLogin = new Regex("^[a-zA-Z]+[a-zA-Z0-9_]{5,}$");
            if (!rgxLogin.IsMatch(tbLogin.Text))
            {
                labMessage.Text =
                    "Login must have only letters, numbers and underscopes. " +
                    "First symbol must be letter. Length must be more than 5 symbols.";
                rfvLogin.IsValid = false;
                return;
            }

            var rgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");
            if (!rgxPassword.IsMatch(tbPassword.Text))
            {
                labMessage.Text =
                    "Password must have letters, numbers and symbols: _,!,@,#,$,%,^,&,*. " +
                    "First symbol must be letter. Length must be more than 5 symbols.";
                rfvPassword.IsValid = false;
                return;
            }

            if (!Page.IsValid)
            {
                labMessage.Text = "Some fields are emprty.";
            }
            else
            {
                MembershipCreateStatus status;
                Membership.CreateUser(tbLogin.Text, tbPassword.Text, tbEmail.Text, tbQuestion.Text,
                    tbAnswer.Text, true, out status);

                if (status == MembershipCreateStatus.Success)
                {
                    Logger.Log.Info("User " + tbLogin.Text + " successfully created.");
                    Roles.AddUserToRole(tbLogin.Text, "User");
                    Session["NewUser"] = "Yes";
                    FormsAuthentication.RedirectToLoginPage();
                }
                else
                {
                    labMessage.Text = "Error create new user.";
                    Logger.Log.Debug("User " + tbLogin.Text + " didn't create.");
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Logger.Log.Debug("Redirected to Index page.");
            Response.Redirect("~/Index.aspx");
        }

    }
}