namespace RadarEstagios.Jobs.Assessment;

public sealed class JobClassificationResult
{
    public JobClassificationResult(
        JobCategory primaryCategory,
        IEnumerable<JobCategory> secondaryCategories,
        IEnumerable<string> classificationReasons)
    {
        PrimaryCategory = primaryCategory;
        SecondaryCategories = secondaryCategories
            .Where(category => category != primaryCategory)
            .Distinct()
            .ToArray()
            .AsReadOnly();
        ClassificationReasons = classificationReasons.ToArray().AsReadOnly();
    }

    public JobCategory PrimaryCategory { get; }

    public IReadOnlyList<JobCategory> SecondaryCategories { get; }

    public IReadOnlyList<string> ClassificationReasons { get; }
}
