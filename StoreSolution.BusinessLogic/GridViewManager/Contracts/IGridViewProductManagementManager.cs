using System.Globalization;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewProductManagementManager<in T> : IGridViewManager<Product, T>
    {
        EditingResults AddOrUpdateProduct(TableRow row, CultureInfo currencyCulture, bool isAdd);
    }
}