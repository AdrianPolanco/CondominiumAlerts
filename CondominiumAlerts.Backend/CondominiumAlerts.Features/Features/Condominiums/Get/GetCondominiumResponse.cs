

namespace CondominiumAlerts.Features.Features.Condominiums.Get
{
    public record GetCondominiumResponse(
        Guid Id,
        string Name,
        string Addres,
        string ImageUrl,
        string Code,
        string Token,
        DateTime TokenExpirationDate,
        int AmountOfUsers
        );
}
