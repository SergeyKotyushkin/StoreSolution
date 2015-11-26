using System.Collections.Generic;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.OrderRepository.Contracts
{
    public interface IOrderSessionRepository
    {
        void Add(object httpSessionState, int id);

        void Remove(object httpSessionState, int id);

        List<Order> GetAll(object httpSessionState);
    }
}