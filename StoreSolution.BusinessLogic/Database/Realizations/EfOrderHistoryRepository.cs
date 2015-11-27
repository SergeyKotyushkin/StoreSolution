using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.Security;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.EfContexts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Database.Realizations
{
    public class EfOrderHistoryRepository : IEfOrderHistoryRepository
    {
        private readonly EfPersonContext _context = new EfPersonContext();

        public IQueryable<OrderHistory> GetAll
        {
            get { return _context.OrdersHistoryTable; }
        }

        public bool Add(OrderHistory orderHistory)
        {
            _context.OrdersHistoryTable.AddOrUpdate(orderHistory);
            return _context.SaveChanges() > 0;
        }

        public bool Add(IEnumerable<OrderItem> orderItems, MembershipUser user, CultureInfo culture)
        {
            var orderHistory = CreateOrderHistory(orderItems, user, culture);

            return Add(orderHistory);
        }

        private static OrderHistory CreateOrderHistory(IEnumerable<OrderItem> orderItems, MembershipUser user, CultureInfo culture)
        {
            var jsonSerialiser = new JavaScriptSerializer();
            var order = jsonSerialiser.Serialize(orderItems);

            return new OrderHistory
            {
                Order = order,
                PersonName = user.UserName,
                PersonEmail = user.Email,
                Total = orderItems.Sum(p => p.Total),
                Date = DateTime.Now,
                Culture = culture.Name
            };
        }
    }
}