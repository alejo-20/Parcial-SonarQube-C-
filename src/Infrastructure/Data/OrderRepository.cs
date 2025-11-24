using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Data
{
    /// <summary>
    /// Order repository implementation using ADO.NET with parameterized queries
    /// Follows Repository Pattern and prevents SQL injection
    /// </summary>
    public class OrderRepository : IOrderRepository, IDisposable
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private SqlConnection? _connection;

        public OrderRepository(string connectionString, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string cannot be empty", nameof(connectionString));

            _connectionString = connectionString;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Saves an order using parameterized queries (prevents SQL injection)
        /// </summary>
        public async Task<int> SaveAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            try
            {
                await EnsureConnectionAsync();

                // Using parameterized queries to prevent SQL injection
                var sql = @"INSERT INTO Orders (CustomerName, ProductName, Quantity, UnitPrice, TotalPrice, CreatedAt) 
                           VALUES (@CustomerName, @ProductName, @Quantity, @UnitPrice, @TotalPrice, @CreatedAt);
                           SELECT CAST(SCOPE_IDENTITY() as int);";

                using var command = new SqlCommand(sql, _connection);
                
                // Add parameters (prevents SQL injection)
                command.Parameters.AddWithValue("@CustomerName", order.CustomerName);
                command.Parameters.AddWithValue("@ProductName", order.ProductName);
                command.Parameters.AddWithValue("@Quantity", order.Quantity);
                command.Parameters.AddWithValue("@UnitPrice", order.UnitPrice);
                command.Parameters.AddWithValue("@TotalPrice", order.CalculateTotal());
                command.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);

                var newId = (int)await command.ExecuteScalarAsync();
                order.SetId(newId);

                _logger.Log($"Order {newId} saved successfully");
                
                return newId;
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Database error while saving order", ex);
                throw new InvalidOperationException("Failed to save order to database", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error while saving order", ex);
                throw;
            }
        }

        private async Task EnsureConnectionAsync()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
