using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StoreSolution.WebProject
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            MembershipCreateStatus status;
            Membership.CreateUser("Coca", "123abcd!", "your@email.tu", "Name?", "Blame", true, out status);

            


            if (status == MembershipCreateStatus.Success)
            {
                Response.Write("Success");
            }
            else
            {
                Response.Write("Fail");
            }
        }

        protected void CB_create_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                B_action.Text = "Create";
            }
            else
            {
                B_action.Text = "Log In";
            }

            L_email.Visible = L_question.Visible = L_answer.Visible = (sender as CheckBox).Checked;
            TB_email.Visible = TB_question.Visible = TB_answer.Visible = (sender as CheckBox).Checked;
        }

        protected void B_action_Click(object sender, EventArgs e)
        {
            if (!CB_create.Checked)
            {
                RequiredFieldValidator3.Enabled = false;
                RequiredFieldValidator4.Enabled = false;
                RequiredFieldValidator5.Enabled = false;
                RegularExpressionValidator3.Enabled = false;
            }

            Page.Validate();
            if (!Page.IsValid)
            {
                L_message.Text = "Mistakes in fields";
                return;
            }

            if (CB_create.Checked)
            {
                MembershipCreateStatus status;
                Membership.CreateUser(TB_login.Text, TB_password.Text, TB_email.Text,
                    TB_question.Text, TB_answer.Text, true, out status);

                if (status == MembershipCreateStatus.Success)
                {
                    Roles.AddUserToRole(TB_login.Text, "Casual");
                    FormsAuthentication.SignOut();
                    TB_login.Text = TB_password.Text = TB_email.Text = TB_question.Text = TB_answer.Text = "";
                    CB_create.Checked = false;
                    CB_create_CheckedChanged((object)CB_create, null);
                }

                L_message.Text = status.ToString();

            }
            else
            {
                var user = Membership.GetUser(TB_login.Text);
                if (user == null) return;

                if (user.IsLockedOut)
                {
                    var lastLockout = Membership.GetUser(TB_login.Text).LastLockoutDate;
                    var unlockDate = lastLockout.AddMinutes(Membership.PasswordAttemptWindow);

                    if (DateTime.Now > unlockDate) Membership.GetUser(TB_login.Text).UnlockUser();
                    else
                    {
                        L_message.Text = "It seems you were blocked. Try again at " + (unlockDate - DateTime.Now).Minutes + " minutes.";
                        return;
                    }
                }

                if (Membership.ValidateUser(TB_login.Text, TB_password.Text))
                {
                    L_message.Text = "+";

                    FormsAuthentication.RedirectFromLoginPage(TB_login.Text, false);
                }
                else
                {
                    L_message.Text = "User is missed";
                }
            }
        }
    }
}