﻿using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewManager<in T>
    {
        void Fill(GridView table, IQueryable<T> data);

        void SetCultureForPriceColumns(GridView table, CultureInfo cultureTo, params int[] columnsIndexes);

        void SavePageIndex(object repository, string name, int index);

        int RestorePageIndex(object repository, string name);

        void RefreshPageIndex(object repository, string name, GridView table);
    }
}