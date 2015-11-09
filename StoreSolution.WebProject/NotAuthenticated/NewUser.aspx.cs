using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using StoreSolution.WebProject.Log4net;

namespace StoreSolution.WebProject.NotAuthenticated
{
    public partial class NewUser : System.Web.UI.Page
    {
        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (null == cookie) return;
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

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
                labMessage.Text = (string) HttpContext.GetGlobalResourceObject("Lang", "NewUser_LoginError");
                rfvLogin.IsValid = false;
                return;
            }

            var rgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");
            if (!rgxPassword.IsMatch(tbPassword.Text))
            {
                labMessage.Text = (string)HttpContext.GetGlobalResourceObject("Lang", "NewUser_PasswordError");
                rfvPassword.IsValid = false;
                return;
            }

            if (!Page.IsValid)
            {
                labMessage.Text = (string)HttpContext.GetGlobalResourceObject("Lang", "NewUser_EmptyFieldsError");
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
                    labMessage.Text = (string)HttpContext.GetGlobalResourceObject("Lang", "NewUser_CreateUserError");
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