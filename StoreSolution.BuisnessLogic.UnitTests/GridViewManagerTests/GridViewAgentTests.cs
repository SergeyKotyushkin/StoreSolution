using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.GridViewManager;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.StructureMap;

namespace StoreSolution.BuisnessLogic.UnitTests.GridViewManagerTests
{
    [TestClass]
    public class GridViewAgentTests
    {
        private const string StorageName = "index";
        private static bool _onDataBoundEnded;
        private readonly int[] _priceColumns = { 3, 4 };

        private static readonly Book[] Data =
        {
            new Book("Book1", "Author1", 19, 130m, 10m),
            new Book("Book2", "Author2", 10, 80m, 8m),
            new Book("Book3", "Author1", 25, 100m, 3m),
            new Book("Book4", "Author3", 45, 180m, 30),
            new Book("Book5", "Author4", 56, 50m, 6m),
            new Book("Book6", "Author1", 78, 67m, 5m)
        };

        private static Dictionary<string, int> _repository;

        private readonly EventHandler _onDataBound = (sender, args) => { _onDataBoundEnded = true; };


        [TestMethod]
        public void Fill_CheckReturnedDataLength_Return6()
        {
            var gridView = ArrangeGridView();
            var agent = ArrangeGridViewAgent(0);

            agent.Fill(gridView, Data.AsQueryable());
            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            const int expected = 6;
            var actual = gridView.Rows.Count;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Fill_CheckReturnedData_ReturnExpectedData()
        {
            var gridView = ArrangeGridView();
            var agent = ArrangeGridViewAgent(0);

            agent.Fill(gridView, Data.AsQueryable());
            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            Assert.AreEqual(Data.Length, gridView.Rows.Count);
            for (var i = 0; i < Data.Length; i++)
            {
                Assert.AreEqual(Data[i].Name, gridView.Rows[i].Cells[0].Text);
                Assert.AreEqual(Data[i].Author, gridView.Rows[i].Cells[1].Text);
                Assert.AreEqual(Data[i].PagesCount + "", gridView.Rows[i].Cells[2].Text);
                Assert.AreEqual(Data[i].Price + "", gridView.Rows[i].Cells[3].Text);
                Assert.AreEqual(Data[i].Discount + "", gridView.Rows[i].Cells[4].Text);
            }
        }

        [TestMethod]
        public void SetCultureForPriceColumns_ConvertPriceColumnsValuesToCurrency_CheckArePriceColumnsValuesContainsCultureCurrencySymbol()
        {
            const decimal rate = 1m;
            const bool performConvert = false;
            var culture = new CultureInfo("en-US");
            var currencySymbol = new RegionInfo(culture.LCID).CurrencySymbol;
            var gridView = ArrangeGridView();
            var agent = ArrangeGridViewAgent(rate);

            agent.Fill(gridView, Data.AsQueryable());
            while (!_onDataBoundEnded) { /* wait until data will bound */ }
            
            agent.SetCultureForPriceColumns(gridView, culture, performConvert, _priceColumns);

            Assert.AreEqual(Data.Length, gridView.Rows.Count);
            for (var i = 0; i < Data.Length; i++)
            {
                foreach (var priceColumn in _priceColumns)
                {
                    Assert.AreEqual(true, gridView.Rows[i].Cells[priceColumn].Text.Contains(currencySymbol));
                }
            }
        }

        [TestMethod]
        public void SetCultureForPriceColumns_ConvertPriceColumnsValuesToCurrencyByRate_CheckArePriceColumnsValuesConverted()
        {
            const decimal rate = 2m;
            const bool performConvert = true;
            var culture = new CultureInfo("en-US");
            var gridView = ArrangeGridView();
            var agent = ArrangeGridViewAgent(rate);

            agent.Fill(gridView, Data.AsQueryable());
            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            agent.SetCultureForPriceColumns(gridView, culture, performConvert, _priceColumns);

            Assert.AreEqual(Data.Length, gridView.Rows.Count);
            for (var i = 0; i < Data.Length; i++)
            {
                foreach (var priceColumn in _priceColumns)
                {
                    var expectedRatedCurrency =
                        (rate*(priceColumn == 3 ? Data[i].Price : Data[i].Discount)).ToString("C", culture);
                    Assert.AreEqual(expectedRatedCurrency, gridView.Rows[i].Cells[priceColumn].Text);
                }
            }
        }

        [TestMethod]
        public void SavePageIndex_SaveIndex7ToRepositoryByName_CheckIndex7InRepositoryByName()
        {
            _repository = new Dictionary<string, int>();

            var agent = ArrangeGridViewAgent(0);

            agent.SavePageIndex(_repository, StorageName, 7);

            Assert.AreEqual(1, _repository.Count);
            Assert.AreEqual(true, _repository.ContainsKey(StorageName));
            Assert.AreEqual(7, _repository[StorageName]);
        }

        [TestMethod]
        public void RestorePageIndex_GetIndexByNameFromRepository_Return2()
        {
            _repository = new Dictionary<string, int> {{StorageName, 2}};

            var agent = ArrangeGridViewAgent(0);

            agent.RestorePageIndex(_repository, StorageName);

            Assert.AreEqual(true, _repository.ContainsKey(StorageName));
            Assert.AreEqual(2, _repository[StorageName]);
        }

        [TestMethod]
        public void CheckIsPageIndexNeedToRefresh_PageIndexAndStoredByNameIndexSameAndEqual3_ReturnFalse()
        {
            _repository = new Dictionary<string, int> {{StorageName, 3}};

            var gridView = ArrangeGridView();
            gridView.PageIndex = 3;

            var agent = ArrangeGridViewAgent(0);

            var actual = agent.CheckIsPageIndexNeedToRefresh(_repository, StorageName, gridView);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void CheckIsPageIndexNeedToRefresh_PageIndex2AndStoredIndex3_ReturnTrue()
        {
            _repository = new Dictionary<string, int> {{StorageName, 3}};

            var gridView = ArrangeGridView();
            gridView.PageIndex = 2;

            var agent = ArrangeGridViewAgent(0);

            var actual = agent.CheckIsPageIndexNeedToRefresh(_repository, StorageName, gridView);

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void SetGridViewPageIndex_SetGridViewPageIndexTo4WhenPageCount6_ReturnGridViewPageIndexEqual4()
        {
            _repository = new Dictionary<string, int> {{StorageName, 4}};

            var gridView = ArrangeGridView();
            SetPagingPropertiesToGridView(gridView);
            var agent = ArrangeGridViewAgent(0);

            agent.Fill(gridView, Data.AsQueryable());
            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            agent.SetGridViewPageIndex(_repository, StorageName, gridView);

            var actual = gridView.PageIndex;

            Assert.AreEqual(4, actual);
        }

        [TestMethod]
        public void SetGridViewPageIndex_SetGridViewPageIndexTo9WhenPageCount6_ReturnGridViewPageIndexEqual5()
        {
            _repository = new Dictionary<string, int> { { StorageName, 9 } };

            var gridView = ArrangeGridView();
            SetPagingPropertiesToGridView(gridView);
            var agent = ArrangeGridViewAgent(0);

            agent.Fill(gridView, Data.AsQueryable());
            while (!_onDataBoundEnded) { /* wait until data will bound */ }
            
            agent.SetGridViewPageIndex(_repository, StorageName, gridView);

            var actual = gridView.PageIndex;

            Assert.AreEqual(5, actual);
        }


        private GridView ArrangeGridView()
        {
            var gridView = new GridView();
            gridView.DataBound += _onDataBound;
            return gridView;
        }

        private static void SetPagingPropertiesToGridView(GridView gridView)
        {
            gridView.PageSize = 1;
            gridView.PagerSettings.Visible = false;
            gridView.AllowPaging = true;
        }
        
        private static GridViewAgent<Book, Dictionary<string, int>> ArrangeGridViewAgent(decimal rate)
        {
            var mockStorageService = new Mock<IStorageService<Dictionary<string, int>>>();
            mockStorageService.Setup(
                m =>
                    m.SetPageIndexByName(It.IsAny<Dictionary<string, int>>(), It.Is<string>(s => s == StorageName),
                        It.IsAny<int>()))
                .Callback((Dictionary<string, int> r, string s, int i) => r[s] = i);

            mockStorageService.Setup(
                m =>
                    m.GetPageIndexByName(It.IsAny<Dictionary<string, int>>(), It.Is<string>(s => s == StorageName)))
                .Returns((Dictionary<string, int> r, string s) => r[s]);
            
            var mockCurrencyConverter = new Mock<ICurrencyConverter>();
            mockCurrencyConverter.Setup(
                m => m.GetRate(It.IsAny<CultureInfo>(), It.IsAny<CultureInfo>(), It.IsAny<DateTime>())).Returns(rate);

            mockCurrencyConverter.Setup(
                m => m.ConvertByRate(It.IsAny<decimal>(), It.IsAny<decimal>())).Returns((decimal v, decimal r) => v * r);

            StructureMapFactory.Init();

            return new GridViewAgent<Book, Dictionary<string, int>>(mockStorageService.Object,
                mockCurrencyConverter.Object);
        }
    }

    public class Book
    {
        public Book(string name, string author, int pagesCount, decimal price, decimal discount)
        {
            Name = name;
            Author = author;
            PagesCount = pagesCount;
            Price = price;
            Discount = discount;
        }

        public string Name { get; set; }
        public string Author { get; set; }
        public int PagesCount { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}
