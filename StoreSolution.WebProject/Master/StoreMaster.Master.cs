using System;
using System.Drawing;
using System.Web.Security;
using StoreSolution.WebProject.Log4net;

namespace StoreSolution.WebProject.Master
{
    public partial class StoreMaster : System.Web.UI.MasterPage
    {
        public string HlUserText
        {
            get { return hlUser.Text; }
            set { hlUser.Text = value; }
        }

        public string LabMessageText
        {
            get { return labMessage.Text; }
            set { labMessage.Text = value; }
        }

        public Color LabMessageForeColor
        {
            get { return labMessage.ForeColor; }
            set { labMessage.ForeColor = value; }
        }

        public bool BtnBackVisibility
        {
            get { return btnBack.Visible; }
            set { btnBack.Visible = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
        }

        private void SignOut()
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                Logger.Log.Error("No user at Product Management page start.");
                Logger.Log.Error("Sign out.");
            }
            else Logger.Log.Error("User " + user.UserName + " sing out.");

            Session.Abandon();
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
            Response.End();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Page.Title == "Your order") Response.Redirect("~/User/ProductCatalog.aspx");
            else if (Page.Title == "Product management") SignOut();
        }
    }
}