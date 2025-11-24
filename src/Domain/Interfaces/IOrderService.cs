using System.Collections.Generic;
using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Service interface for order operations
    /// </summary>
    public interface IOrderService
    {
        Order CreateOrder(string customerName, string productName, int quantity, decimal unitPrice);
        IEnumerable<Order> GetRecentOrders();
    }
}
