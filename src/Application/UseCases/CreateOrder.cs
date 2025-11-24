using System;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.UseCases
{
    /// <summary>
    /// Use case for creating orders
    /// Follows Clean Architecture: orchestrates the business logic without knowing implementation details
    /// Depends on abstractions (IOrderService, IOrderRepository, ILogger)
    /// </summary>
    public class CreateOrderUseCase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;

        public CreateOrderUseCase(
            IOrderService orderService, 
            IOrderRepository orderRepository,
            ILogger logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executes the create order use case
        /// </summary>
        public async Task<Order> ExecuteAsync(string customerName, string productName, int quantity, decimal unitPrice)
        {
            try
            {
                _logger.Log("CreateOrderUseCase starting");

                // Create order using domain service
                var order = _orderService.CreateOrder(customerName, productName, quantity, unitPrice);

                // Persist to database
                await _orderRepository.SaveAsync(order);

                _logger.Log($"Order {order.Id} created successfully");

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to execute CreateOrderUseCase", ex);
                throw;
            }
        }
    }
}

