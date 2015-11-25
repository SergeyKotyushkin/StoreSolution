using System.Data.Entity.Migrations;
using System.Linq;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.EfContext;
using StoreSolution.BusinessLogic.Database.Model;

namespace StoreSolution.BusinessLogic.Database.Realizations
{
    public class EfProductRepository : IEfProductRepository
    {
        private readonly EfProductContext _context;

        public EfProductRepository(EfProductContext context)
        {
            _context = context;
        }

        public IQueryable<Product> Products
        {
            get { return _context.ProductTable.OrderBy(p => p.Id); }
        }

        public bool AddOrUpdateProduct(Product product)
        {
            _context.ProductTable.AddOrUpdate(product);
            return _context.SaveChanges() > 0;
        }

        public bool RemoveProduct(int id)
        {
            var entity = _context.ProductTable.Find(id);
            if (entity == null) return false;
            _context.ProductTable.Remove(entity);
            _context.SaveChanges();
            return true;
        }

        public Product GetProductById(int id)
        {
            return _context.ProductTable.Find(id);
        }

    }
}