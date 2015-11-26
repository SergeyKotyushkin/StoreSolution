using System;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Model;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.User
{
    public partial class ProductCatalog : Page
    {
        private const string PageIndexNameInRepository = "pageIndexNameProductCatalog";
        private const string CurrencyCultureName = "currencyCultureName";
        private const int OrderColumnIndex = 1;
        private const int IndexIdColumn = 3;
        private static readonly int[] ColumnsIndexes = {6};

        private bool _isSearch;
        private StoreMaster _master;
        private readonly Color _productRemovedColor = Color.DarkBlue;
        private readonly Color _successColor = Color.DarkGreen;

        private readonly IEfProductRepository _efProductRepository;
        private readonly IOrderSessionRepository _orderSessionRepository;
        private readonly IUserGroup _userGroup;
        private readonly ILangSetter _langSetter;
        private readonly IGridViewProductCatalogManager _gridViewProductCatalogManager;
        private readonly ICurrencyCultureInfoService _currencyCultureInfoService;

        protected ProductCatalog()
            : this(
                StructureMapFactory.Resolve<IEfProductRepository>(),
                StructureMapFactory.Resolve<IOrderSessionRepository>(), StructureMapFactory.Resolve<IUserGroup>(),
                StructureMapFactory.Resolve<ILangSetter>(),
                StructureMapFactory.Resolve<IGridViewProductCatalogManager>(),
                StructureMapFactory.Resolve<ICurrencyCultureInfoService>())
        {
        }

        protected ProductCatalog(IEfProductRepository efProductRepository, IOrderSessionRepository orderSessionRepository,
            IUserGroup userGroup, ILangSetter langSetter, IGridViewProductCatalogManager gridViewProductCatalogManager,
            ICurrencyCultureInfoService currencyCultureInfoService)
        {
            _efProductRepository = efProductRepository;
            _orderSessionRepository = orderSessionRepository;
            _userGroup = userGroup;
            _langSetter = langSetter;
            _gridViewProductCatalogManager = gridViewProductCatalogManager;
            _currencyCultureInfoService = currencyCultureInfoService;
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
            var id = _gridViewProductCatalogManager.GetIdFromRow(gv, gv.SelectedIndex, IndexIdColumn);

            _orderSessionRepository.Add(Session, id);

            _master.SetLabMessage(_successColor, "ProductCatalog_ProductAdded",
                _efProductRepository.GetProductById(id).Name);

            _gridViewProductCatalogManager.FillOrderColumn(gv, OrderColumnIndex, IndexIdColumn, Session);
        }

        protected void btnBasket_Click(object sender, EventArgs e)
        {
            var user = _userGroup.GetUser();

            Logger.Log.Debug(string.Format("User {0} redirected to Basket page.", user.UserName));

            Response.Redirect(@"~/User/Basket.aspx");
        }
        
        protected void gvTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var gv = (GridView) sender;
            var id = _gridViewProductCatalogManager.GetIdFromRow(gv, e.RowIndex, IndexIdColumn);

            _orderSessionRepository.Remove(Session, id);

            _master.SetLabMessage(_productRemovedColor, "ProductCatalog_ProductRemoved",
                _efProductRepository.GetProductById(id).Name);

            _gridViewProductCatalogManager.FillOrderColumn(gv, OrderColumnIndex, IndexIdColumn, Session);
        }

        protected void gvTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            var gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex;

            _gridViewProductCatalogManager.SavePageIndex(Session, PageIndexNameInRepository, e.NewPageIndex);

            FillGridView();
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            var gv = (GridView)sender;
            _gridViewProductCatalogManager.SetCultureForPriceColumns(gv,
                _currencyCultureInfoService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName), ColumnsIndexes);

            _gridViewProductCatalogManager.FillOrderColumn(gvTable, OrderColumnIndex, IndexIdColumn, Session);
        }

        protected void gvTable_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[IndexIdColumn].CssClass = "hiddenСolumn";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[IndexIdColumn].CssClass = "hiddenСolumn";
                e.Row.Cells[1].Text = _langSetter.Set("ProductCatalog_HeaderCount");
                e.Row.Cells[4].Text = _langSetter.Set("ProductCatalog_HeaderName");
                e.Row.Cells[5].Text = _langSetter.Set("ProductCatalog_HeaderCategory");
                e.Row.Cells[6].Text = _langSetter.Set("ProductCatalog_HeaderPrice");
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            FillGridView();
        }

        protected void cbSearchHeader_CheckedChanged(object sender, EventArgs e)
        {
            var cb = (CheckBox) sender;
            _isSearch = cb.Checked;

            if (cb.Checked) return;

            ClearSearchValues();
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

            _master.HlUserText = userName;

            if (Session["Bought"] == null) return;

            _master.SetLabMessage(_successColor, "ProductCatalog_ProductsBought");
            Session["Bought"] = null;
        }

        private IQueryable<Product> SearchProducts(IQueryable<Product> products, string searchName, int indexCategory)
        {
            if (!string.IsNullOrWhiteSpace(searchName))
                products = _efProductRepository.SearchByName(products, searchName);

            if (indexCategory == 0)
                return products;

            var searchCategory = ddlSearchCategory.Items[ddlSearchCategory.SelectedIndex].Text;
            return _efProductRepository.SearchByCategory(products, searchCategory);
        }

        private void FillGridView()
        {
            _gridViewProductCatalogManager.RefreshPageIndex(Session, ViewStateUserKey, gvTable);

            var products = _efProductRepository.Products;

            if (_isSearch)
                products = SearchProducts(products, tbSearchName.Text.Trim(), ddlSearchCategory.SelectedIndex);

            _gridViewProductCatalogManager.Fill(gvTable, products);

            _gridViewProductCatalogManager.FillCategories(ddlSearchCategory, products);
        }

        private void ClearSearchValues()
        {
            ddlSearchCategory.SelectedIndex = 0;
            tbSearchName.Text = string.Empty;
        }
    }
}