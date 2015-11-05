using System.Data.Entity.Migrations;
using System.Linq;
using StoreSolution.DatabaseProject.Contracts;
using StoreSolution.DatabaseProject.EfContext;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Realizations
{
    public class EfProductRepository : IProductRepository
    {
        private readonly EfProductContext _context = new EfProductContext();

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