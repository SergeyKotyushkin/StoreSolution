using System.Globalization;

namespace StoreSolution.BusinessLogic.Currency.Contracts
{
    public interface ICurrencyCultureService<in T>
    {
        CultureInfo GetCurrencyCultureInfo(T repository, string cultureNameInRepository);
    }
}