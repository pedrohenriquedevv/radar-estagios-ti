using RadarEstagios.Jobs.Validation;
using RadarEstagios.Locations;

namespace RadarEstagios.Jobs.Processing;

public static class JobPostingProcessor
{
    public static JobProcessingResult Process(JobPosting? posting)
    {
        var validation = JobPostingValidator.Validate(posting);

        if (!validation.IsValid)
        {
            return new JobProcessingResult(
                posting!,
                validation,
                normalizedLocation: null,
                validation.Errors);
        }

        var normalizedLocation = LocationNormalizer.Normalize(posting!.OriginalLocation);

        if (normalizedLocation is null)
        {
            var errors = new[]
            {
                new JobValidationError(
                    nameof(posting.OriginalLocation),
                    "OriginalLocation is not recognized.")
            };

            return new JobProcessingResult(
                posting,
                validation,
                normalizedLocation: null,
                errors);
        }

        return new JobProcessingResult(
            posting,
            validation,
            normalizedLocation,
            Array.Empty<JobValidationError>());
    }
}
