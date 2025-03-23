
using FluentValidation.Results;
using LightResults;

namespace CondominiumAlerts.Features.Extensions
{
    public static class FluentToLightResult
    {
         public static Result<T> ToLightResult<T>(this ValidationResult result, T request = default)
         {
            if (result.IsValid)
            {
                return Result<T>.Ok(request);
            }
               
            string errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
            return Result<T>.Fail(errors);
        }

        public static Result ToLightResult(this ValidationResult result)
        {
            if (result.IsValid)
            {
                return Result.Ok();
            }

            string errors = string.Join(", ", result.Errors.Select(e => e.ErrorMessage));
            return Result.Fail(errors);
        }
    }
}
