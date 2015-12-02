using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewProfileAgent<T> : GridViewAgent<OrderToGrid, T>, IGridViewProfileManager<T>
    {
        private readonly IEfOrderHistoryRepository _efOrderHistoryRepository;
        private readonly ILangSetter _langSetter;

        public GridViewProfileAgent(IStorageService<T> storageService, ICurrencyConverter currencyConverter,
            IEfOrderHistoryRepository efOrderHistoryRepository, ILangSetter langSetter)
            : base(storageService, currencyConverter)
        {
            _efOrderHistoryRepository = efOrderHistoryRepository;
            _langSetter = langSetter;
        }


        public IQueryable<OrderToGrid> GetOrderHistoriesList(string userName, CultureInfo culture)
        {
            var history = _efOrderHistoryRepository.GetAll.Where(u => u.PersonName == userName).OrderBy(u => u.Date).ToList();
            
            var jss = new JavaScriptSerializer();

            var number = 1;
            var ordersFromHistory = (from h in history
                                     let productsOrder = jss.Deserialize<ProductsOrder[]>(h.Order)
                                     select new OrderFromHistory
                                     {
                                         Number = number++,
                                         Email = h.PersonEmail,
                                         Date = h.Date,
                                         ProductsOrder = productsOrder,
                                         Total = h.Total,
                                         CultureName = h.Culture
                                     }).ToArray();

            return ordersFromHistory.Select(o =>
            {
                var ordersToGrid = new OrderToGrid(o, _langSetter);
                ordersToGrid.Order = HttpContext.Current.Server.HtmlDecode(ordersToGrid.Order);
                return ordersToGrid;
            }).AsQueryable();
        }
    }
}