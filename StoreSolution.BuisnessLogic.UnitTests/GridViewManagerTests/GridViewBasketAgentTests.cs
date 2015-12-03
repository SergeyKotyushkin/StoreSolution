using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.GridViewManager;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;

namespace StoreSolution.BuisnessLogic.UnitTests.GridViewManagerTests
{
    [TestClass]
    public class GridViewBasketAgentTests
    {
        private static readonly List<Order> OrderList = new List<Order>
        {
            new Order {Id = 1, Count = 3},
            new Order {Id = 7, Count = 1},
            new Order {Id = 5, Count = 2}
        };

        private static readonly List<Product> Products = new List<Product>
        {
            new Product {Id = 7, Name = "Carpet", Category = "Home", Price = 150m},
            new Product {Id = 5, Name = "TV", Category = "Home", Price = 17000m},
            new Product {Id = 64, Name = "Mirror", Category = "Home", Price = 200m},
            new Product {Id = 1, Name = "Flower", Category = "Garden", Price = 75m}
        };

        private static readonly OrderItem[] OrderItemsResultWithoutCurrencyConverting =
        {
            new OrderItem {Count = 1, Name = "Carpet", Price = 150m, Total = 150m},
            new OrderItem {Count = 2, Name = "TV", Price = 17000m, Total = 34000m},
            new OrderItem {Count = 3, Name = "Flower", Price = 75m, Total = 225m}
        };
        
        [TestMethod]
        public void GetOrderItemsList_CheckLengthReturnedData_Return3()
        {
            const decimal rate = 1m;
            const int excepted = 3;

            var agent = ArrangeGridViewBasketAgent(rate);

            var actual = agent.GetOrderItemsList(null, null).Count();

            Assert.AreEqual(excepted, actual);
        }

        [TestMethod]
        public void GetOrderItemsList_CheckReturnedData_ReturnExpectedData()
        {
            const decimal rate = 1m;

            var agent = ArrangeGridViewBasketAgent(rate);

            var actual = agent.GetOrderItemsList(null, null).ToArray();

            Assert.AreEqual(OrderItemsResultWithoutCurrencyConverting.Length, actual.Length);
            for (var i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(OrderItemsResultWithoutCurrencyConverting[i].Count, actual[i].Count);
                Assert.AreEqual(OrderItemsResultWithoutCurrencyConverting[i].Name, actual[i].Name);
                Assert.AreEqual(OrderItemsResultWithoutCurrencyConverting[i].Price, actual[i].Price);
                Assert.AreEqual(OrderItemsResultWithoutCurrencyConverting[i].Total, actual[i].Total);
            }
        }

        [TestMethod]
        public void GetOrderItemsList_CheckReturnedDataWasConverted_ReturnConvertedPriceAndTotalValues()
        {
            const decimal rate = 2m;

            var agent = ArrangeGridViewBasketAgent(rate);

            var actual = agent.GetOrderItemsList(null, null).ToArray();

            Assert.AreEqual(OrderItemsResultWithoutCurrencyConverting.Length, actual.Length);
            for (var i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(rate * OrderItemsResultWithoutCurrencyConverting[i].Price, actual[i].Price);
                Assert.AreEqual(rate * OrderItemsResultWithoutCurrencyConverting[i].Total, actual[i].Total);
            }
        }


        private static GridViewBasketAgent<object> ArrangeGridViewBasketAgent(decimal rate)
        {
            var mockStorageService = new Mock<IStorageService<object>>();

            var mockCurrencyConverter = new Mock<ICurrencyConverter>();
            mockCurrencyConverter.Setup(
                m => m.GetRate(It.IsAny<CultureInfo>(), It.IsAny<CultureInfo>(), It.IsAny<DateTime>())).Returns(rate);

            mockCurrencyConverter.Setup(
                m => m.ConvertByRate(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns((decimal v, decimal r) => v*r);

            var mockProductRepository = new Mock<IEfProductRepository>();
            mockProductRepository.Setup(m => m.Products).Returns(Products.AsQueryable());

            var mockOrderRepository = new Mock<IOrderRepository<object>>();
            mockOrderRepository.Setup(m => m.GetAll(It.IsAny<object>())).Returns(OrderList);

            return new GridViewBasketAgent<object>(mockStorageService.Object,
                mockCurrencyConverter.Object, mockProductRepository.Object, mockOrderRepository.Object);
        }
    }
}
