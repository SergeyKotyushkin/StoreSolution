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
            get { return _context.ProductTable; }
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

        //private readonly List<Product> _products = new List<Product>() 
        //{
        //    new Product() { Id=0, Name="Salt", Category="Food", Price=10 },
        //    new Product() { Id=1, Name="Ball", Category="Sport", Price=950 },
        //    new Product() { Id=2, Name="Coffee", Category="Food", Price=90 },
        //    new Product() { Id=3, Name="Button", Category="Wear", Price=3 }
        //};

        //private static EfProductRepository _instance;
        //private DbRepository()
        //{
        //}
        //public static DbRepository GetInstance()
        //{
        //    return _instance ?? (_instance = new DbRepository());
        //}

        //public IList<Product> GetProducts()
        //{
        //    return _products.Select(m => m).OrderBy(m => m.Id).ToList();
        //}

        //public bool AddProduct(Product product)
        //{
        //    if (_products.Find(p => p.Name == product.Name && p.Category == product.Category && p.Price == product.Price) != null)
        //        return false;
        //    if (_products.Count == 0) product.Id = 0;
        //    else product.Id = _products.Max(p => p.Id) + 1;

        //    _products.Add(product);
        //    return true;
        //}

        //public bool RemoveProduct(int id)
        //{
        //    for (var i = 0; i < _products.Count; i++)
        //    {
        //        if (_products[i].Id == id) 
        //        { 
        //            _products.RemoveAt(i);
        //            return true; 
        //        }
        //    }

        //    return false;
        //}

        //public bool UpdateProduct(Product product)
        //{
        //    foreach (var t in _products.Where(t => t.Id == product.Id))
        //    {
        //        t.Name = product.Name;
        //        t.Category = product.Category;
        //        t.Price = product.Price;
        //        return true;
        //    }

        //    return false;
        //}


        //public Product GetProductById(int id)
        //{
        //    return _products.FirstOrDefault(item => item.Id == id);
        ////}

        //public int GetCurrentId()
        //{
        //    var list = _products.Select(m => m.Id).OrderBy(m => m);
        //    var id = -1;
        //    if (list.Any(item => !list.Contains(++id)))
        //    {
        //        return id;
        //    }

        //    return id + 1;
        //}
    }
}