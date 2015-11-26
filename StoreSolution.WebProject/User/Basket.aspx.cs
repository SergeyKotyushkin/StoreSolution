using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.Mail.Contracts;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : Page
    {
        private const string PageIndexNameInRepository = "pageIndexNameBasket";
        private const string CurrencyCultureName = "currencyCultureName";
        private const string SmtpSectionPath = "system.net/mailSettings/smtp";

        private StoreMaster _master;
        private readonly IEfOrderHistoryRepository _efOrderHistoryRepository;
        private readonly IMailSender _mailSender;
        private readonly IUserGroup _userGroup;
        private readonly ILangSetter _langSetter;
        private readonly ICurrencyCultureInfoService _currencyCultureInfoService;
        private readonly IGridViewBasketManager _gridViewBasketManager;

        protected Basket()
            : this(StructureMapFactory.Resolve<IEfOrderHistoryRepository>(), StructureMapFactory.Resolve<IMailSender>(),
                StructureMapFactory.Resolve<IUserGroup>(), StructureMapFactory.Resolve<ILangSetter>(),
                StructureMapFactory.Resolve<ICurrencyCultureInfoService>(),
                StructureMapFactory.Resolve<IGridViewBasketManager>())
        {
        }

        protected Basket(IEfOrderHistoryRepository efOrderHistoryRepository, IMailSender mailSender,
            IUserGroup userGroup, ILangSetter langSetter, ICurrencyCultureInfoService currencyCultureInfoService,
            IGridViewBasketManager gridViewBasketManager)
        {
            _efOrderHistoryRepository = efOrderHistoryRepository;
            _mailSender = mailSender;
            _userGroup = userGroup;
            _langSetter = langSetter;
            _currencyCultureInfoService = currencyCultureInfoService;
            _gridViewBasketManager = gridViewBasketManager;
        }

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
            _master = (StoreMaster) Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            var user = _userGroup.GetUser();

            SetTitles(user.UserName);

            if (!Page.IsPostBack)
                FillGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser();

            var currencyCultureInfo = _currencyCultureInfoService.GetCurrencyCultureInfo(Request.Cookies,
                CurrencyCultureName);

            var orderItemsList = _gridViewBasketManager.GetOrderItemsList(Session, currencyCultureInfo).ToArray();

            _efOrderHistoryRepository.Add(orderItemsList, user, currencyCultureInfo);

            var @from = ((SmtpSection) ConfigurationManager.GetSection(SmtpSectionPath)).From;
            var mailMessageSuject = _langSetter.Set("Basket_MailMessageSubject");

            _mailSender.Create(@from, user.Email, mailMessageSuject, orderItemsList, true, CultureInfo.CurrentCulture);
            _mailSender.Send();

            Logger.Log.Info(string.Format("Products has bought by user - {0}. {1}", user.UserName, labTotal.Text));
            Session["Bought"] = 1;
            Session["CurrentOrder"] = null;
            Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void GV_table_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gv = (GridView) sender;
            gv.PageIndex = e.NewPageIndex;
        }

        protected void GV_table_PageIndexChanged(object sender, EventArgs e)
        {
            FillGridView();
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            if (gvTable.Rows.Count == 0) return;

            var cultureTo = _currencyCultureInfoService.GetCurrencyCultureInfo(Request.Cookies,
                CurrencyCultureName);
            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                gvTable.Rows[i].Cells[1].Text = decimal.Parse(gvTable.Rows[i].Cells[1].Text).ToString("C", cultureTo);
                gvTable.Rows[i].Cells[3].Text = decimal.Parse(gvTable.Rows[i].Cells[3].Text).ToString("C", cultureTo);
            }
        }

        protected void gvTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Header) return;

            e.Row.Cells[0].Text = _langSetter.Set("Basket_HeaderName");
            e.Row.Cells[1].Text = _langSetter.Set("Basket_HeaderPrice");
            e.Row.Cells[2].Text = _langSetter.Set("Basket_HeaderCount");
            e.Row.Cells[3].Text = _langSetter.Set("Basket_HeaderTotalPrice");
        }


        private void FillGridView()
        {
            var cultureTo = _currencyCultureInfoService.GetCurrencyCultureInfo(Request.Cookies,
                CurrencyCultureName);

            var data = _gridViewBasketManager.GetOrderItemsList(Session, cultureTo);

            _gridViewBasketManager.Fill(gvTable, data);

            SetUiProperties(data, cultureTo);
        }

        private void SetUiProperties(IQueryable<OrderItem> data, IFormatProvider cultureTo)
        {
            labTotal.Text = string.Format(_langSetter.Set("Basket_Total"),
                data.Sum(p => p.Total).ToString("C", cultureTo));

            btnBuy.Enabled = true;
            if (data.Count() != 0) return;

            btnBuy.Enabled = false;
            labTotal.Text = _langSetter.Set("Basket_EmptyOrder");
        }

        private void SetTitles(string userName)
        {
            _master.HlUserText = userName;
        }
    }
}