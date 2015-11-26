using System;
using System.Globalization;
using System.Linq;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewBasketAgent : GridViewAgent<OrderItem>, IGridViewBasketManager
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IEfProductRepository _efProductRepository;
        private readonly IOrderSessionRepository _orderSessionRepository;

        public GridViewBasketAgent(IGridViewPageIndexService gridViewPageIndexService,
            ICurrencyConverter currencyConverter, IEfProductRepository efProductRepository,
            IOrderSessionRepository orderSessionRepository)
            : base(gridViewPageIndexService, currencyConverter)
        {
            _currencyConverter = currencyConverter;
            _efProductRepository = efProductRepository;
            _orderSessionRepository = orderSessionRepository;
        }

        public IQueryable<OrderItem> GetOrderItemsList(object repository, CultureInfo culture)
        {
            var products = _efProductRepository.Products.ToArray();
            var orders = _orderSessionRepository.GetAll(repository);

            var rate = _currencyConverter.GetRate(new CultureInfo("ru-Ru"), culture, DateTime.Now);
            return products.Join(orders, p => p.Id, q => q.Id, (p, q) => new OrderItem
            {
                Name = p.Name,
                Price = _currencyConverter.ConvertByRate(p.Price, rate),
                Count = q.Count,
                Total = (q.Count * _currencyConverter.ConvertByRate(p.Price, rate))
            }).AsQueryable();
        }
    }
}