using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace StoreSolution.BusinessLogic.UserGruop.Contracts
{
    public interface IUserGroup
    {
        MembershipUser GetUser(bool canBeAnonymous = false);

        void SignOut(HttpResponse response, HttpSessionState sessionState);
    }
}