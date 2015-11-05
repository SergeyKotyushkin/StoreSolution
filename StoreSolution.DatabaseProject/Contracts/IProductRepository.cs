using System.Linq;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Contracts
{
    public interface IProductRepository
    {
        IQueryable<Product> Products { get; }
        bool AddOrUpdateProduct(Product product);
        bool RemoveProduct(int id);
        Product GetProductById(int id);
    }
}
