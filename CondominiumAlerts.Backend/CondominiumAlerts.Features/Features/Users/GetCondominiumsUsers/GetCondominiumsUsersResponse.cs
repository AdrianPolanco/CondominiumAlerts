

namespace CondominiumAlerts.Features.Features.Users.GetCondominiumsUsers
{
    public record GetCondominiumsUsersResponse(
             string Id,
             string? ProfilePictureUrl,
             string FullName,
             string Email
        );
}
