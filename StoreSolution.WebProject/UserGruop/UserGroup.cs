using System;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using StoreSolution.WebProject.Log4net;
using StoreSolution.WebProject.UserGruop.Contracts;

namespace StoreSolution.WebProject.UserGruop
{
    public class UserGroup : IUserGroup
    {
        public MembershipUser GetUser()
        {
            var user = Membership.GetUser();

            if (!CheckIsUserNotNull(user)) throw new NullReferenceException();

            return user;
        }

        public void SignOut(HttpResponse response, HttpSessionState sessionState)
        {
            var user = GetUser();
            Logger.Log.Error(string.Format("User {0} sing out.", user.UserName));

            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetExpires(DateTime.Now);

            FormsAuthentication.SignOut();
            sessionState.Abandon();

            response.Redirect("~/Index.aspx");
            response.End();
        }

        private static bool CheckIsUserNotNull(MembershipUser user)
        {
            return user != null;
        }
    }
}