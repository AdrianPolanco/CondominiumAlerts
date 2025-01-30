using LightResults;

namespace CondominiumAlerts.CrossCutting.Results;

public class ValidationError: Error
{
        public string Message { get; }
        public IReadOnlyDictionary<string, object> Metadata { get; }

        public ValidationError(string message, Dictionary<string, object>? metadata = null): base(message: message, metadata: metadata)
        {
            Message = message;
            Metadata = metadata;
        }
}