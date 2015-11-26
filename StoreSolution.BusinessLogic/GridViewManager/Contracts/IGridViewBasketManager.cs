using System.Globalization;
using System.Linq;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewBasketManager : IGridViewManager<OrderItem>
    {
        IQueryable<OrderItem> GetOrderItemsList(object repository, CultureInfo culture);
    }
}