using System.Web.SessionState;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class StorageSessionService : IStorageService<HttpSessionState>
    {
        public int GetPageIndexByName(HttpSessionState repository, string name)
        {
            var index = repository[name];
            return (int?) index ?? 0;
        }

        public void SetPageIndexByName(HttpSessionState repository, string name, int index)
        {
            repository[name] = index;
        }
    }
}