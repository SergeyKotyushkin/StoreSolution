using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.WebProject.Currency.Contracts;
using StoreSolution.WebProject.Lang;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.Master;
using StoreSolution.WebProject.StructureMap;
using StoreSolution.WebProject.User.OrderRepository.Contracts;
using StoreSolution.WebProject.UserGruop.Contracts;

namespace StoreSolution.WebProject.User
{
    public partial class ProductCatalog : Page
    {
        private bool _isSearch;
        private StoreMaster _master;
        private readonly Color _productRemovedColor = Color.DarkBlue;
        private readonly Color _successColor = Color.DarkGreen;

        private readonly IProductRepository _productRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserGroup _userGroup;

        protected ProductCatalog()
            : this(
                StructureMapFactory.Resolve<IProductRepository>(), StructureMapFactory.Resolve<ICurrencyConverter>(),
                StructureMapFactory.Resolve<IOrderRepository>(), StructureMapFactory.Resolve<IUserGroup>())
        {
        }

        protected ProductCatalog(IProductRepository productRepository, ICurrencyConverter currencyConverter,
            IOrderRepository orderRepository, IUserGroup userGroup)
        {
            _productRepository = productRepository;
            _currencyConverter = currencyConverter;
            _orderRepository = orderRepository;
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
            Page.Culture = cookie.Value;
            Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            var user = _userGroup.GetUser();
            
            SetUiProperties(user.UserName);

            if (!Page.IsPostBack)
                FillGridView();
        }

        protected void gvTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            var gv = (GridView) sender;
            var id = GetIdFromRow(gv.SelectedIndex);

            _orderRepository.Add(Page.Session, id);

            _master.SetLabMessage(_successColor, "ProductCatalog_ProductAdded",
                _productRepository.GetProductById(id).Name);

            FillCountColumn();
        }

        protected void btnBasket_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser();

            Logger.Log.Debug(string.Format("User {0} redirected to Basket page.", user.UserName));
            Response.Redirect("~/User/Basket.aspx");
        }
        
        protected void gvTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = GetIdFromRow(e.RowIndex);

            _orderRepository.Remove(Page.Session, id);

            _master.SetLabMessage(_productRemovedColor, "ProductCatalog_ProductRemoved",
                _productRepository.GetProductById(id).Name);

            FillCountColumn();
        }

        protected void gvTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex;
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
            FillGridView();
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
            FillGridView();
        }

        protected void cbSearchHeader_CheckedChanged(object sender, EventArgs e)
        {
            _isSearch = cbSearchHeader.Checked;

            if(cbSearchHeader.Checked) return;

            ddlSearchCategory.SelectedIndex = 0;
            tbSearchName.Text = string.Empty;
            FillGridView();
        }

        protected void ddlSearchCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillGridView();
        }


        private void SetUiProperties(string userName)
        {
            pSearchingBoard.Visible = _isSearch = cbSearchHeader.Checked;

            _master.BtnBackVisibility = false;

            _master.HlUserText = string.Format(LangSetter.Set("Master_ToProfile"), userName);

            if (Session["Bought"] == null) return;

            _master.SetLabMessage(_successColor, "ProductCatalog_ProductsBought");

            Session["Bought"] = null;
        }

        private IQueryable<Product> SearchProducts(IQueryable<Product> products, string searchName, int searchCategory)
        {
            if (!string.IsNullOrWhiteSpace(searchName))
                products = products.Where(p => p.Name.ToLower().Contains(tbSearchName.Text.Trim().ToLower())).Select(p => p);

            if (searchCategory == 0) return products;

            var category = ddlSearchCategory.Items[ddlSearchCategory.SelectedIndex].Text;
            products = products.Where(p => p.Category == category).Select(p => p);

            return products;
        }

        private void FillGridView()
        {
            var products = _productRepository.Products;

            if (_isSearch)
                products = SearchProducts(products, tbSearchName.Text, ddlSearchCategory.SelectedIndex);

            gvTable.DataSource = products.ToList();
            gvTable.DataBind();

            FillCategories(products);
            FillCountColumn();
        }

        private void FillCategories(IQueryable<Product> products)
        {
            var categories = products.Select(p => p.Category).Distinct().ToArray();

            var ddlSearchCategorySelectedIndex = ddlSearchCategory.SelectedIndex;
            ddlSearchCategory.Items.Clear();
            ddlSearchCategory.Items.Add(LangSetter.Set("ProductCatalog_AllCategories"));
            foreach (var category in categories) 
                ddlSearchCategory.Items.Add(category);

            ddlSearchCategory.SelectedIndex = ddlSearchCategorySelectedIndex < ddlSearchCategory.Items.Count
                ? ddlSearchCategorySelectedIndex
                : ddlSearchCategory.Items.Count - 1;
        }

        private void FillCountColumn()
        {
            var orders = _orderRepository.GetAll(Page.Session);

            for (var i = 0; i < gvTable.Rows.Count; i++)
            {
                var id = GetIdFromRow(i);
                var foo = orders.Find(order => order.Id == id);
                gvTable.Rows[i].Cells[1].Text = (foo == null ? 0 : foo.Count).ToString();
            }
        }

        private int GetIdFromRow(int index)
        {
            int id;
            if (!int.TryParse(gvTable.Rows[index].Cells[3].Text, out id)) return -1;

            var product = _productRepository.GetProductById(id);
            return product == null ? -1 : product.Id;
        }   
    }
}