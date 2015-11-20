using System.Globalization;

namespace StoreSolution.WebProject.Currency.Contracts
{
    public interface IRateService
    {
        decimal GetRate(CultureInfo cultureFrom, CultureInfo cultureTo);
    }
}