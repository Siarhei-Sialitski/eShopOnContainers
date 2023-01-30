using MongoDB.Driver;

namespace Coupon.API.Infrastructure.Repositories;

public class LoyaltyMemberRepository : ILoyaltyMemberRepository
{
    private readonly CouponContext _couponContext;

    public LoyaltyMemberRepository(CouponContext couponContext)
    {
        _couponContext = couponContext;
    }

    public async Task<LoyaltyMember> FindLoyaltyMemberByUserId(string userId)
    {
        var filter = Builders<LoyaltyMember>.Filter.Eq("UserId", userId);
        return await _couponContext.LoyaltyMembers.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateLoyaltyMemberByUserId(string userId, double points)
    {
        await _couponContext.LoyaltyMembers.InsertOneAsync(new LoyaltyMember() { UserId = userId, Points = points, TransactionsCount = 1});
    }

    public async Task UpdateLoyaltyMemberByUserId(string userId, double points, int transactionsCount)
    {
        var filter = Builders<LoyaltyMember>.Filter.Eq("UserId", userId);
        var update = Builders<LoyaltyMember>.Update
            .Set(m => m.Points, points)
            .Set(m => m.TransactionsCount, transactionsCount);
        await _couponContext.LoyaltyMembers.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = false }) ;
    }
}
