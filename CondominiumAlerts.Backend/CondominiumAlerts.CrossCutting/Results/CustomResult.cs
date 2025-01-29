using FluentResults;
using FluentValidation.Results;

namespace CondominiumAlerts.CrossCutting.Results;

public class CustomResult<T> : Result<T>
{
    public string CustomMessage { get; private set; }
    public List<ValidationFailure> ValidationFailures { get; private set; } // Lista de errores de validación de FluentValidation

    // Constructor privado: Solo se puede crear a través de los métodos estáticos.
    private CustomResult(bool isSuccess, T value, string error, string customMessage, List<ValidationFailure> validationFailures)
        : base()
    {
        CustomMessage = customMessage;
        ValidationFailures = validationFailures ?? new List<ValidationFailure>();

        if (!isSuccess && !string.IsNullOrEmpty(error))
        {
            // Si el resultado es fallido, agregar un error a la colección de errores
            this.Errors.Add(new Error(error));
        }
        
    }

    // Crear un resultado exitoso
    public static CustomResult<T> Success(T value, string customMessage = "", List<ValidationFailure> validationFailures = null)
    {
        return new CustomResult<T>(true, value, null, customMessage, validationFailures);
    }

    // Crear un resultado fallido
    public static CustomResult<T> Failure(string error, string customMessage = "", List<ValidationFailure> validationFailures = null)
    {
        return new CustomResult<T>(false, default, error, customMessage, validationFailures);
    }

    // Agregar errores de validación a un resultado fallido
    public CustomResult<T> WithValidationFailures(List<ValidationFailure> validationFailures)
    {
        ValidationFailures.AddRange(validationFailures);
        return this;
    }
}
