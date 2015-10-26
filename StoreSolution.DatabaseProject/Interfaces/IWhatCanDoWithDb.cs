using System.Collections.Generic;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Interfaces
{
    public interface IWhatCanDoWithDb
    {
        IList<Product> GetProducts();
        bool AddProduct(Product product);
        bool RemoveProduct(int id);
        bool UpdateProduct(Product product);
        Product GetProductById(int id);
        int GetCurrentId();
    }
}
