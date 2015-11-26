using System;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
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

        public void SetCultureForPriceColumns(GridView table, CultureInfo cultureTo, bool performConvert,
            params int[] columnsIndexes)
        {
            var cultureFrom = new CultureInfo("ru-RU");
            var rate = _currencyConverter.GetRate(cultureFrom, cultureTo, DateTime.Now);
            foreach (var columnIndex in columnsIndexes)
            {
                foreach (GridViewRow row in table.Rows)
                {
                    var price = decimal.Parse(row.Cells[columnIndex].Text);
                    if(performConvert) price = _currencyConverter.ConvertByRate(price, rate);
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

        public void FillGridViewAndRefreshPageIndex(GridView table, IQueryable<T> data, object repository, string name)
        {
            Fill(table, data);

            if (RefreshPageIndex((HttpSessionState)repository, name, table))
                Fill(table, data);
        }


        private bool RefreshPageIndex(object repository, string name, GridView table)
        {
            var pageIndex = RestorePageIndex(repository, name);

            if (table.PageIndex == pageIndex) return false;

            var tablePageCount = table.PageCount;
            if (pageIndex < tablePageCount)
                table.PageIndex = pageIndex;
            else
            {
                _gridViewPageIndexService.SetPageIndexByName(repository, name, tablePageCount);
                table.PageIndex = tablePageCount;
            }

            return true;
        }
    }
}