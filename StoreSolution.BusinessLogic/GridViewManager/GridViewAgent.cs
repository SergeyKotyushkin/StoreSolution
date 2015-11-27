using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewAgent<T, TV> : IGridViewManager<T, TV>
    {
        private readonly IStorageService<TV> _storageService;
        private readonly ICurrencyConverter _currencyConverter;

        public GridViewAgent(IStorageService<TV> storageService, ICurrencyConverter currencyConverter)
        {
            _storageService = storageService;
            _currencyConverter = currencyConverter;
        }

        public void Fill(GridView table, IQueryable<T> data)
        {
            table.DataSource = data.ToList();
            table.DataBind();
        }

        public void SetCultureForPriceColumns(GridView table, CultureInfo cultureTo, bool performConvert,
            params int[] columnsIndexes)
        {
            var cultureFrom = new CultureInfo("ru-RU");
            var rate = _currencyConverter.GetRate(cultureFrom, cultureTo, DateTime.Now);
            foreach (var columnIndex in columnsIndexes)
            {
                foreach (GridViewRow row in table.Rows)
                {
                    if (row.Cells[columnIndex].Controls.Count != 0) continue;

                    var price = decimal.Parse(row.Cells[columnIndex].Text);
                    if (performConvert)
                        price = _currencyConverter.ConvertByRate(price, rate);

                    row.Cells[columnIndex].Text = price.ToString("C", cultureTo);
                }
            }
        }

        public void SavePageIndex(TV repository, string name, int index)
        {
            _storageService.SetPageIndexByName(repository, name, index);
        }

        public int RestorePageIndex(TV repository, string name)
        {
            return _storageService.GetPageIndexByName(repository, name);
        }

        public void FillGridViewAndRefreshPageIndex(GridView table, IQueryable<T> data, TV repository, string name)
        {
            Fill(table, data);

            if (RefreshPageIndex(repository, name, table))
                Fill(table, data);
        }


        private bool RefreshPageIndex(TV repository, string name, GridView table)
        {
            var pageIndex = RestorePageIndex(repository, name);

            if (table.PageIndex == pageIndex) return false;

            var tablePageCount = table.PageCount;
            if (pageIndex < tablePageCount)
                table.PageIndex = pageIndex;
            else
            {
                _storageService.SetPageIndexByName(repository, name, tablePageCount);
                table.PageIndex = tablePageCount;
            }

            return true;
        }
    }
}