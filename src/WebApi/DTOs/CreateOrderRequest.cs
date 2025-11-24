using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    /// <summary>
    /// Data Transfer Object for creating orders
    /// Follows separation of concerns - API models separate from Domain entities
    /// </summary>
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Customer name must be between 1 and 100 characters")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 100 characters")]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
