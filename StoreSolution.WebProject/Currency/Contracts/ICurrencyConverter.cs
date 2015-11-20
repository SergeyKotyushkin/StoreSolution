namespace StoreSolution.WebProject.Currency.Contracts
{
    public interface ICurrencyConverter
    {
        decimal ConvertFromRu(decimal value, string cultureName);
        decimal ConvertFromRu(decimal value, decimal? rate);
        decimal ConvertToRu(decimal value, string cultureName);
        decimal GetRate(string cultureName);
        string GetCultureNameForCurrency(string currency);
    }
}