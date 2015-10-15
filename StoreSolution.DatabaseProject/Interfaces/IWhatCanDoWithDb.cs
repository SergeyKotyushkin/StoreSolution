using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreSolution.DatabaseProject.Model;

namespace StoreSolution.DatabaseProject.Interfaces
{
    public interface IWhatCanDoWithDb
    {
        IList<Product> GetProducts();
        bool AddProduct(Product product);
        bool RemoveProduct(int id);
    }
}
