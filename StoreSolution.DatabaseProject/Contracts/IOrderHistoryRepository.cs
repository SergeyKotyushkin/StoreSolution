using System.Linq;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Contracts
{
    public interface IOrderHistoryRepository
    {
        IQueryable<OrderHistory> OrdersHistory { get; }

        bool AddOrUpdate(OrderHistory orderHistory);
    }
}
