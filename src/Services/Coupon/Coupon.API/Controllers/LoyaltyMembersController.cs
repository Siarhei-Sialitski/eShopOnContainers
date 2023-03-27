namespace Coupon.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class LoyaltyMembersController : ControllerBase
{
    private readonly ILoyaltyMemberRepository _loyaltyMemberRepository;

    public LoyaltyMembersController(ILoyaltyMemberRepository loyaltyMemberRepository)
    {
        _loyaltyMemberRepository = loyaltyMemberRepository;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Infrastructure.Models.LoyaltyMember>> GetLoyaltyMemberByIdAsync(string id)
    {
        var loyaltyMember = await _loyaltyMemberRepository.FindLoyaltyMemberByUserId(id);

        if (loyaltyMember is null)
        {
            return NotFound();
        }

        return loyaltyMember;
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Infrastructure.Models.LoyaltyMember>> UpdateLoyaltyMember([FromBody] LoyaltyMember loyaltyMember)
    {
        var loyaltyMemberFromDb = await _loyaltyMemberRepository.FindLoyaltyMemberByUserId(loyaltyMember.UserId);

        loyaltyMemberFromDb.Points = loyaltyMember.Points;
        await _loyaltyMemberRepository.UpdateLoyaltyMemberByUserId(loyaltyMemberFromDb.UserId, loyaltyMemberFromDb.Points, loyaltyMember.TransactionsCount++);

        return loyaltyMember;
    }
}
