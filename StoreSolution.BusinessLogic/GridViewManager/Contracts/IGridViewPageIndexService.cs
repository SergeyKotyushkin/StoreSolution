namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IGridViewPageIndexService
    {
        int GetPageIndexByName(object repository, string name);

        void SetPageIndexByName(object repository, string name, int index);
    }
}