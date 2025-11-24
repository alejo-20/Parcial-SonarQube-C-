using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Domain.Interfaces;

namespace Domain.Services
{
    /// <summary>
    /// Order service implementing business logic
    /// Follows Dependency Inversion Principle - depends on abstractions (ILogger)
    /// Follows Single Responsibility Principle - only handles order business logic
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly ILogger _logger;
        private readonly List<Order> _recentOrders;
        private const int MaxRecentOrders = 100;

        public OrderService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recentOrders = new List<Order>();
        }

        /// <summary>
        /// Creates a new order with proper validation
        /// </summary>
        public Order CreateOrder(string customerName, string productName, int quantity, decimal unitPrice)
        {
            try
            {
                var order = new Order(customerName, productName, quantity, unitPrice);
                
                // Keep track of recent orders (in-memory cache)
                _recentOrders.Add(order);
                
                // Maintain max size
                if (_recentOrders.Count > MaxRecentOrders)
                {
                    _recentOrders.RemoveAt(0);
                }

                _logger.Log($"Order created for customer: {customerName}, product: {productName}");
                
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create order for customer: {customerName}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets recent orders
        /// </summary>
        public IEnumerable<Order> GetRecentOrders()
        {
            return _recentOrders.AsReadOnly();
        }
    }
}
