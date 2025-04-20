
using FluentValidation.Results;
using LightResults;
using Microsoft.Extensions.Logging;


namespace CondominiumAlerts.Features.Extensions
{
    public static class FluentToLightResult
    {
        public static Result<T> ToLightResult<T>(this ValidationResult result, ILogger? logger = null, T response = default)
        {

            if (result.IsValid)
            {
                return Result<T>.Ok(response);
            }

            string errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));

            logger?.LogWarning("Validation failed {errors}",errors);
            return Result<T>.Fail(errors);
        }

        public static Result ToLightResult(this ValidationResult result, ILogger? logger = default)
        {
            if (result.IsValid)
            {
                return Result.Ok();
            }

            string errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));


            logger?.LogWarning("Validation failed {errors}", errors);
            return Result.Fail(errors);
        }
    }
}
