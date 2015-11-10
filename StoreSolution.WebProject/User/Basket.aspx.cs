using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using StoreSolution.WebProject.Model;
using System.Web.Security;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.MyIoC;
using StoreSolution.WebProject.Currency;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.User
{
    public partial class Basket : System.Web.UI.Page
    {
        private StoreMaster _master;
        private const double Tolerance = double.Epsilon;
        private readonly IProductRepository _productRepository;

        protected Basket()
            : this(SimpleContainer.Resolve<IProductRepository>())
        {
            
        }

        protected Basket(IProductRepository iProductRepository)
        {
            _productRepository = iProductRepository;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (null == cookie) return;
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master != null) _master.BtnBackVisibility = true;

            var user = Membership.GetUser();
            if (user == null) SignOut();

            SetTitles(user);

            if (!Page.IsPostBack)
                FillOrdersGridView();
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null)
            {
                SignOut();
                return;
            }

            Logger.Log.Info("Products has bought by user - " + user.UserName + ". " + labTotal.Text);
            Session["Bought"] = 1;
            Session["CurrentOrder"] = null;
            Response.Redirect("~/User/ProductCatalog.aspx");
        }

        protected void btnSignOut_Click(object sender, EventArgs e)
        {
            SignOut();
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

            var rate = CurrencyConverter.GetRate(CultureInfo.CurrentCulture);
            decimal sum = 0;
            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                var price = CurrencyConverter.ConvertFromRu(decimal.Parse(gvTable.Rows[i].Cells[1].Text), rate);
                var total = decimal.Parse(gvTable.Rows[i].Cells[2].Text)*price;
                sum += total;
                gvTable.Rows[i].Cells[1].Text = string.Format("{0:c}", price);
                gvTable.Rows[i].Cells[3].Text = string.Format("{0:c}", total);
            }

            var text = (string)HttpContext.GetGlobalResourceObject("Lang", "Basket_Total");
            if (text != null) labTotal.Text = string.Format(text, sum);
        }

        protected void gvTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Header) return;
            e.Row.Cells[0].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "Basket_HeaderName");
            e.Row.Cells[1].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "Basket_HeaderPrice");
            e.Row.Cells[2].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "Basket_HeaderCount");
            e.Row.Cells[3].Text = (string)HttpContext.GetGlobalResourceObject("Lang", "Basket_HeaderTotalPrice");
        }


        private void FillOrdersGridView()
        {
            var products = _productRepository.Products.ToList();

            var orders = GetOrdersFromSession();

            var list = products.Join(orders, p => p.Id, q => q.Id, (p, q) => new { p.Name, p.Price, q.Count, Total = (q.Count * p.Price) }).ToList();
            gvTable.DataSource = list;               
            gvTable.DataBind();
            
            btnBuy.Enabled = true;
            if (list.Count != 0) return;
            btnBuy.Enabled = false;
            labTotal.Text = (string)HttpContext.GetGlobalResourceObject("Lang", "Basket_EmptyOrder");
        }

        private void SetTitles(MembershipUser user)
        {
            var hlUserText = (string)HttpContext.GetGlobalResourceObject("Lang", "Master_ToProfile");
            if (hlUserText != null) _master.HlUserText = string.Format(hlUserText, user.UserName);
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

        private IEnumerable<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }
    }
}