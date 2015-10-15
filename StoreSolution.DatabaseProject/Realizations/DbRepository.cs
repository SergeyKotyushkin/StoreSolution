using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.DatabaseProject.Interfaces;

namespace StoreSolution.DatabaseProject.Realizations
{
    public class DbRepository : IWhatCanDoWithDb
    {
        private List<Product> products = new List<Product>() {
            new Product() { Id=0, Name="Salt", Category="Food", Price=10 },
            new Product() { Id=1, Name="Ball", Category="Sport", Price=950 },
            new Product() { Id=2, Name="Coffee", Category="Food", Price=90 },
            new Product() { Id=3, Name="Button", Category="Wear", Price=3 }
        };

        private static DbRepository instance;

        public DbRepository()
        {
 
        }

        public static DbRepository GetInstance()
        {
            if (instance == null) instance = new DbRepository();
            return instance;
        }

        public IList<Product> GetProducts()
        {
            return products;
        }

        public bool AddProduct(Product product)
        {
            products.Add(product);
            return true;
        }

        public bool RemoveProduct(int id)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Id == id) 
                { 
                    products.RemoveAt(i);
                    return true; 
                }
            }

            return false;
        }

        public Product GetProductById(int id)
        {
            foreach (var item in products)
            {
                if (item.Id == id) return item;
            }

            return null;
        }
    }
}