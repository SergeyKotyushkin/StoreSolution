using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.UI;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Model;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.Mail.Contracts;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : Page
    {
        private const string SmtpSectionPath = "system.net/mailSettings/smtp";
        private StoreMaster _master;
        private readonly IEfProductRepository _efProductRepository;
        private readonly IEfOrderHistoryRepository _efOrderHistoryRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IOrderRepository _orderRepository;
        private readonly IMailSender _mailSender;
        private readonly IUserGroup _userGroup;
        private readonly ILangSetter _langSetter;

        protected Basket()
            : this(
                StructureMapFactory.Resolve<IEfProductRepository>(),
                StructureMapFactory.Resolve<IEfOrderHistoryRepository>(),
                StructureMapFactory.Resolve<ICurrencyConverter>(), StructureMapFactory.Resolve<IMailSender>(),
                StructureMapFactory.Resolve<IUserGroup>(), StructureMapFactory.Resolve<IOrderRepository>(),
                StructureMapFactory.Resolve<ILangSetter>())
        {
        }

        protected Basket(IEfProductRepository efProductRepository, IEfOrderHistoryRepository efOrderHistoryRepository,
            ICurrencyConverter currencyConverter, IMailSender mailSender, IUserGroup userGroup,
            IOrderRepository orderRepository, ILangSetter langSetter)
        {
            _efProductRepository = efProductRepository;
            _efOrderHistoryRepository = efOrderHistoryRepository;
            _currencyConverter = currencyConverter;
            _mailSender = mailSender;
            _userGroup = userGroup;
            _orderRepository = orderRepository;
            _langSetter = langSetter;
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

            var user = _userGroup.GetUser(false);

            SetTitles(user.UserName);

            if (!Page.IsPostBack)
                FillOrdersGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser(false);

            var products = _efProductRepository.Products.ToArray();
            var orders = _orderRepository.GetAll(Session).ToArray();
            var currencyCultureInfo = _master.GetCurrencyCultureInfo();
            var orderItemsList = GetOrderItemsList(products, orders, currencyCultureInfo).ToArray();

            SaveOrderToDatabase(orderItemsList, user, currencyCultureInfo);

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
            FillOrdersGridView();
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            if (gvTable.Rows.Count == 0) return;

            var cultureTo = _master.GetCurrencyCultureInfo();
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


        private void FillOrdersGridView()
        {
            var products = _efProductRepository.Products.ToArray();
            var orders = _orderRepository.GetAll(Session);

            var cultureTo = _master.GetCurrencyCultureInfo();
            var list = GetOrderItemsList(products, orders, cultureTo).ToArray();
            gvTable.DataSource = list;
            gvTable.DataBind();

            labTotal.Text = string.Format(_langSetter.Set("Basket_Total"),
                list.Sum(p => p.Total).ToString("C", cultureTo));

            btnBuy.Enabled = true;
            if (list.Length != 0) return;

            btnBuy.Enabled = false;
            labTotal.Text = _langSetter.Set("Basket_EmptyOrder");
        }

        private void SetTitles(string userName)
        {
            _master.HlUserText = string.Format(_langSetter.Set("Master_ToProfile"), userName);
        }

        private IEnumerable<OrderItem> GetOrderItemsList(IEnumerable<Product> products, IEnumerable<Order> orders,
            CultureInfo culture)
        {
            var rate = _currencyConverter.GetRate(new CultureInfo("ru-Ru"), culture, DateTime.Now);
            return products.Join(orders, p => p.Id, q => q.Id, (p, q) => new OrderItem
            {
                Name = p.Name,
                Price = _currencyConverter.ConvertByRate(p.Price, rate),
                Count = q.Count,
                Total = (q.Count*_currencyConverter.ConvertByRate(p.Price, rate))
            });
        }

        private void SaveOrderToDatabase(IEnumerable<OrderItem> orderItems, MembershipUser user, CultureInfo culture)
        {
            _efOrderHistoryRepository.Add(orderItems, user, culture);
        }
    }
}