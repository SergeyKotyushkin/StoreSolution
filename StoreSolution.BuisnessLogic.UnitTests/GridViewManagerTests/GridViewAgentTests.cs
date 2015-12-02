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
        static bool _onDataBoundEnded;
        readonly int[] _priceColumns = { 3, 4 };

        private readonly Book[] _data =
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
        public void Fill_SetDataToGridview_GridViewHasSameValues()
        {
            var gridView = ArrangeGridView(false);

            var agent = ArrangeGridViewAgent();

            agent.Fill(gridView, _data.AsQueryable());

            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            Assert.AreEqual(_data.Length, gridView.Rows.Count);
            for (var i = 0; i < _data.Length; i++)
            {
                Assert.AreEqual(_data[i].Name, gridView.Rows[i].Cells[0].Text);
                Assert.AreEqual(_data[i].Author, gridView.Rows[i].Cells[1].Text);
                Assert.AreEqual(_data[i].PagesCount.ToString(), gridView.Rows[i].Cells[2].Text);
                Assert.AreEqual(_data[i].Price+"", gridView.Rows[i].Cells[3].Text);
                Assert.AreEqual(_data[i].Discount + "", gridView.Rows[i].Cells[4].Text);
            }
        }

        [TestMethod]
        public void SetCultureForPriceColumns_SetDataToGridViewWithoutConvert_PriceColumnsContainsUsdSymbol()
        {
            var gridView = ArrangeGridView(false);

            var agent = ArrangeGridViewAgent();
            agent.Fill(gridView, _data.AsQueryable());

            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            var culture = new CultureInfo("en-US");
            const bool performConvert = false;
            agent.SetCultureForPriceColumns(gridView, culture, performConvert, _priceColumns);

            Assert.AreEqual(_data.Length, gridView.Rows.Count);
            for (var i = 0; i < _data.Length; i++)
            {
                foreach (var priceColumn in _priceColumns)
                {
                    Assert.AreEqual(true, gridView.Rows[i].Cells[priceColumn].Text.Contains('$'));

                    Assert.AreEqual(priceColumn == 3 ? _data[i].Price : _data[i].Discount,
                        decimal.Parse(gridView.Rows[i].Cells[priceColumn].Text, NumberStyles.Currency, culture));
                }
            }
        }

        [TestMethod]
        public void SetCultureForPriceColumns_SetDataToGridViewPerformConvert_PriceColumnsContainsUsdSymbolAndConvertedValues()
        {
            var gridView = ArrangeGridView(false);

            var agent = ArrangeGridViewAgent();
            agent.Fill(gridView, _data.AsQueryable());

            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            var culture = new CultureInfo("en-US");
            const bool performConvert = true;
            agent.SetCultureForPriceColumns(gridView, culture, performConvert, _priceColumns);

            var converter = StructureMapFactory.Resolve<ICurrencyConverter>();

            Assert.AreEqual(_data.Length, gridView.Rows.Count);
            for (var i = 0; i < _data.Length; i++)
            {
                foreach (var priceColumn in _priceColumns)
                {
                    Assert.AreEqual(true, gridView.Rows[i].Cells[priceColumn].Text.Contains('$'));

                    var value = converter.ConvertFromRubles(culture,
                        priceColumn == 3 ? _data[i].Price : _data[i].Discount, DateTime.Now);
                    Assert.AreEqual(value,
                        decimal.Parse(gridView.Rows[i].Cells[priceColumn].Text, NumberStyles.Currency, culture));
                }
            }
        }

        [TestMethod]
        public void SavePageIndex_Save7ToRepository_Check7InRepository()
        {
            _repository.Clear();

            var agent = ArrangeGridViewAgent();

            agent.SavePageIndex(_repository, StorageName, 7);

            Assert.AreEqual(1, _repository.Count);
            Assert.AreEqual(7, _repository[StorageName]);
        }

        [TestMethod]
        public void RestorePageIndex_GetByNameFromRepository_Check2InRepository()
        {
            _repository = new Dictionary<string, int> {{StorageName, 2}};

            var agent = ArrangeGridViewAgent();

            agent.RestorePageIndex(_repository, StorageName);

            Assert.AreEqual(2, _repository[StorageName]);
        }

        [TestMethod]
        public void CheckIsPageIndexNeedToRefresh_PageIndexNoNeedToRefresh_ReturnFalse()
        {
            _repository = new Dictionary<string, int> {{StorageName, 3}};

            var gridView = ArrangeGridView(false);
            gridView.PageIndex = 3;

            var agent = ArrangeGridViewAgent();

            var actual = agent.CheckIsPageIndexNeedToRefresh(_repository, StorageName, gridView);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void CheckIsPageIndexNeedToRefresh_PageIndexNeedToRefresh_ReturnTrue()
        {
            _repository = new Dictionary<string, int> {{StorageName, 3}};

            var gridView = ArrangeGridView(false);
            gridView.PageIndex = 2;
            
            var agent = ArrangeGridViewAgent();
            
            var actual = agent.CheckIsPageIndexNeedToRefresh(_repository, StorageName, gridView);

            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void SetGridViewPageIndex_SetGridViewPageIndexTo4WhenPageCount6_ReturnGridViewPageIndexAs4()
        {
            _repository = new Dictionary<string, int> {{StorageName, 4}};

            var gridView = ArrangeGridView(true);
            
            var agent = ArrangeGridViewAgent();
            agent.Fill(gridView, _data.AsQueryable());
            
            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            agent.SetGridViewPageIndex(_repository, StorageName, gridView);

            var actual = gridView.PageIndex;

            Assert.AreEqual(4, actual);
        }

        [TestMethod]
        public void SetGridViewPageIndex_SetGridViewPageIndexTo9WhenPageCount6_ReturnGridViewPageIndexAs5()
        {
            _repository = new Dictionary<string, int> {{StorageName, 9}};

            var gridView = ArrangeGridView(true);

            var agent = ArrangeGridViewAgent();
            agent.Fill(gridView, _data.AsQueryable());

            while (!_onDataBoundEnded) { /* wait until data will bound */ }

            agent.SetGridViewPageIndex(_repository, StorageName, gridView);

            var actual = gridView.PageIndex;

            Assert.AreEqual(5, actual);
        }


        private GridView ArrangeGridView(bool allowPaging)
        {
            var gridView = new GridView();

            gridView.DataBound += _onDataBound;

            if (!allowPaging) return gridView;

            gridView.PageSize = 1;
            gridView.PagerSettings.Visible = false;
            gridView.AllowPaging = true;

            return gridView;
        }
        
        private static GridViewAgent<Book, Dictionary<string, int>> ArrangeGridViewAgent()
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
            
            StructureMapFactory.Init();

            return new GridViewAgent<Book, Dictionary<string, int>>(
                mockStorageService.Object,
                StructureMapFactory.Resolve<ICurrencyConverter>());
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
