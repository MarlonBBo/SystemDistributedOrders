namespace SystemDistributedOrders.Application.Common.Exceptions;

public sealed class NotFoundException : Exception
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} com identificador '{key}' não foi encontrado.")
    {
        ResourceName = resourceName;
        Key = key;
    }

    public string ResourceName { get; }
    public object Key { get; }
}
