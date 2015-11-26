using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewAgent<T> : IGridViewManager<T>
    {
        private readonly IGridViewPageIndexService _gridViewPageIndexService;
        private readonly ICurrencyConverter _currencyConverter;

        public GridViewAgent(IGridViewPageIndexService gridViewPageIndexService, ICurrencyConverter currencyConverter)
        {
            _gridViewPageIndexService = gridViewPageIndexService;
            _currencyConverter = currencyConverter;
        }

        public void Fill(GridView table, IQueryable<T> data)
        {
            table.DataSource = data.ToList();
            table.DataBind();
        }

        public void SetCultureForPriceColumns(GridView table, CultureInfo cultureTo, params int[] columnsIndexes)
        {
            var cultureFrom = new CultureInfo("ru-RU");
            var rate = _currencyConverter.GetRate(cultureFrom, cultureTo, DateTime.Now);
            foreach (var columnIndex in columnsIndexes)
            {
                foreach (GridViewRow row in table.Rows)
                {
                    var price = _currencyConverter.ConvertByRate(decimal.Parse(row.Cells[columnIndex].Text), rate);
                    row.Cells[columnIndex].Text = price.ToString("C", cultureTo);
                }
            }
        }

        public void SavePageIndex(object repository, string name, int index)
        {
            _gridViewPageIndexService.SetPageIndexByName(repository, name, index);
        }

        public int RestorePageIndex(object repository, string name)
        {
            return _gridViewPageIndexService.GetPageIndexByName(repository, name);
        }

        public void RefreshPageIndex(object repository, string name, GridView table)
        {
            var pageIndex = RestorePageIndex(repository, name);

            if (table.PageIndex == pageIndex) return;

            var tablePageCount = table.PageCount;
            if (pageIndex < tablePageCount)
                table.PageIndex = pageIndex;
            else
            {
                _gridViewPageIndexService.SetPageIndexByName(repository, name, tablePageCount);
                table.PageIndex = tablePageCount;
            }
        }
    }
}