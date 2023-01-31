namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public string OrderStatus { get; }
    public string UserId { get; }
    public string BuyerName { get; }
    public IEnumerable<OrderStockItem> OrderStockItems { get; }
    public int TotalSum { get; }

    public OrderStatusChangedToPaidIntegrationEvent(int orderId,
        string orderStatus,
        string userId,
        string buyerName,
        IEnumerable<OrderStockItem> orderStockItems,
        int totalSum)
    {
        OrderId = orderId;
        OrderStockItems = orderStockItems;
        OrderStatus = orderStatus;
        UserId = userId;
        BuyerName = buyerName;
        TotalSum = totalSum;
    }
}

