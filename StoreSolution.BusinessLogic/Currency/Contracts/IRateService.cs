using System.Globalization;

namespace StoreSolution.BusinessLogic.Currency.Contracts
{
    public interface IRateService
    {
        decimal GetRate(CultureInfo cultureFrom, CultureInfo cultureTo);
    }
}