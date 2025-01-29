using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CondominiumAlerts.CrossCutting.Results;

public static class ResultsExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value); 

        var errors = result.Errors.Select(e => new { e.Message }).ToArray();
        return TypedResults.BadRequest(new { errors });
    }
}