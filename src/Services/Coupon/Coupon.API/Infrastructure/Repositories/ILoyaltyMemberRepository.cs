namespace Coupon.API.Infrastructure.Repositories;

public interface ILoyaltyMemberRepository
{
    Task<LoyaltyMember> FindLoyaltyMemberByUserId(string userId);
    Task CreateLoyaltyMemberByUserId(string userId, double points);
    Task UpdateLoyaltyMemberByUserId(string userId, double points, int transactionsCount);
}
