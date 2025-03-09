

namespace CondominiumAlerts.Features.Features.Condominium.Get
{
    public record GetCondominiumResponce(
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
