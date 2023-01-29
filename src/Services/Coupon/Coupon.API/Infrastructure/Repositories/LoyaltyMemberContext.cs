using Coupon.API.Infrastructure.Models;
using MongoDB.Driver;

namespace Coupon.API.Infrastructure.Repositories;

// TO DO Merge contexts
public class LoyaltyMemberContext
{
    private readonly IMongoDatabase _database = null;

    public LoyaltyMemberContext(IOptions<CouponSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);

        if (client is null)
        {
            throw new MongoConfigurationException("Cannot connect to the database. The connection string is not valid or the database is not accessible");
        }

        _database = client.GetDatabase(settings.Value.CouponMongoDatabase);
    }

    public IMongoCollection<LoyaltyMember> LoyaltyMembers => _database.GetCollection<LoyaltyMember>("LoyaltyMemberCollection");
}
