using System.Collections.Generic;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.OrderRepository.Contracts
{
    public interface IOrderRepository<in T>
    {
        void Add(T repository, int id);

        void Remove(T repository, int id);

        List<Order> GetAll(T repository);
    }
}