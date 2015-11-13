using System.Data.Entity.Migrations;
using System.Linq;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.EfContext;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Realizations
{
    public class EfOrderHistoryRepository : IOrderHistoryRepository
    {
        private readonly EfPersonContext _context = new EfPersonContext();

        public IQueryable<OrderHistory> OrdersHistory
        {
            get { return _context.OrdersHistoryTable; }
        }

        public bool AddOrUpdate(OrderHistory orderHistory)
        {
            _context.OrdersHistoryTable.AddOrUpdate(orderHistory);
            return _context.SaveChanges() > 0;
        }
    }
}