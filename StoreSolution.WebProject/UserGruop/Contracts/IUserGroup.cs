using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace StoreSolution.WebProject.UserGruop.Contracts
{
    public interface IUserGroup
    {
        MembershipUser GetUser();

        void SignOut(HttpResponse response, HttpSessionState sessionState);
    }
}