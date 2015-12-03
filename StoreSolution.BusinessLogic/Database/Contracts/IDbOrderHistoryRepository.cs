using System.Collections.Generic;
using System.Linq;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Database.Contracts
{
    public interface IDbOrderHistoryRepository
    {
        IQueryable<OrderHistory> GetAll();

        bool AddOrUpdate(OrderHistory orderHistory);

        bool AddOrUpdate(IEnumerable<OrderItem> orderItems, string userName, string userEmail, string cultureName);
    }
}
