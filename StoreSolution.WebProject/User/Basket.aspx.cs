using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using StoreSolution.WebProject.Model;
using System.Web.Security;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.WebProject.Currency.Contracts;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;
using StructureMap;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : System.Web.UI.Page
    {
        private StoreMaster _master;
        private readonly IProductRepository _productRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly ICurrencyConverter _currencyConverter;

        protected Basket()
            : this(
                ObjectFactory.GetInstance<IProductRepository>(), ObjectFactory.GetInstance<IOrderHistoryRepository>(),
                ObjectFactory.GetInstance<ICurrencyConverter>())
        {
        }

        protected Basket(IProductRepository productRepository, IOrderHistoryRepository orderHistoryRepository, ICurrencyConverter currencyConverter)
        {
            _productRepository = productRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _currencyConverter = currencyConverter;
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
            
            var user = Membership.GetUser();
            if (user == null)
            {
                _master.SignOut(false);
                return;
            }

            SetTitles(user);

            if (!Page.IsPostBack)
                FillOrdersGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                _master.SignOut(false);
                return;
            }

            var products = _productRepository.Products.ToList();
            var orders = GetOrdersFromSession();
            var cultureInfo = _master.GetCurrencyCultureInfo();
            var list = products.Join(orders, p => p.Id, q => q.Id, (p, q) => new
            {
                p.Name, 
                Price = _currencyConverter.ConvertFromRu(p.Price, cultureInfo.Name),
                q.Count,
                Total = (q.Count * _currencyConverter.ConvertFromRu(p.Price, cultureInfo.Name))
            }).ToList();

            var total = list.Sum(p => p.Total);

            var jsonSerialiser = new JavaScriptSerializer();
            var order = jsonSerialiser.Serialize(list);

            var orderToHistory = new OrderHistory
            {
                Order = order,
                PersonName = user.UserName,
                PersonEmail = user.Email,
                Total = total,
                Date = DateTime.Now,
                Culture = cultureInfo.Name
            };

            _orderHistoryRepository.AddOrUpdate(orderToHistory);

            var orderList = string.Format("{0}</ul>", list.Aggregate("<ul>",
                            (current, p) =>
                                current +
                                string.Format(LangSetter.Set("Basket_MailOrderList"), p.Name, p.Count,
                                    p.Price.ToString("C", cultureInfo))));

            var text =
                string.Format(LangSetter.Set("Basket_MailMessage"), DateTime.Now.Date.ToShortDateString(), orderList,
                    total.ToString("C", cultureInfo));
            SendEmailToConsumer(user, text);

            Logger.Log.Info(string.Format("Products has bought by user - {0}. {1}", user.UserName, labTotal.Text));
            Session["Bought"] = 1;
            Session["CurrentOrder"] = null;
            Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void GV_table_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTable.PageIndex = e.NewPageIndex;
        }

        protected void GV_table_PageIndexChanged(object sender, EventArgs e)
        {
            FillOrdersGridView();
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            if(gvTable.Rows.Count == 0) return;

            var cultureInfo = _master.GetCurrencyCultureInfo();

            var rate = _currencyConverter.GetRate(cultureInfo.Name);
            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                var price = _currencyConverter.ConvertFromRu(decimal.Parse(gvTable.Rows[i].Cells[1].Text), rate);
                gvTable.Rows[i].Cells[1].Text = price.ToString("C", cultureInfo);
                gvTable.Rows[i].Cells[3].Text = decimal.Parse(gvTable.Rows[i].Cells[3].Text).ToString("C", cultureInfo);
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
            var products = _productRepository.Products.ToList();

            var orders = GetOrdersFromSession();

            var cultureInfo = _master.GetCurrencyCultureInfo();
            var list =
                products.Join(orders, p => p.Id, q => q.Id,
                    (p, q) =>
                        new
                        {
                            p.Name,
                            p.Price,
                            q.Count,
                            Total = (q.Count * _currencyConverter.ConvertFromRu(p.Price, cultureInfo.Name))
                        })
                    .ToList();
            gvTable.DataSource = list;               
            gvTable.DataBind();

            var sum = list.Sum(p => p.Total);

            var text = LangSetter.Set("Basket_Total");
            if (text != null) labTotal.Text = string.Format(text, sum.ToString("C", cultureInfo));
            
            btnBuy.Enabled = true;
            if (list.Count != 0) return;
            btnBuy.Enabled = false;
            labTotal.Text = LangSetter.Set("Basket_EmptyOrder");
        }

        private void SetTitles(MembershipUser user)
        {
            var hlUserText = LangSetter.Set("Master_ToProfile");
            if (hlUserText != null) _master.HlUserText = string.Format(hlUserText, user.UserName);
        }
        
        private IEnumerable<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        private static void SendEmailToConsumer(MembershipUser user, string text)
        {
            var admin = Membership.GetUser("Sergey");
            if (admin == null) return;

            using (var message = new MailMessage())
            {
                message.From = new MailAddress("OnlineStore@admmin");
                message.To.Add(new MailAddress(user.Email));
                message.CC.Add(new MailAddress(user.Email));
                message.Subject = "Online Store Alert!";
                message.IsBodyHtml = true;
                message.Body = text;
                using (var client = new SmtpClient())
                {
                    client.Send(message);
                }
            }
        }
    }
}