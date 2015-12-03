using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.EfContexts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.JsonSerialize.Contracts;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Database.Realizations
{
    public class EfOrderHistoryRepository : IDbOrderHistoryRepository
    {
        private readonly IJsonSerializer _jsonSerializer;

        private readonly EfPersonContext _context = new EfPersonContext();

        public EfOrderHistoryRepository(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        public IQueryable<OrderHistory> GetAll()
        {
            return _context.OrdersHistoryTable;
        }

        public bool AddOrUpdate(OrderHistory orderHistory)
        {
            _context.OrdersHistoryTable.AddOrUpdate(orderHistory);
            return _context.SaveChanges() > 0;
        }

        public bool AddOrUpdate(IEnumerable<OrderItem> orderItems, string userName, string userEmail, string cultureName)
        {
            var orderHistory = CreateOrderHistory(orderItems, userName, userEmail, cultureName);

            return AddOrUpdate(orderHistory);
        }

        private OrderHistory CreateOrderHistory(IEnumerable<OrderItem> orderItems, string userName, string userEmail,
            string cultureName)
        {
            var order = _jsonSerializer.Serialize(orderItems);

            return new OrderHistory
            {
                Order = order,
                PersonName = userName,
                PersonEmail = userEmail,
                Total = orderItems.Sum(p => p.Total),
                Date = DateTime.Now,
                Culture = cultureName
            };
        }
    }
}