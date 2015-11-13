using System;

namespace StoreSolution.WebProject.Model
{
    public class OrderFromHistory
    {
        public int Number;
        public DateTime Date;
        public string Email;
        public ProductsOrder[] ProductsOrder;
        public decimal Total;
        public string CultureName;
    }
}