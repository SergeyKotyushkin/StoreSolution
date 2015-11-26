﻿using System;

namespace StoreSolution.BusinessLogic.Models
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