using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.WebProject.Currency.Contracts;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;
using StoreSolution.WebProject.Model;
using StoreSolution.WebProject.StructureMap;

namespace StoreSolution.WebProject.User
{
    public partial class ProductCatalog : System.Web.UI.Page
    {
        private bool _isSearch;
        private StoreMaster _master;

        private readonly IProductRepository _productRepository;
        private readonly ICurrencyConverter _currencyConverter;

        protected ProductCatalog()
            : this(StructureMapFactory.Resolve<IProductRepository>(), StructureMapFactory.Resolve<ICurrencyConverter>())
        {
            
        }

        protected ProductCatalog(IProductRepository productRepository, ICurrencyConverter currencyConverter)
        {
            _productRepository = productRepository;
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
                
            _master.BtnBackVisibility = false;

            var user = Membership.GetUser();
            if (user == null)
            {
                _master.SignOut(false);
                return;
            }

            pSearchingBoard.Visible = cbSearchHeader.Checked;
            _isSearch = cbSearchHeader.Checked;
            
            SetTitles(user);

            FillGridView(false);
            FillCountColumn();
        }

        protected void gvTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            var id = GetIdFromRow(gvTable.SelectedIndex);

            var orders = GetOrdersFromSession();

            AddToOrders(orders, id);
            LoadOrdersToSession(orders);
            FillCountColumn();
        }

        protected void btnBasket_Click(object sender, EventArgs e)
        {
            var user = Membership.GetUser();
            if (user == null) return;

            Logger.Log.Debug(string.Format("User {0} redirected to Basket page.", user.UserName));
            Response.Redirect("~/User/Basket.aspx");
        }
        
        protected void gvTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = GetIdFromRow(e.RowIndex);

            var orders = GetOrdersFromSession();
            RemoveFromOrders(orders, id);
            LoadOrdersToSession(orders);
            FillCountColumn();
        }

        protected void gvTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTable.PageIndex = e.NewPageIndex;
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            var cultureFrom = new CultureInfo("ru-RU");
            var cultureTo = _master.GetCurrencyCultureInfo();

            var rate = _currencyConverter.GetRate(cultureFrom, cultureTo, DateTime.Now);
            foreach (GridViewRow row in gvTable.Rows)
            {
                var price = _currencyConverter.ConvertByRate(decimal.Parse(row.Cells[6].Text), rate);
                row.Cells[6].Text = price.ToString("C", cultureTo);
            }
        }

        protected void gvTable_RowCreated(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    e.Row.Cells[3].CssClass = "hiddenСolumn";
                    break;
                case DataControlRowType.Header:
                    e.Row.Cells[3].CssClass = "hiddenСolumn";
                    break;
            }
        }

        protected void gvTable_PageIndexChanged(object sender, EventArgs e)
        {
            FillGridView(true);
            FillCountColumn();
        }

        protected void gvTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Header) return;
            e.Row.Cells[1].Text = LangSetter.Set("ProductCatalog_HeaderCount");
            e.Row.Cells[4].Text = LangSetter.Set("ProductCatalog_HeaderName");
            e.Row.Cells[5].Text = LangSetter.Set("ProductCatalog_HeaderCategory");
            e.Row.Cells[6].Text = LangSetter.Set("ProductCatalog_HeaderPrice");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            FillGridView(true);
            FillCountColumn();
        }

        protected void cbSearchHeader_CheckedChanged(object sender, EventArgs e)
        {
            _isSearch = cbSearchHeader.Checked;

            if(cbSearchHeader.Checked) return;
            ddlSearchCategory.SelectedIndex = 0;
            tbSearchName.Text = string.Empty;
            FillGridView(true);
            FillCountColumn();
        }

        
        private void SetTitles(MembershipUser user)
        {
            var hlUserText = LangSetter.Set("Master_ToProfile");
            if (hlUserText != null) _master.HlUserText = string.Format(hlUserText, user.UserName);

            if (Session["Bought"] == null) return;
            _master.LabMessageForeColor = Color.DarkGreen;
            _master.LabMessageText = LangSetter.Set("ProductCatalog_ProductsBought");
            Session["Bought"] = null;
        }

        private void FillGridView(bool bind)
        {
            var products = _productRepository.Products;

            if (_isSearch)
            {
                if (tbSearchName.Text.Trim() != string.Empty)
                    products = products.Where(p => p.Name.ToLower().Contains(tbSearchName.Text.Trim().ToLower())).Select(p => p);
                if (ddlSearchCategory.SelectedIndex != 0)
                {
                    var category = ddlSearchCategory.Items[ddlSearchCategory.SelectedIndex].Text;
                    products =
                        products.Where(p => p.Category == category).Select(p => p);
                }
                
                FillCategories(products);
            }

            if (!Page.IsPostBack)
            {
                FillCategories(products);
            }

            gvTable.DataSource = products.Select(p => new
            {
                p.Id, 
                p.Name, 
                p.Category, 
                p.Price
            }).ToList();
            if (!Page.IsPostBack || bind)
                gvTable.DataBind();
        }

        private void FillCategories(IQueryable<Product> products)
        {
            var categories = products.Select(p => p.Category).Distinct().ToList();
            var ddlSearchCategorySelectedIndex = ddlSearchCategory.SelectedIndex;
            ddlSearchCategory.Items.Clear();
            ddlSearchCategory.Items.Add(LangSetter.Set("ProductCatalog_AllCategories"));
            foreach (var category in categories) ddlSearchCategory.Items.Add(category);
            ddlSearchCategory.SelectedIndex = ddlSearchCategorySelectedIndex < ddlSearchCategory.Items.Count
                ? ddlSearchCategorySelectedIndex
                : ddlSearchCategory.Items.Count - 1;
        }

        private void FillCountColumn()
        {
            var orders = GetOrdersFromSession();

            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                var id = GetIdFromRow(i);
                var foo = orders.Find(order => order.Id == id);
                gvTable.Rows[i].Cells[1].Text = foo == null ? (0).ToString() : (foo.Count).ToString();
            }
        }

        private void LoadOrdersToSession(List<Order> orders)
        {
            Session["CurrentOrder"] = orders;
        }

        private List<Order> GetOrdersFromSession()
        {
            return Session["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        private int GetIdFromRow(int index)
        {
            int id;
            if (!int.TryParse(gvTable.Rows[index].Cells[3].Text, out id)) return -1;

            var product = _productRepository.GetProductById(id);
            return product == null ? -1 : product.Id;
        }

        private void AddToOrders(List<Order> orders, int id)
        {
            var order = orders.Find(o => o.Id == id);
            if (order == null) orders.Add(new Order {Id = id, Count = 1});
            else order.Count++;

            _master.LabMessageForeColor = Color.DarkGreen;
            var text = LangSetter.Set("ProductCatalog_ProductAdded");
            if (text != null) _master.LabMessageText = string.Format(text, _productRepository.GetProductById(id).Name);
        }

        private void RemoveFromOrders(List<Order> orders, int id)
        {
            var order = orders.Find(o => o.Id == id);
            if (order == null || order.Count == 0) return;
            if (order.Count == 1) orders.Remove(order);
            else order.Count--;

            _master.LabMessageForeColor = Color.DarkBlue;
            var text = LangSetter.Set("ProductCatalog_ProductRemoved");
            if (text != null) _master.LabMessageText = string.Format(text, _productRepository.GetProductById(id).Name);
        }
    }
}