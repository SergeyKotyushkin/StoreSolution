using System.Data.Entity;
using StoreSolution.BusinessLogic.Database.Models;

namespace StoreSolution.BusinessLogic.Database.EfContexts
{
    public class EfProductContext : DbContext
    {
        public EfProductContext()
            : base("name=EfProductContext")
        {
        }

        public virtual DbSet<Product> ProductTable { get; set; }
    }
}
