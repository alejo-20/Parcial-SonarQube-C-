using System;
using Domain.Interfaces;

namespace Infrastructure.Logging
{
    /// <summary>
    /// Logger implementation following Dependency Inversion Principle
    /// Implements ILogger interface defined in Domain
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly bool _enabled;

        public ConsoleLogger(bool enabled = true)
        {
            _enabled = enabled;
        }

        public void Log(string message)
        {
            if (!_enabled) return;
            
            Console.WriteLine($"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        public void LogError(string message, Exception? exception = null)
        {
            if (!_enabled) return;

            Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}");
            
            if (exception != null)
            {
                Console.WriteLine($"Exception: {exception.GetType().Name}");
                Console.WriteLine($"Message: {exception.Message}");
                Console.WriteLine($"StackTrace: {exception.StackTrace}");
            }
        }
    }
}
