using System;
using System.Globalization;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.Database.Models;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewProductManagementAgent : GridViewAgent<Product, HttpSessionState>, IGridViewProductManagementManager<HttpSessionState>
    {
        private readonly IEfProductRepository _efProductRepository;
        private readonly ICurrencyConverter _currencyConverter;

        public GridViewProductManagementAgent(IStorageService<HttpSessionState> storageService,
            ICurrencyConverter currencyConverter, IEfProductRepository efProductRepository)
            : base(storageService, currencyConverter)
        {
            _efProductRepository = efProductRepository;
            _currencyConverter = currencyConverter;
        }

        public EditingResults AddOrUpdateProduct(TableRow row, CultureInfo currencyCulture, bool isAdd)
        {
            var id = isAdd ? -1 : int.Parse(((TextBox)row.Cells[2].Controls[0]).Text);
            var name = ((TextBox)row.Cells[3].Controls[0]).Text.Trim();
            var category = ((TextBox)row.Cells[4].Controls[0]).Text.Trim();
            var priceString = ((TextBox)row.Cells[5].Controls[0]).Text.Trim();
            decimal price;

            return CheckIsNewProductValid(name, category, priceString, out price, currencyCulture)
                ? SetProduct(new Product { Id = id, Name = name, Category = category, Price = price }, currencyCulture)
                : EditingResults.FailValidProduct;
        }

        private static bool CheckIsNewProductValid(string name, string category, string price, out decimal priceResult,
            IFormatProvider currencyCulture)
        {
            var rgx = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z]+[a-zA-Z0-9_ ]*$");

            var isDeimal = decimal.TryParse(price, NumberStyles.Currency, currencyCulture, out priceResult);

            return isDeimal && rgx.IsMatch(name) && rgx.IsMatch(category);
        }

        private EditingResults SetProduct(Product product, CultureInfo currencyCulture)
        {
            product.Price = _currencyConverter.ConvertToRubles(currencyCulture, product.Price, DateTime.Now);

            return _efProductRepository.AddOrUpdateProduct(product)
                ? EditingResults.Success
                : EditingResults.FailAddOrUpdate;
        }
    }
}