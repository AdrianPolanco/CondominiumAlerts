

namespace CondominiumAlerts.Features.Features.Condominium.GetCondominiumsJoinedByUser
{
    public record GetCondominiumsJoinedByUserResponse(
        Guid Id,
        string Name,
        string Address,
        string ImageUrl);
}
