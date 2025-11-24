using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    /// <summary>
    /// Repository interface following Repository Pattern and Dependency Inversion Principle
    /// </summary>
    public interface IOrderRepository
    {
        Task<int> SaveAsync(Order order);
    }
}
