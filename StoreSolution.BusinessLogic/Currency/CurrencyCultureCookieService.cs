using System.Globalization;
using System.Web;
using StoreSolution.BusinessLogic.Currency.Contracts;

namespace StoreSolution.BusinessLogic.Currency
{
    public class CurrencyCultureCookieService : ICurrencyCultureService<HttpCookieCollection>
    {
        public CultureInfo GetCurrencyCultureInfo(HttpCookieCollection repository, string cultureNameInRepository)
        {
            var cookie = repository.Get(cultureNameInRepository);
            var currencyCultureName = CultureInfo.CurrentCulture.Name;
            if (cookie == null)
                repository.Set(new HttpCookie(cultureNameInRepository, currencyCultureName));
            else
                currencyCultureName = cookie.Value;

            return new CultureInfo(currencyCultureName);
        }
    }
}