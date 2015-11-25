using System;
using System.Collections.Generic;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Mail.Contracts
{
    public interface IMailSender
    {
        void Send();

        void Create(string @from, string to, string subject, IEnumerable<OrderItem> orderItemsBody, bool isBodyHtml,
            IFormatProvider culture);

        bool CheckIsMessageCreated();

        bool SuccessfulySend { get; }
    }
}