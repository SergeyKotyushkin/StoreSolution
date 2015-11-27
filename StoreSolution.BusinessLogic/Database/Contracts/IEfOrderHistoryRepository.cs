using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Security;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Database.Contracts
{
    public interface IEfOrderHistoryRepository
    {
        IQueryable<OrderHistory> GetAll { get; }

        bool Add(OrderHistory orderHistory);

        bool Add(IEnumerable<OrderItem> orderItems, MembershipUser user, CultureInfo culture);
    }
}
