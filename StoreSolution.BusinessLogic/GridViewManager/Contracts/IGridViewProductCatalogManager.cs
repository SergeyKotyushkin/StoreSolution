using System.Linq;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewProductCatalogManager<in T> : IGridViewManager<Product, T>
    {
        void FillOrderColumn(GridView table, int columnIndex, int indexIdColumn, T sessionState);

        void FillCategories(DropDownList ddl, IQueryable<Product> data);

        int GetIdFromRow(GridView table, int indexRow, int indexIdColumn);
    }
}