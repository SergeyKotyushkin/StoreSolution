using System.Collections.Generic;
using System.Web.SessionState;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;

namespace StoreSolution.BusinessLogic.OrderRepository
{
    public class OrderSessionRepository : IOrderSessionRepository
    {
        public void Add(object httpSessionState, int id)
        {
            var sessionState = (HttpSessionState) httpSessionState;
            var orders = GetAll(sessionState);

            var order = orders.Find(o => o.Id == id);
            if (order == null) orders.Add(new Order { Id = id, Count = 1 });
            else order.Count++;

            SetAll(sessionState, orders);
        }

        public void Remove(object httpSessionState, int id)
        {
            var sessionState = (HttpSessionState)httpSessionState;
            var orders = GetAll(sessionState);

            var order = orders.Find(o => o.Id == id);
            if (order == null || order.Count == 0) return;
            if (order.Count == 1) orders.Remove(order);
            else order.Count--;

            SetAll(sessionState, orders);
        }

        public List<Order> GetAll(object httpSessionState)
        {
            var sessionState = (HttpSessionState)httpSessionState;
            return sessionState["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        private static void SetAll(HttpSessionState sessionState, IEnumerable<Order> orders)
        {
            sessionState["CurrentOrder"] = orders;
        }
    }
}