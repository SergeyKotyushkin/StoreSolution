using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreSolution.WebProject.Currency.Contracts;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StructureMap;

namespace StoreSolution.WebProject.Master
{
    public partial class StoreMaster : MasterPage
    {
        private readonly ICurrencyConverter _currencyConverter;

        public StoreMaster():this(ObjectFactory.GetInstance<ICurrencyConverter>())
        {
            
        }

        public StoreMaster(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

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
            rub.Visible = usd.Visible = gbp.Visible = false;
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
            var cookie = Request.Cookies.Get("currency");
            var currency = cookie != null
                ? cookie.Value
                : _currencyConverter.GetCurrencyNameForCulture(CultureInfo.CurrentCulture.Name);

            if (cookie == null) Response.Cookies.Set(new HttpCookie("currency", currency));

            var currentCultureName =
                _currencyConverter.GetCultureNameForCurrency(currency);

            return
                CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == currentCultureName) ??
                CultureInfo.CurrentCulture;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            MarkCurrentCulture(CultureInfo.CurrentCulture);

            var cookie = Request.Cookies.Get("currency");
            var currency = cookie != null
                ? cookie.Value
                : _currencyConverter.GetCurrencyNameForCulture(CultureInfo.CurrentCulture.Name);

            if(cookie == null) Response.Cookies.Set(new HttpCookie("currency", currency));

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

            var id = button.ID.ToUpper();
            Response.Cookies.Set(new HttpCookie("currency", id));
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

        private void MarkCurrentCurrency(string currency)
        {
            var imageButtons = new[] { rub, usd, gbp };
            var imageButton = imageButtons.FirstOrDefault(b => b.ID.ToUpper() == currency) ?? usd;

            imageButton.BorderStyle = BorderStyle.Solid;
            imageButton.BorderColor = Color.White;
            imageButton.BorderWidth = 1;
            imageButton.Style.Add("padding", "1px");
        }
    }
}