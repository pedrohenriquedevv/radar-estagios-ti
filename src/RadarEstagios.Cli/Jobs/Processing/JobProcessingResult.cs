using RadarEstagios.Jobs.Validation;

namespace RadarEstagios.Jobs.Processing;

public sealed class JobProcessingResult
{
    private readonly List<JobValidationError> _errors;
    private readonly IReadOnlyCollection<JobValidationError> _readOnlyErrors;

    public JobProcessingResult(
        JobPosting posting,
        JobValidationResult validation,
        string? normalizedLocation,
        IEnumerable<JobValidationError> errors)
    {
        Posting = posting;
        Validation = validation;
        NormalizedLocation = normalizedLocation;
        _errors = errors.ToList();
        _readOnlyErrors = _errors.AsReadOnly();
    }

    public JobPosting Posting { get; }

    public JobValidationResult Validation { get; }

    public string? NormalizedLocation { get; }

    public bool IsSuccessful => Validation.IsValid && NormalizedLocation is not null && _errors.Count == 0;

    public IReadOnlyCollection<JobValidationError> Errors => _readOnlyErrors;
}
