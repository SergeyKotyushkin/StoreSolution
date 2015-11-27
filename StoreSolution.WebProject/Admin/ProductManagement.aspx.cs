using System;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.GridViewManager;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Log4net;
using StoreSolution.BusinessLogic.StructureMap;
using StoreSolution.BusinessLogic.UserGruop.Contracts;
using StoreSolution.WebProject.Master;

namespace StoreSolution.WebProject.Admin
{
    public partial class ProductManagement : Page
    {
        private const string PageIndexNameInRepository = "pageIndexNameProductManager";
        private const string CurrencyCultureName = "currencyCultureName";

        private const int ColumnId = 2;
        private const int PriceColumnsIndex = 5;

        private static readonly Color ErrorColor = Color.Red;
        private static readonly Color SuccessColor = Color.DarkGreen;
        
        private StoreMaster _master;
        private readonly IEfProductRepository _efProductRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IUserGroup _userGroup;
        private readonly ILangSetter _langSetter;
        private readonly ICurrencyCultureService<HttpCookieCollection> _currencyCultureService;
        private readonly IGridViewProductManagementManager<HttpSessionState> _gridViewProductManagementManager;

        protected ProductManagement()
            : this(
                StructureMapFactory.Resolve<IEfProductRepository>(), StructureMapFactory.Resolve<ICurrencyConverter>(),
                StructureMapFactory.Resolve<IUserGroup>(), StructureMapFactory.Resolve<ILangSetter>(),
                StructureMapFactory.Resolve<ICurrencyCultureService<HttpCookieCollection>>(),
                StructureMapFactory.Resolve<IGridViewProductManagementManager<HttpSessionState>>())
        {
        }

        protected ProductManagement(IEfProductRepository efProductRepository, ICurrencyConverter currencyConverter,
            IUserGroup userGroup, ILangSetter langSetter, ICurrencyCultureService<HttpCookieCollection> currencyCultureService,
            IGridViewProductManagementManager<HttpSessionState> gridViewProductManagementManager)
        {
            _efProductRepository = efProductRepository;
            _currencyConverter = currencyConverter;
            _userGroup = userGroup;
            _langSetter = langSetter;
            _currencyCultureService = currencyCultureService;
            _gridViewProductManagementManager = gridViewProductManagementManager;
        }

        protected override void InitializeCulture()
        {
            var cookie = Request.Cookies["language"];
            if (cookie == null)
            {
                cookie = new HttpCookie("language", "en-US");
                Response.Cookies.Add(cookie);
            }
            Page.Culture = Page.UICulture = cookie.Value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _master = (StoreMaster)Page.Master;
            if (_master == null) throw new HttpUnhandledException("Wrong master page.");

            var user = _userGroup.GetUser();

            SetTitles(user.UserName);
            
            SetUiProperties();

            if (!IsPostBack)
                FillGridView();
        }

        protected void gvTable_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            _master.SetLabMessage(Color.Empty, string.Empty);
            gvTable.PageIndex = e.NewPageIndex;

            _gridViewProductManagementManager.SavePageIndex(Session, PageIndexNameInRepository, e.NewPageIndex);

            FillGridView();
        }

        protected void gvTable_RowEditing(object sender, GridViewEditEventArgs e)
        {
            _master.SetLabMessage(Color.Empty, string.Empty);
            gvTable.EditIndex = e.NewEditIndex;

            SetSettingsRowEditing(false);
        }
        
        protected void gvTable_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            gvTable.PagerSettings.Visible = true;

