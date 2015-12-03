using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.GridViewManager;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.JsonSerialize.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BuisnessLogic.UnitTests.GridViewManagerTests
{
    [TestClass]
    public class GridViewProfileAgentTests
    {
        #region arranged data

        private static readonly OrderHistory[] OrderHistories =
        {
            new OrderHistory
            {
                Id = 1,PersonName = "User1",
                PersonEmail = "User1@mail.com",
                Order = "some order1",
                Total = 230m,
                Culture = "en-US",
                Date = DateTime.Now
            },
            new OrderHistory
            {
                Id = 1,
                PersonName = "User2",
                PersonEmail = "User2@mail.com",
                Order = "some order2",
                Total = 45m,
                Culture = "ru-RU",
                Date = new DateTime(2015, 1, 20)
            },
            new OrderHistory
            {
                Id = 1,
                PersonName = "User2",
                PersonEmail = "User2@mail.com",
                Order = "some order3",
                Total = 102m,
                Culture = "en-GB",
                Date = new DateTime(2015, 2, 20)
            }
        };

        private static readonly ProductOrder[] ProductOrdersFirst =
        {
            new ProductOrder {Name = "Lemon", Count = 3, Price = 15m, Total = 45m}
        };

        private static readonly ProductOrder[] ProductOrdersSecond =
        {
            new ProductOrder {Name = "Orange", Count = 2, Price = 16m, Total = 32m},
            new ProductOrder {Name = "Banana", Count = 7, Price = 10m, Total = 70m}
        };

        #endregion

        [TestMethod]
        public void GetOrderToGridList_CheckReturnData_ReturnedDataSameAsexpected()
        {
            const string userName = "User2";

            var agent = ArrangeGridViewProfileAgent();
            
            var expected = GetExpectedOrderHistories().ToArray();
            
            var actual = agent.GetOrderToGridList(userName).ToArray();

            Assert.AreEqual(expected.Length, actual.Length);
            for (var i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i][0], actual[i].Number);
                Assert.AreEqual(expected[i][1], actual[i].Email);
                Assert.AreEqual(expected[i][2], actual[i].Order);
                Assert.AreEqual(expected[i][3], actual[i].Total);
                Assert.AreEqual(expected[i][4], actual[i].Date);
            }
        }


        private static GridViewProfileAgent<object> ArrangeGridViewProfileAgent()
        {
            var mockStorageService = new Mock<IStorageService<object>>();

            var mockCurrencyConverter = new Mock<ICurrencyConverter>();

            var mockOrderHistoryRepository = new Mock<IDbOrderHistoryRepository>();
            mockOrderHistoryRepository.Setup(m => m.GetAll()).Returns(OrderHistories.AsQueryable());

            var  mockLangSetter = new Mock<ILangSetter>();
            mockLangSetter.Setup(m => m.Set(It.IsAny<string>())).Returns((string s) => s.Substring(s.IndexOf('_') + 1));

            var mockJsonSerializer = new Mock<IJsonSerializer>();
            mockJsonSerializer.Setup(m => m.Deserialize<ProductOrder[]>(It.Is<string>(s => s == "some order2")))
                .Returns(ProductOrdersFirst);
            mockJsonSerializer.Setup(m => m.Deserialize<ProductOrder[]>(It.Is<string>(s => s == "some order3")))
                .Returns(ProductOrdersSecond);

            return new GridViewProfileAgent<object>(mockStorageService.Object, mockCurrencyConverter.Object,
                mockOrderHistoryRepository.Object, mockLangSetter.Object, mockJsonSerializer.Object);
        }

        private static IEnumerable<ArrayList> GetExpectedOrderHistories()
        {
            const string firstOrder = "<b>Lemon</b> (Quantity <b>3</b>: Price <b>15,00 ₽</b>) Total <b>45,00 ₽</b>";
            const string secondOrder = "<b>Orange</b> (Quantity <b>2</b>: Price <b>£16.00</b>) Total <b>£32.00</b>\r\n" +
                                       "<b>Banana</b> (Quantity <b>7</b>: Price <b>£10.00</b>) Total <b>£70.00</b>";

            return new []
            {
                new ArrayList {1, "User2@mail.com", firstOrder, "<b>45,00 ₽</b>", new DateTime(2015, 1, 20)},
                new ArrayList {2, "User2@mail.com", secondOrder, "<b>£102.00</b>", new DateTime(2015, 2, 20)}
            };
        }
    }
}
