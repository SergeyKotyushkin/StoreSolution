﻿using System;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject
{
    public partial class Index : Page
    {
        private StoreMaster _master;
        private readonly Color _successColor = Color.ForestGreen;
        private readonly Color _errorColor = Color.Red;
        private readonly IUserGroup _userGroup;

        public Index() : this(StructureMapFactory.Resolve<IUserGroup>())
        {
        }

        public Index(IUserGroup userGroup)
        {
            _userGroup = userGroup;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Set(cookie);
            }
            Page.Culture = Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            SetUiProperties();

            ShowMessageIfNewUserWasCreated();

            var user = _userGroup.GetUser(true);

            if (user != null)
                RedirectFromIndexByRole(user.UserName);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid && Membership.ValidateUser(tbLogin.Text, tbPassword.Text))
            {
                Logger.Log.Info(string.Format("User {0} logged in.", tbLogin.Text));
                FormsAuthentication.SetAuthCookie(tbLogin.Text, false);
                RedirectFromIndexByRole(tbLogin.Text);
            }
            else
            {
                Logger.Log.Error(string.Format("User {0} didn't log in.", tbLogin.Text));
                _master.SetLabMessage(_errorColor, "Index_ValidateError");
            }
        }


        private void SetUiProperties()
        {
            _master.HideMoney();
            _master.BtnBackVisibility = false;
            _master.BtnSignOutVisibility = false;
        }

        private void RedirectFromIndexByRole(string userName)
        {
            if (Roles.IsUserInRole(userName, "Admin"))
            {
                Logger.Log.Debug(string.Format("Admin {0} redirected to ProductManagement page.", userName));
                Response.Redirect("~/Admin/ProductManagement.aspx");
            }
            else if (Roles.IsUserInRole(userName, "User"))
            {
                Logger.Log.Debug(string.Format("User {0} redirected to ProductCatalog page.", userName));
                Response.Redirect("~/User/ProductCatalog.aspx");
            }
        }

        private void ShowMessageIfNewUserWasCreated()
        {
            if (Session["NewUser"] == null) return;

            _master.SetLabMessage(_successColor, "Index_SuccessfullyUserCreation");
            Logger.Log.Info("Successfully user creation.");
            Session["NewUser"] = null;
        }
    }
}