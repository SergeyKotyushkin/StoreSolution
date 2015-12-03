using System.Linq;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.JsonSerialize.Contracts;
using StoreSolution.BusinessLogic.Lang.Contracts;
using StoreSolution.BusinessLogic.Models;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewProfileAgent<T> : GridViewAgent<OrderToGrid, T>, IGridViewProfileManager<T>
    {
        private readonly IEfOrderHistoryRepository _efOrderHistoryRepository;
        private readonly ILangSetter _langSetter;
        private readonly IJsonSerializer _jsonSerializer;

        public GridViewProfileAgent(IStorageService<T> storageService, ICurrencyConverter currencyConverter,
            IEfOrderHistoryRepository efOrderHistoryRepository, ILangSetter langSetter, IJsonSerializer jsonSerializer)
            : base(storageService, currencyConverter)
        {
            _efOrderHistoryRepository = efOrderHistoryRepository;
            _langSetter = langSetter;
            _jsonSerializer = jsonSerializer;
        }


        public IQueryable<OrderToGrid> GetOrderToGridList(string userName)
        {
            var history = _efOrderHistoryRepository.GetAll.Where(u => u.PersonName == userName).OrderBy(u => u.Date).ToList();
            
            var number = 1;
            var ordersFromHistory = (from h in history
                                     let productsOrder = _jsonSerializer.Deserialize<ProductOrder[]>(h.Order)
                                     select new OrderFromHistory
                                     {
                                         Number = number++,
                                         Email = h.PersonEmail,
                                         Date = h.Date,
                                         ProductOrder = productsOrder,
                                         Total = h.Total,
                                         CultureName = h.Culture
                                     }).ToArray();

            return ordersFromHistory.Select(o => new OrderToGrid(o, _langSetter)).AsQueryable();
        }
    }
}