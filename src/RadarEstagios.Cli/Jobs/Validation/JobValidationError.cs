namespace RadarEstagios.Jobs.Validation;

public sealed class JobValidationError
{
    public string Field { get; }

    public string Message { get; }

    public JobValidationError(string field, string message)
    {
        Field = field ?? throw new global::System.ArgumentNullException(nameof(field));
        Message = message ?? throw new global::System.ArgumentNullException(nameof(message));
    }
}
