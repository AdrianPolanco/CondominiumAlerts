

namespace CondominiumAlerts.Features.Features.Condominiums.GetCondominiumsJoinedByUser
{
    public record GetCondominiumsJoinedByUserResponse(
        Guid Id,
        string Name,
        string Address,
        string ImageUrl);
}
