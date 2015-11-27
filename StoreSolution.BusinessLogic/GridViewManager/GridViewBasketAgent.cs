using System;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using StoreSolution.BusinessLogic.Currency.Contracts;
using StoreSolution.BusinessLogic.Database.Contracts;
using StoreSolution.BusinessLogic.GridViewManager.Contracts;
using StoreSolution.BusinessLogic.Models;
using StoreSolution.BusinessLogic.OrderRepository.Contracts;

namespace StoreSolution.BusinessLogic.GridViewManager
{
    public class GridViewBasketAgent : GridViewAgent<OrderItem, HttpSessionState>, IGridViewBasketManager<HttpSessionState>
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IEfProductRepository _efProductRepository;
        private readonly IOrderRepository<HttpSessionState> _orderRepository;

        public GridViewBasketAgent(IStorageService<HttpSessionState> storageService,
            ICurrencyConverter currencyConverter, IEfProductRepository efProductRepository,
            IOrderRepository<HttpSessionState> orderRepository)
            : base(storageService, currencyConverter)
        {
            _currencyConverter = currencyConverter;
            _efProductRepository = efProductRepository;
            _orderRepository = orderRepository;
        }

        public IQueryable<OrderItem> GetOrderItemsList(HttpSessionState repository, CultureInfo culture)
        {
            var products = _efProductRepository.Products.ToArray();
            var orders = _orderRepository.GetAll(repository);

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