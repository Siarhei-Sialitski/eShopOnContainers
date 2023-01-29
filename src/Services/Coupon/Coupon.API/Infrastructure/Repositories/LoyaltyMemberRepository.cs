using MongoDB.Driver;

namespace Coupon.API.Infrastructure.Repositories;

public class LoyaltyMemberRepository : ILoyaltyMemberRepository
{
    private readonly LoyaltyMemberContext _loyaltyMemberContext;

    public LoyaltyMemberRepository(LoyaltyMemberContext loyaltyMemberContext)
    {
        _loyaltyMemberContext = loyaltyMemberContext;
    }

    public async Task<LoyaltyMember> FindLoyaltyMemberById(string id)
    {
        var filter = Builders<LoyaltyMember>.Filter.Eq("Id", id);
        return await _loyaltyMemberContext.LoyaltyMembers.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateLoyaltyMemberById(string id, double points)
    {
        await _loyaltyMemberContext.LoyaltyMembers.InsertOneAsync(new LoyaltyMember() { Id = id, Points = points, TransactionsCount = 1});
    }

    public async Task UpdateLoyaltyMemberById(string id, double points, int transactionsCount)
    {
        var filter = Builders<LoyaltyMember>.Filter.Eq("Id", id);
        var update = Builders<LoyaltyMember>.Update
            .Set(m => m.Points, points)
            .Set(m => m.TransactionsCount, transactionsCount);
        await _loyaltyMemberContext.LoyaltyMembers.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false }) ;
    }
}
