using System.Data.Entity;
using StoreSolution.BusinessLogic.Database.Model;

namespace StoreSolution.BusinessLogic.Database.EfContext
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
