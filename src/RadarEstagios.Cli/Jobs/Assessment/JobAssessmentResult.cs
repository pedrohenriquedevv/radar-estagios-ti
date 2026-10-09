using RadarEstagios.Jobs.Analysis;
using RadarEstagios.Jobs.Processing;

namespace RadarEstagios.Jobs.Assessment;

public sealed class JobAssessmentResult
{
    public JobAssessmentResult(
        JobPosting posting,
        JobProcessingResult processing,
        JobOpportunityAnalysisResult analysis,
        JobClassificationResult classification,
        JobScoreResult score)
    {
        Posting = posting;
        Processing = processing;
        Analysis = analysis;
        Classification = classification;
        Score = score;
    }

    public JobPosting Posting { get; }

    public JobProcessingResult Processing { get; }

    public JobOpportunityAnalysisResult Analysis { get; }

    public JobCategory PrimaryCategory => Classification.PrimaryCategory;

    public IReadOnlyList<JobCategory> SecondaryCategories => Classification.SecondaryCategories;

    public IReadOnlyList<string> ClassificationReasons => Classification.ClassificationReasons;

    public int TotalScore => Score.TotalScore;

    public FitBand FitBand => Score.FitBand;

    public IReadOnlyList<JobScoreReason> ScoreReasons => Score.Reasons;

    public JobClassificationResult Classification { get; }

    public JobScoreResult Score { get; }
}
