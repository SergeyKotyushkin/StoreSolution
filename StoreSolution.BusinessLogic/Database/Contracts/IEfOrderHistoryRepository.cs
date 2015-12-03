using System.Collections.Generic;
using System.Linq;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Database.Contracts
{
    public interface IEfOrderHistoryRepository
    {
        IQueryable<OrderHistory> GetAll { get; }

        bool Add(OrderHistory orderHistory);

        bool Add(IEnumerable<OrderItem> orderItems, string userName, string userEmail, string cultureName);
    }
}
