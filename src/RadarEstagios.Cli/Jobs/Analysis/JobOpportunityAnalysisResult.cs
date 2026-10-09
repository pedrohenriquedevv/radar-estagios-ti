namespace RadarEstagios.Jobs.Analysis;

public sealed class JobOpportunityAnalysisResult
{
    public JobOpportunityAnalysisResult(
        OpportunityType opportunityType,
        IEnumerable<string> technologies,
        IEnumerable<string> reasons)
    {
        OpportunityType = opportunityType;
        Technologies = technologies.ToArray().AsReadOnly();
        Reasons = reasons.ToArray().AsReadOnly();
    }

    public OpportunityType OpportunityType { get; }

    public IReadOnlyList<string> Technologies { get; }

    public IReadOnlyList<string> Reasons { get; }
}
