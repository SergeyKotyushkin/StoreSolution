using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewManager<in T, in TV>
    {
        void Fill(GridView table, IQueryable<T> data);

        void SetCultureForPriceColumns(GridView table, CultureInfo cultureTo, bool performConvert, params int[] columnsIndexes);

        void SavePageIndex(TV repository, string name, int index);

        int RestorePageIndex(TV repository, string name);

        bool CheckIsPageIndexNeedToRefresh(TV repository, string name, GridView table);
        
        void SetGridViewPageIndex(TV repository, string name, GridView table);
    }
}