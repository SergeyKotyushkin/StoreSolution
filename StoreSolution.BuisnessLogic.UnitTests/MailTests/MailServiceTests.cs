using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Mail;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BuisnessLogic.UnitTests.MailTests
{
    [TestClass]
    public class MailServiceTests
    {
        [TestMethod]
        public void GetBody_GiveItemsListAndCulture_ReturnExpectedBodyAsString()
        {
            var orderItemsList = new List<OrderItem>
            {
                new OrderItem {Name = "Car", Count = 1, Price = 500m, Total = 500m},
                new OrderItem {Name = "Camera", Count = 3, Price = 70m, Total = 210m},
            };

            var mailService = GetMailService();

            var expected = GetExpectedBody();

            var actual = mailService.GetBody(orderItemsList, new CultureInfo("ru-RU"));

            Assert.AreEqual(expected, actual);
        }

        private static MailService GetMailService()
        {
            var langSetterMock = new Mock<ILangSetter>();
            const string mailOrderList = @"<li><b>{0}</b> [количество: {1}; цена: {2}]</li>";
            const string mailMessage = "<p>Спасибо за совершенные покупки в нашем <b>\"Online Store\"</b>." +
                                       "<br/>Сегодня ({0}) Вы приобрели:<br/>{1}Итого: <b>{2}</b>.<br/>" +
                                       "Надеемся, что Вам понравился наш магазин. Ждём Вас снова!</p>";

            langSetterMock.Setup(m => m.Set(It.Is<string>(n => n == "Basket_MailOrderList"))).Returns(mailOrderList);
            langSetterMock.Setup(m => m.Set(It.Is<string>(n => n == "Basket_MailMessage"))).Returns(mailMessage);

            return new MailService(langSetterMock.Object);
        }

        private static string GetExpectedBody()
        {
            return
                "<p>Спасибо за совершенные покупки в нашем <b>\"Online Store\"</b>." +
                "<br/>Сегодня (" + DateTime.Now.ToShortDateString() + ") Вы приобрели:<br/>" +
                "<ul>" +
                "<li><b>Car</b> [количество: 1; цена: 500,00 ₽]</li>" +
                "<li><b>Camera</b> [количество: 3; цена: 70,00 ₽]</li>" +
                "</ul>" +
                "Итого: <b>710,00 ₽</b>." +
                "<br/>Надеемся, что Вам понравился наш магазин. Ждём Вас снова!</p>";
        }
    }
}
