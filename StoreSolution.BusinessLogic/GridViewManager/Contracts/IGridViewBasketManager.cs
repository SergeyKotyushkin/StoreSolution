using System.Globalization;
using System.Linq;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewBasketManager<in T> : IGridViewManager<OrderItem, T>
    {
        IQueryable<OrderItem> GetOrderItemsList(T repository, CultureInfo culture);
    }
}