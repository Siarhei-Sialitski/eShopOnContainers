using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Coupon.API.Infrastructure.Models;

public class LoyaltyMember
{
    [BsonIgnoreIfDefault]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public double Points { get; set; }
    public int TransactionsCount { get; set; }
}
