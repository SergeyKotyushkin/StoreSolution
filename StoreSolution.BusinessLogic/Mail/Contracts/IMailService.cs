using System;
using System.Collections.Generic;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.Mail.Contracts
{
    public interface IMailService
    {
        string GetBody(IEnumerable<OrderItem> orderItemsBody, IFormatProvider culture);
    }
}