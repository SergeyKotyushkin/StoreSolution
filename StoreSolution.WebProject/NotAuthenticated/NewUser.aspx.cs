using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StoreSolution.WebProject
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
                    "Login must have only letters, numbers and underscope. " +
                    "First symbol must be letter. " +
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
                    Roles.AddUserToRole(tbLogin.Text, "User");
                    Session["NewUser"] = "Yes";
                    FormsAuthentication.RedirectToLoginPage();
                }
                else
                {
                    labMessage.Text = "Error create new user.";
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            FormsAuthentication.RedirectToLoginPage();
        }

    }
}