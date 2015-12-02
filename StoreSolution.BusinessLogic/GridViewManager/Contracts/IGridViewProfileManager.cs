using System.Globalization;
using System.Linq;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewProfileManager<in T> : IGridViewManager<OrderToGrid, T>
    {
        IQueryable<OrderToGrid> GetOrderHistoriesList(string userName, CultureInfo culture);
    }
}