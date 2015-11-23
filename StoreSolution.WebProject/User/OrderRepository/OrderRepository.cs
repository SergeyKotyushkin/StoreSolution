﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using StoreSolution.DatabaseProject.Model;
using StoreSolution.WebProject.Currency.Contracts;
using StoreSolution.WebProject.Model;
using StoreSolution.WebProject.User.OrderRepository.Contracts;

namespace StoreSolution.WebProject.User.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ICurrencyConverter _currencyConverter;

        public OrderRepository(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

        public void Add(HttpSessionState sessionState, int id)
        {
            var orders = GetAll(sessionState);

            var order = orders.Find(o => o.Id == id);
            if (order == null) orders.Add(new Order { Id = id, Count = 1 });
            else order.Count++;

            SetAll(sessionState, orders);
        }

        public void Remove(HttpSessionState sessionState, int id)
        {
            var orders = GetAll(sessionState);

            var order = orders.Find(o => o.Id == id);
            if (order == null || order.Count == 0) return;
            if (order.Count == 1) orders.Remove(order);
            else order.Count--;

            SetAll(sessionState, orders);
        }

        public List<Order> GetAll(HttpSessionState sessionState)
        {
            return sessionState["CurrentOrder"] as List<Order> ?? new List<Order>();
        }

        private static void SetAll(HttpSessionState sessionState, IEnumerable<Order> orders)
        {
            sessionState["CurrentOrder"] = orders;
        }
    }
}