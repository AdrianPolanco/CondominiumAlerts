using Carter;
using CondominiumAlerts.Features.Features.PriorityLevels.Add;
using CondominiumAlerts.Features.Features.PriorityLevels.Delete;
using CondominiumAlerts.Features.Features.PriorityLevels.Get;
using CondominiumAlerts.Features.Features.PriorityLevels.GetById;
using CondominiumAlerts.Features.Features.PriorityLevels.Update;
using LightResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CondominiumAlerts.Api.Endpoints
{
    public class PriorityLevelsModule : ICarterModule
    {
        private const string mainPath = "/priorityLevels";
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet(GetEndpointPattern(""), async (ISender sender, [AsParameters] GetPriorityLevelsQuery request, CancellationToken cancellationToken) =>
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

            app.MapPost(GetEndpointPattern("/add"), async (ISender sender, [FromBody] AddPriorityLevelCommand request, CancellationToken cancellationToken) =>
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

                return Results.Ok(responce);

            });

            app.MapPut(GetEndpointPattern("/update"), async (ISender sender, [FromBody] UpdatePriorityLevelCommand request) =>
            {
                Result<UpdatePriorityLevelResponse> result = await sender.Send(request);

                if (result.IsFailed)
                {
                    return Results.BadRequest(result);
                }

                var responce = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value,
                };

                return Results.Ok(responce);

            });

            //TODO : Make the condominiumId come from the request claims in the user token
            app.MapDelete(GetEndpointPattern("/delete"), async (ISender sender, [FromBody] DeletePriorityLevelCommand request, CancellationToken cancellationToken) =>
            {
                Result<DeletePriorityLevelResponse> result = await sender.Send(request, cancellationToken);

                if (result.IsFailed)
                {
                    return Results.BadRequest(result);
                }

                var response = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value,
                };

                return Results.Ok(response);

            });

            //TODO : Make the condominiumId come from the request claims in the user token
            app.MapGet(GetEndpointPattern("/id"), async (ISender sender, [AsParameters] GetByIdPriorityLevelQuery request, CancellationToken cancellationToken) =>
            {
                Result<GetByIdPriorityLevelResponse> result = await sender.Send(request, cancellationToken);

                if (result.IsFailed)
                {
                    return Results.BadRequest(result);
                }

                var response = new
                {
                    IsSuccess = result.IsSuccess,
                    Data = result.Value,
                };

                return Results.Ok(response);

            });
        }

        #region private Methods 
        private string GetEndpointPattern(string name = "")
        {
            return mainPath + name;
        }

        #endregion
    }
}
