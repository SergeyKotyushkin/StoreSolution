using System.Web;
using StoreSolution.BusinessLogic.Lang.Contracts;

namespace StoreSolution.BusinessLogic.Lang
{
    public class LangSetter : ILangSetter
    {
        public string Set(string name)
        {
            return (string)HttpContext.GetGlobalResourceObject("Lang", name);
        } 
    }
}