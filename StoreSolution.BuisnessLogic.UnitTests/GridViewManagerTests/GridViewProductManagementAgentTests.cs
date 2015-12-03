using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.GridViewManager;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;

namespace StoreSolution.BuisnessLogic.UnitTests.GridViewManagerTests
{
    [TestClass]
    public class GridViewProductManagementAgentTests
    {
        private static List<Product> _products;
        
        [TestMethod]
        public void AddOrUpdateProduct_TryAddValidProduct_ReturnSuccessResult()
        {
            _products = new List<Product>();
            var agent = ArrangeGridViewProductManagementAgent(0);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 10, name: "Lego", category: "Play", price: "400");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: true);

            Assert.AreEqual(EditingResults.Success, actualResult);
            Assert.AreEqual(1, _products.Count);
        }

        [TestMethod]
        public void AddOrUpdateProduct_TryAddNotValidProductOnName_ReturnFailValidProductResult()
        {
            _products = new List<Product>();
            var agent = ArrangeGridViewProductManagementAgent(0);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 10, name: "1Lego", category: "Play", price: "400");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: true);

            Assert.AreEqual(EditingResults.FailValidProduct, actualResult);
            Assert.AreEqual(0, _products.Count);
        }

        [TestMethod]
        public void AddOrUpdateProduct_TryAddNotValidProductOnCategory_ReturnFailValidProductResult()
        {
            _products = new List<Product>();
            var agent = ArrangeGridViewProductManagementAgent(0);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 10, name: "1Lego", category: "- 1www", price: "400");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: true);

            Assert.AreEqual(EditingResults.FailValidProduct, actualResult);
            Assert.AreEqual(0, _products.Count);
        }

        [TestMethod]
        public void AddOrUpdateProduct_TryAddNotValidProductOnPrice_ReturnFailValidProductResult()
        {
            _products = new List<Product>();
            var agent = ArrangeGridViewProductManagementAgent(0);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 10, name: "1Lego", category: "Play", price: "price");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: true);

            Assert.AreEqual(EditingResults.FailValidProduct, actualResult);
            Assert.AreEqual(0, _products.Count);
        }

        [TestMethod]
        public void AddOrUpdateProduct_TryUpdateValidProduct_ReturnSuccessResult()
        {
            _products = new List<Product> {new Product {Id = 777, Name = "Pepper", Category = "Food", Price = 15m}};
            const decimal rate = 1m;
            var agent = ArrangeGridViewProductManagementAgent(rate);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 777, name: "Pepper Hot", category: "Food", price: "29");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: false);

            Assert.AreEqual(EditingResults.Success, actualResult);
            Assert.AreEqual(1, _products.Count);
            Assert.AreEqual(777, _products[0].Id);
            Assert.AreEqual("Pepper Hot", _products[0].Name);
            Assert.AreEqual("Food", _products[0].Category);
            Assert.AreEqual(29m, _products[0].Price);
        }

        [TestMethod]
        public void AddOrUpdateProduct_TryUpdateNotValidProductOnName_ReturnFailValidProductResult()
        {
            _products = new List<Product> { new Product { Id = 777, Name = "Pepper", Category = "Food", Price = 15m } };
            var agent = ArrangeGridViewProductManagementAgent(1);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 777, name: "5412Pepper Hot", category: "Food", price: "29");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: false);

            Assert.AreEqual(EditingResults.FailValidProduct, actualResult);
            Assert.AreEqual(1, _products.Count);
            Assert.AreEqual(777, _products[0].Id);
            Assert.AreEqual("Pepper", _products[0].Name);
            Assert.AreEqual("Food", _products[0].Category);
            Assert.AreEqual(15m, _products[0].Price);
        }

        [TestMethod]
        public void AddOrUpdateProduct_TryUpdateNotValidProductOnCategory_ReturnFailValidProductResult()
        {
            _products = new List<Product> { new Product { Id = 777, Name = "Pepper", Category = "Food", Price = 15m } };
            var agent = ArrangeGridViewProductManagementAgent(0);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 777, name: "Pepper Hot", category: "___Food", price: "29");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: false);

            Assert.AreEqual(EditingResults.FailValidProduct, actualResult);
            Assert.AreEqual(1, _products.Count);
            Assert.AreEqual(777, _products[0].Id);
            Assert.AreEqual("Pepper", _products[0].Name);
            Assert.AreEqual("Food", _products[0].Category);
            Assert.AreEqual(15m, _products[0].Price);
        }

        [TestMethod]
        public void AddOrUpdateProduct_TryUpdateNotValidProductOnPrice_ReturnFailValidProductResult()
        {
            _products = new List<Product> { new Product { Id = 777, Name = "Pepper", Category = "Food", Price = 15m } };
            var agent = ArrangeGridViewProductManagementAgent(0);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 777, name: "Pepper Hot", category: "___Food", price: "2$9");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: true);

            Assert.AreEqual(EditingResults.FailValidProduct, actualResult);
            Assert.AreEqual(1, _products.Count);
            Assert.AreEqual(777, _products[0].Id);
            Assert.AreEqual("Pepper", _products[0].Name);
            Assert.AreEqual("Food", _products[0].Category);
            Assert.AreEqual(15m, _products[0].Price);
        }
        
        [TestMethod]
        public void AddOrUpdateProduct_AddProductWithPriceConvertingByRate_ReturnConvertedByRatePrice()
        {
            _products = new List<Product>();
            const decimal rate = 4m;
            const decimal price = 400m;
            var agent = ArrangeGridViewProductManagementAgent(rate);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 10, name: "Lego", category: "Play", price: price+"");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: true);

            Assert.AreEqual(EditingResults.Success, actualResult);
            Assert.AreEqual(1, _products.Count);
            Assert.AreEqual(_products[0].Price, rate*price);
        }

        [TestMethod]
        public void AddOrUpdateProduct_UpdateProductWithPriceConvertingByRate_ReturnConvertedByRatePrice()
        {
            const decimal rate = 2.45m;
            const decimal price = 15m;
            _products = new List<Product> {new Product {Id = 890, Name = "Pepper", Category = "Food", Price = price}};
            var agent = ArrangeGridViewProductManagementAgent(rate);
            var culture = new CultureInfo("en-US");
            var tableRow = ArrangeTableRow(id: 890, name: "Lego", category: "Play", price: price + "");

            var actualResult = agent.AddOrUpdateProduct(tableRow, culture, isAdd: false);

            Assert.AreEqual(EditingResults.Success, actualResult);
            Assert.AreEqual(1, _products.Count);
            Assert.AreEqual(_products[0].Price, rate * price);
        }


        private static TableRow ArrangeTableRow(int id, string name, string category, string price)
        {
            var tableRow = new TableRow();
            tableRow.Cells.Add(new TableCell());
            tableRow.Cells.Add(new TableCell());
            tableRow.Cells.Add(new TableCell {Controls = {new TextBox {Text = id + ""}}});
            tableRow.Cells.Add(new TableCell {Controls = {new TextBox {Text = name}}});
            tableRow.Cells.Add(new TableCell {Controls = {new TextBox {Text = category}}});
            tableRow.Cells.Add(new TableCell {Controls = {new TextBox {Text = price}}});
            return tableRow;
        }

        private static GridViewProductManagementAgent<object> ArrangeGridViewProductManagementAgent(decimal rate)
        {
            var mockStorageService = new Mock<IStorageService<object>>();

            var mockCurrencyConverter = new Mock<ICurrencyConverter>();
            mockCurrencyConverter.Setup(
                m => m.ConvertToRubles(It.IsAny<CultureInfo>(), It.IsAny<decimal>(), It.IsAny<DateTime>()))
                .Returns((CultureInfo c, decimal v, DateTime d) => v*rate);

            var mockProductRepository = new Mock<IEfProductRepository>();
            mockProductRepository.Setup(m => m.AddOrUpdateProduct(It.Is<Product>(p => p == null))).Returns(false);
            mockProductRepository.Setup(m => m.AddOrUpdateProduct(It.Is<Product>(p => p != null))).Returns(
                (Product p) =>
                {
                    var index = _products.FindIndex(q => q.Id == p.Id);
                    if (index == -1) _products.Add(p);
                    else _products[index] = p;
                    return true;
                });

            return new GridViewProductManagementAgent<object>(mockStorageService.Object, mockCurrencyConverter.Object,
                mockProductRepository.Object);
        }
    }
}
