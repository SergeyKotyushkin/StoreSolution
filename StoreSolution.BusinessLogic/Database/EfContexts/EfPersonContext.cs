using System.Data.Entity;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.Database.EfContexts
{
    public class EfPersonContext : DbContext
    {
        public EfPersonContext()
            : base("name=EfPersonContext")
        {
        }

        public DbSet<Person> PersonTable { get; set; }
        public DbSet<OrderHistory> OrdersHistoryTable { get; set; }
    }
}
