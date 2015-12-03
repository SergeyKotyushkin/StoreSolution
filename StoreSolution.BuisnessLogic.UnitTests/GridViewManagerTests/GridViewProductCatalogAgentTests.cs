using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.GridViewManager;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;

namespace StoreSolution.BuisnessLogic.UnitTests.GridViewManagerTests
{
    [TestClass]
    public class GridViewProductCatalogAgentTests
    {
        private static bool _onDataBoundEnded;

        private static readonly List<Order> OrderList = new List<Order>
        {
            new Order {Id = 11, Count = 3},
            new Order {Id = 100, Count = 1},
            new Order {Id = 5, Count = 2}
        };

        private static readonly Product[] Products =
        {
            new Product {Id = 100, Name = "Oil", Category = "Food", Price = 60m},
            new Product {Id = 11, Name = "Salt", Category = "Home", Price = 5m},
            new Product {Id = 5, Name = "Cream", Category = "Food", Price = 25m},
            new Product {Id = 29, Name = "Jeans", Category = "Wear", Price = 1500m}
        };

        private static readonly int[] OrderColumn = { 1, 3, 2, 0 };

        private readonly EventHandler _onDataBound = (sender, args) => { _onDataBoundEnded = true; };


        [TestMethod]
        public void FillOrderColumn_CheckReturnedData_ReturnedDataSameAsExpected()
        {
            const int orderColumnIndex = 0;
            const int idColumnIndex = 1;

            var agent = ArrangeGridViewProductCatalogAgent();

            var gridView = ArrangeGridView();

            agent.Fill(gridView, Products.AsQueryable());
            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            agent.FillOrderColumn(gridView, orderColumnIndex, idColumnIndex, null);

            Assert.AreEqual(OrderColumn.Length, gridView.Rows.Count);
            for (var i = 0; i < OrderColumn.Length; i++)
            {
                Assert.AreEqual(OrderColumn[i].ToString(), gridView.Rows[i].Cells[orderColumnIndex].Text);
            }
        }

        [TestMethod]
        public void FillCategories_CheckCategoriesWillAddedToDropDownListDistinct_ReturnListOfDistinctCategoriesForAllProducts()
        {
            var dropDownList = ArrangeDropDownList();
            var agent = ArrangeGridViewProductCatalogAgent();
            
            var expected = new[] { "All", "Food", "Home", "Wear" };

            agent.FillCategories(dropDownList, Products.AsQueryable());

            var actual = dropDownList.Items;

            Assert.AreEqual(expected.Length, actual.Count);
            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i].Text);
            }
        }

        [TestMethod]
        public void FillCategories_CheckIfProductsEmpty_ReturnOnlyOneCategoryNamedAll()
        {
            var dropDownList = ArrangeDropDownList();
            var agent = ArrangeGridViewProductCatalogAgent();

            var expected = new[] {"All"};

            agent.FillCategories(dropDownList, new Product[0].AsQueryable());
            
            var actual = dropDownList.Items;

            Assert.AreEqual(expected.Length, actual.Count);
            Assert.AreEqual(expected[0], actual[0].Text);
        }


        [TestMethod]
        public void GetIdFromRow_CheckIfRowIndexIsValid_ReturnProductByIndex2Id()
        {
            const int rowIndex = 2;
            const int idColumnIndex = 1;
            var gridView = ArrangeGridView();
            var agent = ArrangeGridViewProductCatalogAgent();

            agent.Fill(gridView, Products.AsQueryable());

            const int expected = 5;
            
            var actual = agent.GetIdFromRow(gridView, rowIndex, idColumnIndex);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetIdFromRow_CheckIfRowIndexIsNotValid_ReturnMinus1()
        {
            const int rowIndex = 5;
            const int idColumnIndex = 1;
            var gridView = ArrangeGridView();
            var agent = ArrangeGridViewProductCatalogAgent();

            agent.Fill(gridView, Products.AsQueryable());

            const int expected = -1;

            var actual = agent.GetIdFromRow(gridView, rowIndex, idColumnIndex);

            Assert.AreEqual(expected, actual);
        }


        private GridViewProductCatalogAgent<object> ArrangeGridViewProductCatalogAgent()
        {
            var mockProductRepository = new Mock<IEfProductRepository>();
            mockProductRepository.Setup(m => m.GetProductById(It.IsAny<int>()))
                .Returns((int id) => Products.FirstOrDefault(p => p.Id == id));

            var mockOrderRepository = new Mock<IOrderRepository<object>>();
            mockOrderRepository.Setup(m => m.GetAll(It.IsAny<object>())).Returns(OrderList);

            var mockLangSetter = new Mock<ILangSetter>();
            mockLangSetter.Setup(m => m.Set(It.IsAny<string>())).Returns("All");

            var mockStorageService = new Mock<IStorageService<object>>();
            var mockCurrencyConverter = new Mock<ICurrencyConverter>();

            return new GridViewProductCatalogAgent<object>(mockProductRepository.Object, mockOrderRepository.Object,
                mockLangSetter.Object, mockStorageService.Object, mockCurrencyConverter.Object);
        }

        private GridView ArrangeGridView()
        {
            var gv = new GridView();
            gv.DataBound += _onDataBound;
            gv.Columns.Add(new BoundField());
            return gv;
        }

        private static DropDownList ArrangeDropDownList()
        {
            return new DropDownList();
        }
    }
}
