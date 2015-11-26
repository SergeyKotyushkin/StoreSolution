using System.Linq;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Database.Model;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewProductCatalogManager : IGridViewManager<Product>
    {
        void FillOrderColumn(GridView table, int columnIndex, int indexIdColumn, object repository);

        void FillCategories(DropDownList ddl, IQueryable<Product> data);

        int GetIdFromRow(GridView table, int indexRow, int indexIdColumn);
    }
}