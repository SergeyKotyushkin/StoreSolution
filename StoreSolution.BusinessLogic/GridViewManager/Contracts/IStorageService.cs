namespace StoreSolution.BusinessLogic.GridViewManager.Contracts
{
    public interface IStorageService<in T>
    {
        int GetPageIndexByName(T repository, string name);

        void SetPageIndexByName(T repository, string name, int index);
    }
}