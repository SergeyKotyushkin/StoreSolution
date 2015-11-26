using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.NotAuthenticated
{
    public partial class NewUser : System.Web.UI.Page
    {
        private static readonly Color ErrorColor = Color.Red;

        private StoreMaster _master;
        
        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Add(cookie);
            }
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            _master.HideMoney();
            _master.BtnSignOutVisibility = false;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            _master.SetLabMessage(Color.Empty, string.Empty);
            var rgxLogin = new Regex("^[a-zA-Z]+[a-zA-Z0-9_]{5,}$");
            if (!rgxLogin.IsMatch(tbLogin.Text))
            {
                _master.SetLabMessage(ErrorColor, "NewUser_LoginError");
                rfvLogin.IsValid = false;
                return;
            }

            var rgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");
            if (!rgxPassword.IsMatch(tbPassword.Text))
            {
                _master.SetLabMessage(ErrorColor, "NewUser_PasswordError");
                rfvPassword.IsValid = false;
                return;
            }

            if (!Page.IsValid) _master.SetLabMessage(ErrorColor, "NewUser_EmptyFieldsError");
            else
            {
                MembershipCreateStatus status;
                Membership.CreateUser(tbLogin.Text, tbPassword.Text, tbEmail.Text, tbQuestion.Text,
                    tbAnswer.Text, true, out status);

                if (status == MembershipCreateStatus.Success)
                {
                    Logger.Log.Info(string.Format("User {0} successfully created.", tbLogin.Text));
                    Roles.AddUserToRole(tbLogin.Text, "User");
                    Session["NewUser"] = "Yes";
                    Response.Redirect("~/Index.aspx");
                }
                else
                {
                    _master.SetLabMessage(ErrorColor, "NewUser_CreateUserError");
                    Logger.Log.Debug(string.Format("User {0} didn't create.", tbLogin.Text));
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