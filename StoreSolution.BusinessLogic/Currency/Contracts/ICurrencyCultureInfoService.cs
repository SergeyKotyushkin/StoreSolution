using System.Globalization;

namespace StoreSolution.BusinessLogic.Currency.Contracts
{
    public interface ICurrencyCultureInfoService
    {
        CultureInfo GetCurrencyCultureInfo(object repository, string cultureNameInRepository);
    }
}