            const bool isAdd = false;
            var currencyCulture = _currencyCultureService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName);
            var result = _gridViewProductManagementManager.AddOrUpdateProduct(gvTable.Rows[gvTable.EditIndex],
                currencyCulture, isAdd);

            ShowAddOrUpdateResult(result, isAdd, (string)e.NewValues["Name"]);
            
            SetSettingsRowEditing(true);
        }

        protected void gvTable_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            SetSettingsRowEditing(true);
        }

        protected void gvTable_PreRender(object sender, EventArgs e)
        {
            if (gvTable.EditIndex == -1) return;

            gvTable.Rows[gvTable.EditIndex].Cells[ColumnId].Enabled = false;
            SetPriceTextBoxCultureToCurrencyCulture();
        }
        
        protected void gvTable_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var id = int.Parse(gvTable.Rows[e.RowIndex].Cells[2].Text);

            var product = _efProductRepository.GetProductById(id);
            if (_efProductRepository.RemoveProduct(id))
            {
                _master.SetLabMessage(SuccessColor, "ProductManagement_ProductWasRemoved");
                Logger.Log.Info(string.Format("Product {0} successfully added.", product.Name));
            }
            else
                _master.SetLabMessage(ErrorColor, "ProductManagement_ProductWasNotRemoved");

            FillGridView();
        }

        protected void btnYes_Click(object sender, EventArgs e)
        {
            const bool isAdd = true;
            var currencyCulture = _currencyCultureService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName);
            var result = _gridViewProductManagementManager.AddOrUpdateProduct(gvTable.FooterRow, currencyCulture, isAdd);

            ShowAddOrUpdateResult(result, isAdd, ((TextBox)gvTable.FooterRow.Cells[3].Controls[0]).Text);

            SetSettingsRowEditing(true);

            SetVisibilityOfMessageBox(false);
        }
        
        protected void btnNo_Click(object sender, EventArgs e)
        {
            SetVisibilityOfMessageBox(false);
        }

        protected void gvTable_DataBound(object sender, EventArgs e)
        {
            var cultureTo = _currencyCultureService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName);

            _gridViewProductManagementManager.SetCultureForPriceColumns(gvTable, cultureTo, true, PriceColumnsIndex);
        }

        protected void gvTable_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            switch (e.Row.RowType)
            {
                case DataControlRowType.DataRow:
                    if (e.Row.RowIndex == 0 && e.Row.Cells[ColumnId].Text == (-1).ToString())
                        e.Row.Visible = false;
                    break;
                case DataControlRowType.Header:
                    e.Row.Cells[2].Text = _langSetter.Set("ProductManagement_HeaderId");
                    e.Row.Cells[3].Text = _langSetter.Set("ProductManagement_HeaderName");
                    e.Row.Cells[4].Text = _langSetter.Set("ProductManagement_HeaderCategory");
                    e.Row.Cells[5].Text = _langSetter.Set("ProductManagement_HeaderPrice");
                    break;
            }
        }

        protected void gvTable_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Footer)
                CreateFooter(e.Row);
        }


        private void FillGridView()
        {
            var data = _efProductRepository.Products;

            if (!data.Any())
                data =
                    new EnumerableQuery<Product>(new[] {new Product {Id = -1, Name = "0", Category = "0", Price = 0}});

            _gridViewProductManagementManager.FillGridViewAndRefreshPageIndex(gvTable, data, Session,
                PageIndexNameInRepository);
        }

        private void SetSettingsRowEditing(bool isEndEditing)
        {
            if (isEndEditing)
                gvTable.EditIndex = -1;

            gvTable.PagerSettings.Visible = isEndEditing;
            FillGridView();
            SetInsertEditDeleteButtonsEnabled(isEndEditing);
        }

        private void ShowAddOrUpdateResult(EditingResults result, bool isAdd, string productName)
        {
            switch (result)
            {
                case EditingResults.Success:
                    if (isAdd)
                        _gridViewProductManagementManager.SavePageIndex(Session, PageIndexNameInRepository,
                            gvTable.PageCount - 1);
                    _master.SetLabMessage(SuccessColor, "ProductManagement_ProductWasAdded");
                    Logger.Log.Info(string.Format("Product {0} successfully added.", productName));
                    break;
                case EditingResults.FailAddOrUpdate:
                case EditingResults.FailValidProduct:
                    _master.SetLabMessage(ErrorColor, "ProductManagement_ProductWasNotAdded");
                    break;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            _master.SetLabMessage(Color.Empty, string.Empty);
            SetVisibilityOfMessageBox(true);
        }

        private void CreateFooter(TableRow row)
        {
            var bInsert = new Button
            {
                Text = _langSetter.Set("ProductManagement_InsertButton"),
                CssClass = "btnInGvTable"
            };
            bInsert.Click += btnInsert_Click;

            row.Cells[1].Controls.Add(bInsert);
            for (var i = 3; i < 6; i++)
                row.Cells[i].Controls.Add(new TextBox());
        }

        private void SetPriceTextBoxCultureToCurrencyCulture()
        {
            var priceTextbox = (TextBox)gvTable.Rows[gvTable.EditIndex].Cells[PriceColumnsIndex].Controls[0];
            var price = decimal.Parse(priceTextbox.Text);
            var cultureTo = _currencyCultureService.GetCurrencyCultureInfo(Request.Cookies, CurrencyCultureName);
            var culturePrice = _currencyConverter.ConvertFromRubles(cultureTo, price, DateTime.Now);
            priceTextbox.Text = culturePrice.ToString("C", cultureTo);
        }

        private void SetInsertEditDeleteButtonsEnabled(bool enabled)
        {
            foreach (var row in gvTable.Rows.Cast<GridViewRow>().Where(row => row.RowIndex != gvTable.EditIndex))
                row.Cells[0].Enabled = row.Cells[1].Enabled = enabled;

            gvTable.FooterRow.Visible = enabled;
        }

        private void SetVisibilityOfMessageBox(bool visibility)
        {
            myDialogBox.Visible = visibility;
            gvTable.Enabled = !visibility;
        }

        private void SetTitles(string userName)
        {
            _master.HlUserText = userName;
        }

        private void SetUiProperties()
        {
            _master.BtnBackVisibility = false;
        }
    }
}