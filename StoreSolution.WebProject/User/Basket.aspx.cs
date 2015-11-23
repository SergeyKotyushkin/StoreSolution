using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Web.UI;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.WebProject.Currency.Contracts;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;
using StoreSolution.WebProject.Model;
using StoreSolution.WebProject.StructureMap;
using StoreSolution.WebProject.User.MailSender.Contracts;
using StoreSolution.WebProject.User.OrderRepository.Contracts;
using StoreSolution.WebProject.UserGruop.Contracts;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : Page
    {
        private const string SmtpSectionPath = "system.net/mailSettings/smtp";
        private StoreMaster _master;
        private readonly IProductRepository _productRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IOrderRepository _orderRepository;
        private readonly IMailSender _mailSender;
        private readonly IUserGroup _userGroup;

        protected Basket()
            : this(
                StructureMapFactory.Resolve<IProductRepository>(), StructureMapFactory.Resolve<IOrderHistoryRepository>(),
                StructureMapFactory.Resolve<ICurrencyConverter>(), StructureMapFactory.Resolve<IMailSender>(),
                StructureMapFactory.Resolve<IUserGroup>(), StructureMapFactory.Resolve<IOrderRepository>())
        {
        }

        protected Basket(IProductRepository productRepository, IOrderHistoryRepository orderHistoryRepository,
            ICurrencyConverter currencyConverter, IMailSender mailSender, IUserGroup userGroup, IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _currencyConverter = currencyConverter;
            _mailSender = mailSender;
            _userGroup = userGroup;
            _orderRepository = orderRepository;
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
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");
            
            var user = _userGroup.GetUser();

            SetTitles(user.UserName);

            if (!Page.IsPostBack)
                FillOrdersGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser();

            var products = _productRepository.Products.ToArray();
            var orders = _orderRepository.GetAll(Session).ToArray();
            var currencyCultureInfo = _master.GetCurrencyCultureInfo();
            var orderItemsList = GetOrderItemsList(products, orders, currencyCultureInfo).ToArray();

            SaveOrderToDatabase(orderItemsList, user, currencyCultureInfo);

            var from = ((SmtpSection)ConfigurationManager.GetSection(SmtpSectionPath)).From;
            var mailMessageSuject = LangSetter.Set("Basket_MailMessageSubject");
            var mailMessageBody = GetMailMessageBody(orderItemsList, currencyCultureInfo);

            _mailSender.Send(from, user.Email, mailMessageSuject, mailMessageBody, true);
            
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
            if(gvTable.Rows.Count == 0) return;

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

            e.Row.Cells[0].Text = LangSetter.Set("Basket_HeaderName");
            e.Row.Cells[1].Text = LangSetter.Set("Basket_HeaderPrice");
            e.Row.Cells[2].Text = LangSetter.Set("Basket_HeaderCount");
            e.Row.Cells[3].Text = LangSetter.Set("Basket_HeaderTotalPrice");
        }


        private void FillOrdersGridView()
        {
            var products = _productRepository.Products.ToArray();
            var orders = _orderRepository.GetAll(Session);

            var cultureTo = _master.GetCurrencyCultureInfo();
            var list = GetOrderItemsList(products, orders, cultureTo).ToArray();
            gvTable.DataSource = list;               
            gvTable.DataBind();

            labTotal.Text = string.Format(LangSetter.Set("Basket_Total"),
                list.Sum(p => p.Total).ToString("C", cultureTo));

            btnBuy.Enabled = true;
            if (list.Length != 0) return;

            btnBuy.Enabled = false;
            labTotal.Text = LangSetter.Set("Basket_EmptyOrder");
        }

        private void SetTitles(string userName)
        {
            _master.HlUserText = string.Format(LangSetter.Set("Master_ToProfile"), userName);
        }

        private static string GetMailMessageBody(IEnumerable<OrderItem> orderItemsList, IFormatProvider currencyCultureInfo)
        {
            var mailMessageBody = CreateMailMessageBody(orderItemsList, currencyCultureInfo);

            return mailMessageBody;
        }

        private IEnumerable<OrderItem> GetOrderItemsList(IEnumerable<Product> products, IEnumerable<Order> orders,
            CultureInfo currencyCultureInfo)
        {
            var rate = _currencyConverter.GetRate(new CultureInfo("ru-Ru"), currencyCultureInfo, DateTime.Now);
            return products.Join(orders, p => p.Id, q => q.Id, (p, q) => new OrderItem
            {
                Name = p.Name,
                Price = _currencyConverter.ConvertByRate(p.Price, rate),
                Count = q.Count,
                Total = (q.Count*_currencyConverter.ConvertByRate(p.Price, rate))
            });
        }

        private void SaveOrderToDatabase(IEnumerable<OrderItem> orderItemsList, MembershipUser user,
            CultureInfo currencyCultureInfo)
        {
            var orderToHistory = CreateOrderToHistory(orderItemsList, user, currencyCultureInfo);

            _orderHistoryRepository.AddOrUpdate(orderToHistory);
        }

        private static OrderHistory CreateOrderToHistory(IEnumerable<OrderItem> orderItemsList, MembershipUser user,
            CultureInfo currencyCultureInfo)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var order = jsonSerialiser.Serialize(orderItemsList);

            return new OrderHistory
            {
                Order = order,
                PersonName = user.UserName,
                PersonEmail = user.Email,
                Total = orderItemsList.Sum(p => p.Total),
                Date = DateTime.Now,
                Culture = currencyCultureInfo.Name
            };
        }

        private static string CreateMailMessageBody(IEnumerable<OrderItem> orderItemsList,
            IFormatProvider currencyCultureInfo)
        {
            var orderItems = orderItemsList.ToArray();

            var orderList = string.Format("{0}</ul>", orderItems.Aggregate("<ul>",
                (current, p) =>
                    current +
                    string.Format(
                        LangSetter.Set("Basket_MailOrderList"), 
                        p.Name, 
                        p.Count,
                        p.Price.ToString("C", currencyCultureInfo))));

            var total = orderItems.Sum(p => p.Total);

            var mailMessageBody =
                string.Format(
                    LangSetter.Set("Basket_MailMessage"),
                    DateTime.Now.Date.ToShortDateString(),
                    orderList,
                    total.ToString("C", currencyCultureInfo));

            return mailMessageBody;
        }
    }
}