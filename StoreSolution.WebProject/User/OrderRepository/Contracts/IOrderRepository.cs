using System.Collections.Generic;
using System.Web.SessionState;
using StoreSolution.WebProject.Model;

namespace StoreSolution.WebProject.User.OrderRepository.Contracts
{
    public interface IOrderRepository
    {
        void Add(HttpSessionState sessionState, int id);

        void Remove(HttpSessionState sessionState, int id);

        List<Order> GetAll(HttpSessionState sessionState);
    }
}