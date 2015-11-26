using System.Web.SessionState;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewPageIndexSessionService : IGridViewPageIndexService
    {
        public int GetPageIndexByName(object repository, string name)
        {
            var session = (HttpSessionState) repository;

            var index = session[name];
            return (int?) index ?? 0;
        }

        public void SetPageIndexByName(object repository, string name, int index)
        {
            var session = (HttpSessionState)repository;

            session[name] = index;
        }
    }
}