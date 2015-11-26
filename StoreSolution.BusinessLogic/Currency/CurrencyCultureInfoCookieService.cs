using System.Globalization;
using System.Web;
using StoreSolution.BusinessLogic.Currency.Contracts;

namespace StoreSolution.BusinessLogic.Currency
{
    public class CurrencyCultureInfoCookieService : ICurrencyCultureInfoService
    {
        public CultureInfo GetCurrencyCultureInfo(object repository, string cultureNameInRepository)
        {
            var cookies = (HttpCookieCollection) repository;

            var cookie = cookies.Get(cultureNameInRepository);
            var currencyCultureName = CultureInfo.CurrentCulture.Name;
            if (cookie == null)
                cookies.Set(new HttpCookie(cultureNameInRepository, currencyCultureName));
            else
                currencyCultureName = cookie.Value;

            return new CultureInfo(currencyCultureName);
        }
    }
}