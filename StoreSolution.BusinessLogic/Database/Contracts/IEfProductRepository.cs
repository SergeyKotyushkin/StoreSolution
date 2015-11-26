using System.Linq;
using StoreSolution.BusinessLogic.Database.Model;

namespace StoreSolution.BusinessLogic.Database.Contracts
{
    public interface IEfProductRepository
    {
        IQueryable<Product> Products { get; }

        bool AddOrUpdateProduct(Product product);

        bool RemoveProduct(int id);

        Product GetProductById(int id);

        IQueryable<Product> SearchByName(IQueryable<Product> products, string searchName);

        IQueryable<Product> SearchByCategory(IQueryable<Product> products, string searchCategory);
    }
}