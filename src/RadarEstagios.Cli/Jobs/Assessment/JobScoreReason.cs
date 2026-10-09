namespace RadarEstagios.Jobs.Assessment;

public sealed class JobScoreReason
{
    public JobScoreReason(string description, int points)
    {
        Description = description;
        Points = points;
    }

    public string Description { get; }

    public int Points { get; }
}
