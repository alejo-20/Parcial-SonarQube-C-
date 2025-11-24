using System;

namespace Domain.Entities
{
    /// <summary>
    /// Order entity with proper encapsulation
    /// Following Single Responsibility Principle - only handles order data
    /// </summary>
    public class Order
    {
        // Using properties instead of public fields for encapsulation
        public int Id { get; private set; }
        public string CustomerName { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Constructor for creating new orders
        public Order(string customerName, string productName, int quantity, decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name cannot be empty", nameof(customerName));
            
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty", nameof(productName));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            CustomerName = customerName;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CreatedAt = DateTime.UtcNow;
        }

        // Constructor for loading from database
        public Order(int id, string customerName, string productName, int quantity, decimal unitPrice, DateTime createdAt)
            : this(customerName, productName, quantity, unitPrice)
        {
            Id = id;
            CreatedAt = createdAt;
        }

        public void SetId(int id)
        {
            if (Id == 0)
                Id = id;
        }

        /// <summary>
        /// Business logic: Calculate total price
        /// Removed logging dependency - follows Single Responsibility Principle
        /// </summary>
        public decimal CalculateTotal()
        {
            return Quantity * UnitPrice;
        }
    }
}
