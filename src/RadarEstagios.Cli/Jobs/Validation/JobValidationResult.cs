namespace RadarEstagios.Jobs.Validation;

public sealed class JobValidationResult
{
    private readonly List<JobValidationError> _errors = new();
    private readonly IReadOnlyCollection<JobValidationError> _readOnlyErrors;

    public JobValidationResult()
    {
        _readOnlyErrors = _errors.AsReadOnly();
    }

    public bool IsValid => _errors.Count == 0;

    public IReadOnlyCollection<JobValidationError> Errors => _readOnlyErrors;

    internal void AddError(JobValidationError error)
    {
        _errors.Add(error);
    }
}
