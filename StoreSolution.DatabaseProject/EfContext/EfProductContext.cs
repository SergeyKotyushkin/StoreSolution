using System.Data.Entity;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.EfContext
{
    public class EfProductContext : DbContext
    {
        public EfProductContext()
            : base("name=EfProductContext")
        {
        }

        public DbSet<Product> ProductTable { get; set; }
    }
}
