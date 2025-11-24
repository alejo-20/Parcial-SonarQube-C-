namespace Domain.Interfaces
{
    /// <summary>
    /// Logger abstraction following Dependency Inversion Principle
    /// Domain defines the contract, Infrastructure provides implementation
    /// </summary>
    public interface ILogger
    {
        void Log(string message);
        void LogError(string message, System.Exception exception = null);
    }
}
