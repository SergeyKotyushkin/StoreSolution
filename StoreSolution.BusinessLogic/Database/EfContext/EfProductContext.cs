using System.Data.Entity;
using StoreSolution.BusinessLogic.Database.Model;

namespace StoreSolution.BusinessLogic.Database.EfContext
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
