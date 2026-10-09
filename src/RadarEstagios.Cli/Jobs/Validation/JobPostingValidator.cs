using RadarEstagios.Jobs.Validation;

namespace RadarEstagios.Jobs.Validation;

public static class JobPostingValidator
{
    public static JobValidationResult Validate(RadarEstagios.Jobs.JobPosting? posting)
    {
        var result = new JobValidationResult();

        if (posting is null)
        {
            result.AddError(new JobValidationError("JobPosting", "JobPosting instance is null."));
            return result;
        }

        if (string.IsNullOrWhiteSpace(posting.Source))
        {
            result.AddError(new JobValidationError(nameof(posting.Source), "Source is required and cannot be empty."));
        }

        if (posting.SourceUrl is null)
        {
            result.AddError(new JobValidationError(nameof(posting.SourceUrl), "SourceUrl is required and cannot be null."));
        }

        if (string.IsNullOrWhiteSpace(posting.OriginalTitle))
        {
            result.AddError(new JobValidationError(nameof(posting.OriginalTitle), "OriginalTitle is required and cannot be empty."));
        }

        if (string.IsNullOrWhiteSpace(posting.OriginalCompany))
        {
            result.AddError(new JobValidationError(nameof(posting.OriginalCompany), "OriginalCompany is required and cannot be empty."));
        }

        if (string.IsNullOrWhiteSpace(posting.OriginalLocation))
        {
            result.AddError(new JobValidationError(nameof(posting.OriginalLocation), "OriginalLocation is required and cannot be empty."));
        }

        return result;
    }
}
