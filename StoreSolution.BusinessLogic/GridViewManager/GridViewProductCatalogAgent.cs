using System.Linq;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Model;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewProductCatalogAgent : GridViewAgent<Product>, IGridViewProductCatalogManager
    {
        private readonly IEfProductRepository _efProductRepository;
        private readonly IOrderSessionRepository _orderSessionRepository;
        private readonly ILangSetter _langSetter;

        public GridViewProductCatalogAgent(IEfProductRepository efProductRepository,
            IOrderSessionRepository orderSessionRepository, ILangSetter langSetter,
            IGridViewPageIndexService gridViewPageIndexService, ICurrencyConverter currencyConverter)
            : base(gridViewPageIndexService, currencyConverter)
        {
            _efProductRepository = efProductRepository;
            _orderSessionRepository = orderSessionRepository;
            _langSetter = langSetter;
        }

        public void FillOrderColumn(GridView table, int columnIndex, int indexIdColumn, object repository)
        {
            var orders = _orderSessionRepository.GetAll((HttpSessionState)repository);

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var id = GetIdFromRow(table, i, indexIdColumn);
                var foo = orders.Find(order => order.Id == id);
                table.Rows[i].Cells[columnIndex].Text = (foo == null ? 0 : foo.Count).ToString();
            }
        }

        public void FillCategories(DropDownList ddl, IQueryable<Product> data)
        {
            var categories = data.Select(p => p.Category).Distinct().ToArray();

            var ddlSearchCategorySelectedIndex = ddl.SelectedIndex;

            ddl.Items.Clear();

            ddl.Items.Add(_langSetter.Set("ProductCatalog_AllCategories"));
            foreach (var category in categories)
                ddl.Items.Add(category);

            ddl.SelectedIndex = ddlSearchCategorySelectedIndex < ddl.Items.Count
                ? ddlSearchCategorySelectedIndex
                : ddl.Items.Count - 1;
        }

        public int GetIdFromRow(GridView table, int indexRow, int indexIdColumn)
        {
            int id;
            if (!int.TryParse(table.Rows[indexRow].Cells[indexIdColumn].Text, out id)) return -1;

            var product = _efProductRepository.GetProductById(id);
            return product == null ? -1 : product.Id;
        }
    }
}