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
    public class GridViewBasketAgent<T> : GridViewAgent<OrderItem, T>, IGridViewBasketManager<T>
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IDbProductRepository _dbProductRepository;
        private readonly IOrderRepository<T> _orderRepository;

        public GridViewBasketAgent(IStorageService<T> storageService,
            ICurrencyConverter currencyConverter, IDbProductRepository dbProductRepository,
            IOrderRepository<T> orderRepository)
            : base(storageService, currencyConverter)
        {
            _currencyConverter = currencyConverter;
            _dbProductRepository = dbProductRepository;
            _orderRepository = orderRepository;
        }

        public IQueryable<OrderItem> GetOrderItemsList(T repository, CultureInfo culture)
        {
            var products = _dbProductRepository.GetAll().ToArray();
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