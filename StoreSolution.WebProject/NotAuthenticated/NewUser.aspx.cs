using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Web;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.NotAuthenticated
{
    public partial class NewUser : System.Web.UI.Page
    {
        private static readonly Color ErrorColor = Color.Red;
        private static readonly Regex RgxLogin = new Regex("^[a-zA-Z]+[a-zA-Z0-9_]{5,}$");
        private static readonly Regex RgxPassword = new Regex("^[a-zA-Z0-9_!@#$%^&*]{5,}$");

        private StoreMaster _master;

        private readonly IUserGroup _userGroup;

        public NewUser() : this(StructureMapFactory.Resolve<IUserGroup>())
        {
        }

        public NewUser(IUserGroup userGroup)
        {
            _userGroup = userGroup;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Add(cookie);
            }
            Page.Culture = Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            SetUiProperties();
        }
        
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();

            _master.SetLabMessage(Color.Empty, string.Empty);
            
            if (!RgxLogin.IsMatch(tbLogin.Text))
            {
                _master.SetLabMessage(ErrorColor, "NewUser_LoginError");
                rfvLogin.IsValid = false;
                return;
            }

            if (!RgxPassword.IsMatch(tbPassword.Text))
            {
                _master.SetLabMessage(ErrorColor, "NewUser_PasswordError");
                rfvPassword.IsValid = false;
                return;
            }

            if (!Page.IsValid) 
                _master.SetLabMessage(ErrorColor, "NewUser_EmptyFieldsError");
            else
            {
                if (_userGroup.CreateUser(tbLogin.Text, tbPassword.Text, tbEmail.Text, tbQuestion.Text, tbAnswer.Text))
                {
                    Logger.Log.Info(string.Format("User {0} successfully created.", tbLogin.Text));
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
        

        private void SetUiProperties()
        {
            _master.HideMoney();
            _master.BtnSignOutVisibility = false;
        }

    }
}