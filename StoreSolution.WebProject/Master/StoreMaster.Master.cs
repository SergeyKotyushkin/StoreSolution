using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;

namespace StoreSolution.WebProject.Master
{
    public partial class StoreMaster : MasterPage
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

        public bool BtnSignOutVisibility
        {
            get { return btnSignOut.Visible; }
            set { btnSignOut.Visible = value; }
        }

        public void HiddenMoney()
        {
            rub_ru_RU.Visible = usd_en_US.Visible = gbp_en_GB.Visible = false;
        }


        public void SignOut(bool saveSession)
        {
            var user = Membership.GetUser();
            if (user != null) Logger.Log.Error(string.Format("User {0} sing out.", user.UserName));

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now);

            FormsAuthentication.SignOut();
            if(!saveSession) Session.Abandon();
            Response.Redirect("~/Index.aspx");
            Response.End();
        }

        public virtual CultureInfo GetCurrencyCultureInfo()
        {
            var cookie = Request.Cookies.Get("currencyCultureName");
            var currencyCultureName = CultureInfo.CurrentCulture.Name;
            if (cookie == null)
                Response.Cookies.Set(new HttpCookie("currencyCultureName", currencyCultureName));
            else
                currencyCultureName = cookie.Value;

            return new CultureInfo(currencyCultureName);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MarkCurrentCulture(CultureInfo.CurrentCulture);

            var cookie = Request.Cookies.Get("currencyCultureName");
            var currency = CultureInfo.CurrentCulture.Name;
            if (cookie == null)
                Response.Cookies.Set(new HttpCookie("currencyCultureName", currency));
            else 
                currency = cookie.Value;

            MarkCurrentCurrency(currency);
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            SignOut(false);
        }

        protected void lang_Click(object sender, ImageClickEventArgs e)
        {
            var button = sender as ImageButton;
            if (button == null) return;

            var id = button.ID.Replace('_', '-');
            Response.Cookies.Set(new HttpCookie("language", id));
            Response.Redirect(Request.RawUrl);
        }

        protected void langMoney_Click(object sender, ImageClickEventArgs e)
        {
            var button = sender as ImageButton;
            if (button == null) return;

            var id = button.ID.Substring(4).Replace('_', '-');
            Response.Cookies.Set(new HttpCookie("currencyCultureName", id));
            Response.Redirect(Request.RawUrl);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            if (Page.Title == LangSetter.Set("Basket_Title")) Response.Redirect("~/User/ProductCatalog.aspx");
            else if (Page.Title == LangSetter.Set("NewUser_Title")) Response.Redirect("~/Index.aspx");
            else if (Page.Title == LangSetter.Set("Profile_Title")) Response.Redirect("~/Index.aspx");
            else if (Page.Title == LangSetter.Set("ProductManagement_Title")) SignOut(false);
        }

        private void MarkCurrentCulture(CultureInfo ci)
        {
            var imageButtons = new[] { en_US, ru_RU };
            var imageButton = imageButtons.FirstOrDefault(b => b.ID.Replace('_', '-') == ci.Name) ?? en_US;

            imageButton.BorderStyle = BorderStyle.Solid;
            imageButton.BorderColor = Color.White;
            imageButton.BorderWidth = 1;
            imageButton.Style.Add("padding", "3px");
        }

        private void MarkCurrentCurrency(string cultureName)
        {
            var imageButtons = new[] { rub_ru_RU, usd_en_US, gbp_en_GB };
            var imageButton = imageButtons.FirstOrDefault(b => b.ID.Substring(4).Replace('_', '-') == cultureName) ??
                              usd_en_US;

            imageButton.BorderStyle = BorderStyle.Solid;
            imageButton.BorderColor = Color.White;
            imageButton.BorderWidth = 1;
            imageButton.Style.Add("padding", "1px");
        }
    }
}