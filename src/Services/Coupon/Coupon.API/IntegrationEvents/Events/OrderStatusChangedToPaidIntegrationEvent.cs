namespace Coupon.API.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public string UserId { get; }
    public int TotalSum { get; }

    public OrderStatusChangedToPaidIntegrationEvent(
        int orderId,
        string userId,
        int totalSum)
    {
        OrderId = orderId;
        UserId = userId;
        TotalSum = totalSum;
    }
}
