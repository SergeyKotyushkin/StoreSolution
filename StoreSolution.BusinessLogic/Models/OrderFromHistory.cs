using System;

namespace StoreSolution.BusinessLogic.Models
{
    public class OrderFromHistory
    {
        public int Number;
        public DateTime Date;
        public string Email;
        public ProductOrder[] ProductOrder;
        public decimal Total;
        public string CultureName;
    }
}