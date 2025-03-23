using Carter;
using CondominiumAlerts.Features.Features.PriorityLevels.Add;
using CondominiumAlerts.Features.Features.PriorityLevels.Get;
using LightResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CondominiumAlerts.Api.Endpoints
{
    public class PriorityLevelsModule : ICarterModule
    {
        private const string mainPath = "/priorityLevels/";
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(GetEndpointPattern("get"), async (ISender sender, [AsParameters] GetPriorityLevelsQuery request, CancellationToken cancellationToken) =>
            {
                Result<GetPriorityLevelResponce> result = await sender.Send(request, cancellationToken);

                if (result.IsFailed)
                {
                    return Results.BadRequest(result);
                }

                var responce = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value,
                };

                return Results.NotFound(responce);

            });

            app.MapPost(GetEndpointPattern("add"), async (ISender sender, [FromBody] AddPriorityLevelCommand request, CancellationToken cancellationToken) =>
            {
                Result<AddPriorityLevelResponse> result = await sender.Send(request, cancellationToken);

                if (result.IsFailed)
                {
                    return Results.BadRequest(result);
                }

                var responce = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value,
                };

                return Results.NotFound(responce);

            });
        }

        #region private Methods 
        private string GetEndpointPattern(string name)
        {
            return mainPath + name;
        }

        #endregion
    }
}
