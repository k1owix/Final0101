namespace WebApi.Contracts;

public sealed record CreatedOrderResponse(
    int OrderId,
    int OrderNumber,
    DateTime OrderDate,
    string ReceiptCode,
    string StatusName);
