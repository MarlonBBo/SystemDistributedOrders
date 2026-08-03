namespace SystemDistributedOrders.Application.Common.Exceptions;

public sealed class ValidationException : Exception
{
    public ValidationException(string propertyName, string errorMessage)
        : base(errorMessage)
    {
        Errors = new Dictionary<string, string[]>
        {
            [propertyName] = [errorMessage]
        };
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
