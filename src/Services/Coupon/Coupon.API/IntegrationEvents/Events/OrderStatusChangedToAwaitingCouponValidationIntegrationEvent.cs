﻿using Newtonsoft.Json;

namespace Coupon.API.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingCouponValidationIntegrationEvent : IntegrationEvent
{
    [JsonProperty]
    public int OrderId { get; private set; }

    [JsonProperty]
    public string OrderStatus { get; private set; }

    [JsonProperty]
    public string BuyerName { get; private set; }

    [JsonProperty]
    public string Code { get; private set; }
}
