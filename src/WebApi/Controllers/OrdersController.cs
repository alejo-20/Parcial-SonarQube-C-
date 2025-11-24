using Microsoft.AspNetCore.Mvc;
using Application.UseCases;
using WebApi.DTOs;
using Domain.Interfaces;

namespace WebApi.Controllers
{
    /// <summary>
    /// Orders API Controller
    /// Follows REST principles and separation of concerns
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly CreateOrderUseCase _createOrderUseCase;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        public OrdersController(
            CreateOrderUseCase createOrderUseCase,
            IOrderService orderService,
            ILogger logger)
        {
            _createOrderUseCase = createOrderUseCase;
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _createOrderUseCase.ExecuteAsync(
                    request.CustomerName,
                    request.ProductName,
                    request.Quantity,
                    request.UnitPrice
                );

                var response = new OrderResponse
                {
                    Id = order.Id,
                    CustomerName = order.CustomerName,
                    ProductName = order.ProductName,
                    Quantity = order.Quantity,
                    UnitPrice = order.UnitPrice,
                    TotalPrice = order.CalculateTotal(),
                    CreatedAt = order.CreatedAt
                };

                return CreatedAtAction(nameof(CreateOrder), new { id = order.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Validation error creating order", ex);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating order", ex);
                return StatusCode(500, new { error = "An error occurred while creating the order" });
            }
        }

        /// <summary>
        /// Gets recent orders
        /// </summary>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
        public IActionResult GetRecentOrders()
        {
            try
            {
                var orders = _orderService.GetRecentOrders();
                var response = orders.Select(o => new OrderResponse
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    ProductName = o.ProductName,
                    Quantity = o.Quantity,
                    UnitPrice = o.UnitPrice,
                    TotalPrice = o.CalculateTotal(),
                    CreatedAt = o.CreatedAt
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving recent orders", ex);
                return StatusCode(500, new { error = "An error occurred while retrieving orders" });
            }
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("/health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
