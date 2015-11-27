using System.Collections.Generic;
using System.Web.SessionState;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;

namespace StoreSolution.BusinessLogic.OrderRepository
{
    public class OrderSessionRepository : IOrderRepository<HttpSessionState>
    {
        public void Add(HttpSessionState repository, int id)
        {
            var orders = GetAll(repository);

            var order = orders.Find(o => o.Id == id);
            if (order == null) orders.Add(new Order { Id = id, Count = 1 });
            else order.Count++;

            SetAll(repository, orders);
        }

        public void Remove(HttpSessionState repository, int id)
        {
            var orders = GetAll(repository);

            var order = orders.Find(o => o.Id == id);
            if (order == null || order.Count == 0) return;
            if (order.Count == 1) orders.Remove(order);
            else order.Count--;

            SetAll(repository, orders);
        }

        public List<Order> GetAll(HttpSessionState repository)
        {
            return repository["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        private static void SetAll(HttpSessionState sessionState, IEnumerable<Order> orders)
        {
            sessionState["CurrentOrder"] = orders;
        }
    }
}