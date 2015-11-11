using System.Data.Entity;
using System.Linq;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.EfContext
{
    public class EfPersonContext: DbContext
    {
        public EfPersonContext()
            : base("name=EfPersonContext")
        {
        }

        public DbSet<Person> PersonTable { get; set; }
        public DbSet<OrderHistory> OrdersHistoryTable { get; set; }
    }
}
