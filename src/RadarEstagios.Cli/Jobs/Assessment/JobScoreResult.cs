namespace RadarEstagios.Jobs.Assessment;

public sealed class JobScoreResult
{
    public JobScoreResult(IEnumerable<JobScoreReason> reasons)
    {
        Reasons = reasons.ToArray().AsReadOnly();
    }

    public IReadOnlyList<JobScoreReason> Reasons { get; }

    public int TotalScore => Reasons.Sum(reason => reason.Points);

    public FitBand FitBand => TotalScore switch
    {
        >= 100 => FitBand.ImmediateApplication,
        >= 75 => FitBand.HighFit,
        >= 50 => FitBand.ModerateFit,
        >= 25 => FitBand.AdjacentOpportunity,
        _ => FitBand.LowFit
    };
}
