namespace RadarEstagios.Jobs;

public sealed class JobPosting
{
    public required string Source { get; init; }

    public string? SourceId { get; init; }

    public required Uri SourceUrl { get; init; }

    public Uri? ApplicationUrl { get; init; }

    public required string OriginalTitle { get; init; }

    public required string OriginalCompany { get; init; }

    public required string OriginalLocation { get; init; }

    public string? Description { get; init; }
}