namespace Coupon.API.Infrastructure.Repositories;

public interface ILoyaltyMemberRepository
{
    Task<LoyaltyMember> FindLoyaltyMemberById(string id);
    Task CreateLoyaltyMemberById(string id, double points);
    Task UpdateLoyaltyMemberById(string id, double points, int transactionsCount);
}
