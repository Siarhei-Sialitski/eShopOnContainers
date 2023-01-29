using Coupon.API.IntegrationEvents.Events;

namespace Coupon.API.IntegrationEvents.EventHandlers;

public class OrderStatusChangedToPaidIntegrationEventHandler : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    private readonly ILoyaltyMemberRepository _loyaltyMemberRepository;
    private readonly ILoggerFactory _logger;

    public OrderStatusChangedToPaidIntegrationEventHandler(
        ILoyaltyMemberRepository loyaltyMemberRepository, ILoggerFactory logger)
    {
        _loyaltyMemberRepository = loyaltyMemberRepository;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        var loyaltyMember = await _loyaltyMemberRepository.FindLoyaltyMemberById(@event.UserId);
        if (loyaltyMember == null)
        {
            await _loyaltyMemberRepository.CreateLoyaltyMemberById(@event.UserId, @event.TotalSum * 0.1);
            _logger.CreateLogger<OrderStatusChangedToPaidIntegrationEvent>()
            .LogTrace("Loyalty member with Id: {UserId} created with {points} points ",
                @event.UserId, @event.TotalSum * 0.1);
        }
        else
        {
            await _loyaltyMemberRepository.UpdateLoyaltyMemberById(@event.UserId, @event.TotalSum * 0.1 + loyaltyMember.Points, loyaltyMember.TransactionsCount++);
            _logger.CreateLogger<OrderStatusChangedToPaidIntegrationEvent>()
            .LogTrace("Loyalty member with Id: {UserId} updated with {points} points and transactions count {transactionsCount}",
                @event.UserId, @event.TotalSum * 0.1 + loyaltyMember.Points, loyaltyMember.TransactionsCount);
        }
    }
}
