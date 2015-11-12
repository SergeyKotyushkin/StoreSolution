using System.Web;

namespace StoreSolution.WebProject.Lang
{
    public static class LangSetter
    {
        public static string Set(string name)
        {
            return (string)HttpContext.GetGlobalResourceObject("Lang", name);
        } 
    }
}