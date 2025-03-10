using CondominiumAlerts.CrossCutting.CQRS.Interfaces;
using LightResults;

namespace CondominiumAlerts.Features.Features.Users;

public record GetUserDataQuery(string Id): IQuery<Result<GetUserDataResponse>>;