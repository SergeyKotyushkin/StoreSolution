﻿using System;
using System.Globalization;
using System.Linq;
using StoreSolution.BusinessLogic.Lang.Contracts;

namespace StoreSolution.BusinessLogic.Models
{
    public class OrderToGrid
    {
        public OrderToGrid(OrderFromHistory source, ILangSetter langSetter)
        {
            Number = source.Number;
            Date = source.Date;
            Email = source.Email;
            var culture =
                CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => c.Name == source.CultureName);

            if (culture == null)
            {
                Order = langSetter.Set("OrderToGrid_UnknownCurrency");
                return;
            }

            var quantity = langSetter.Set("Profile_Quantity");
            var price = langSetter.Set("Profile_Price");
            var total = langSetter.Set("Profile_Total");
            var order = source.ProductOrder.Aggregate(string.Empty,
                (current, p) =>
                    current +
                    string.Format("<b>{0}</b> ({1} <b>{2}</b>: {3} <b>{4}</b>)" + " {5} <b>{6}</b>{7}", p.Name, quantity, p.Count, price,
                        p.Price.ToString("C", culture), total, p.Total.ToString("C", culture), Environment.NewLine));

            Order = order.Substring(0, order.Length - Environment.NewLine.Length);
            Total = string.Format("<b>{0}</b>", source.Total.ToString("C", culture));
        }

        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public string Order { get; set; }
        public string Total { get; set; }
    }
}