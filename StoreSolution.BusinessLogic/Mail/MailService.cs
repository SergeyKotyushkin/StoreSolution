using System;
using System.Collections.Generic;
using System.Linq;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Mail.Contracts;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Mail
{
    public class MailService : IMailService
    {
        private readonly ILangSetter _langSetter;

        public MailService(ILangSetter langSetter)
        {
            _langSetter = langSetter;
        }

        public string GetBody(IEnumerable<OrderItem> orderItemsBody, IFormatProvider culture)
        {
            var orderItems = orderItemsBody.ToArray();

            var orderList = string.Format("{0}</ul>", orderItems.Aggregate("<ul>",
                (current, p) =>
                    current +
                    string.Format(
                        _langSetter.Set("Basket_MailOrderList"),
                        p.Name,
                        p.Count,
                        p.Price.ToString("C", culture))));

            var total = orderItems.Sum(p => p.Total);

            var mailMessageBody =
                string.Format(
                    _langSetter.Set("Basket_MailMessage"),
                    DateTime.Now.Date.ToShortDateString(),
                    orderList,
                    total.ToString("C", culture));

            return mailMessageBody;
        }
    }
}