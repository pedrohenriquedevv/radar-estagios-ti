using RadarEstagios.Jobs;
using RadarEstagios.Jobs.Validation;

namespace RadarEstagios.Tests.Jobs.Validation;

public class JobPostingValidatorTests
{
    private static JobPosting ValidJobPosting()
    {
        return new JobPosting
        {
            Source = "SomeSource",
            SourceId = "123",
            SourceUrl = new Uri("https://example.com/job/123"),
            ApplicationUrl = new Uri("https://example.com/apply/123"),
            OriginalTitle = "Estagiário de TI",
            OriginalCompany = "Empresa X",
            OriginalLocation = "Catalão, GO",
            Description = "Descrição da vaga"
        };
    }

    private static JobPosting BuildPosting(
        string? source = null,
        string? sourceId = null,
        Uri? sourceUrl = null,
        Uri? applicationUrl = null,
        string? originalTitle = null,
        string? originalCompany = null,
        string? originalLocation = null,
        string? description = null)
    {
        var basePosting = ValidJobPosting();

        return new JobPosting
        {
            Source = source ?? basePosting.Source,
            SourceId = sourceId ?? basePosting.SourceId,
            SourceUrl = sourceUrl ?? basePosting.SourceUrl,
            ApplicationUrl = applicationUrl ?? basePosting.ApplicationUrl,
            OriginalTitle = originalTitle ?? basePosting.OriginalTitle,
            OriginalCompany = originalCompany ?? basePosting.OriginalCompany,
            OriginalLocation = originalLocation ?? basePosting.OriginalLocation,
            Description = description ?? basePosting.Description
        };
    }

    private static JobPosting BuildPostingWithSourceUrlNull(
        string? source = null,
        string? sourceId = null,
        Uri? applicationUrl = null,
        string? originalTitle = null,
        string? originalCompany = null,
        string? originalLocation = null,
        string? description = null)
    {
        var p = BuildPosting(source, sourceId, new Uri("https://example.com/job/123"), applicationUrl, originalTitle, originalCompany, originalLocation, description);
        // create instance with SourceUrl set to null by constructing anew
        return new JobPosting
        {
            Source = p.Source,
            SourceId = p.SourceId,
            SourceUrl = null!,
            ApplicationUrl = p.ApplicationUrl,
            OriginalTitle = p.OriginalTitle,
            OriginalCompany = p.OriginalCompany,
            OriginalLocation = p.OriginalLocation,
            Description = p.Description
        };
    }

    private static JobPosting BuildPostingWithDescriptionNull()
    {
        var p = BuildPosting();
        return new JobPosting
        {
            Source = p.Source,
            SourceId = p.SourceId,
            SourceUrl = p.SourceUrl,
            ApplicationUrl = p.ApplicationUrl,
            OriginalTitle = p.OriginalTitle,
            OriginalCompany = p.OriginalCompany,
            OriginalLocation = p.OriginalLocation,
            Description = null
        };
    }

    private static JobPosting BuildPostingWithSourceIdNull()
    {
        var p = BuildPosting();
        return new JobPosting
        {
            Source = p.Source,
            SourceId = null,
            SourceUrl = p.SourceUrl,
            ApplicationUrl = p.ApplicationUrl,
            OriginalTitle = p.OriginalTitle,
            OriginalCompany = p.OriginalCompany,
            OriginalLocation = p.OriginalLocation,
            Description = p.Description
        };
    }

    private static JobPosting BuildPostingWithApplicationUrlNull()
    {
        var p = BuildPosting();
        return new JobPosting
        {
            Source = p.Source,
            SourceId = p.SourceId,
            SourceUrl = p.SourceUrl,
            ApplicationUrl = null,
            OriginalTitle = p.OriginalTitle,
            OriginalCompany = p.OriginalCompany,
            OriginalLocation = p.OriginalLocation,
            Description = p.Description
        };
    }

    [Fact]
    public void Validate_ValidJobPosting_IsValidTrueAndNoErrors()
    {
        var posting = ValidJobPosting();

        var result = JobPostingValidator.Validate(posting);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_SourceEmpty_ProducesError(string source)
    {
        var posting = BuildPosting(source: source);

        var result = JobPostingValidator.Validate(posting);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.Source));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_OriginalTitleEmpty_ProducesError(string title)
    {
        var posting = BuildPosting(originalTitle: title);

        var result = JobPostingValidator.Validate(posting);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.OriginalTitle));
    }

    [Fact]
    public void Validate_OriginalCompanyEmpty_ProducesError()
    {
        var posting = BuildPosting(originalCompany: "");

        var result = JobPostingValidator.Validate(posting);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.OriginalCompany));
    }

    [Fact]
    public void Validate_OriginalLocationEmpty_ProducesError()
    {
        var posting = BuildPosting(originalLocation: "");

        var result = JobPostingValidator.Validate(posting);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.OriginalLocation));
    }

    [Fact]
    public void Validate_SourceUrlNull_ProducesError()
    {
        var posting = BuildPostingWithSourceUrlNull();

        var result = JobPostingValidator.Validate(posting);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.SourceUrl));
    }

    [Fact]
    public void Validate_DescriptionNull_IsStillValid()
    {
        var posting = BuildPostingWithDescriptionNull();

        var result = JobPostingValidator.Validate(posting);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_SourceIdNull_IsStillValid()
    {
        var posting = BuildPostingWithSourceIdNull();

        var result = JobPostingValidator.Validate(posting);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_ApplicationUrlNull_IsStillValid()
    {
        var posting = BuildPostingWithApplicationUrlNull();

        var result = JobPostingValidator.Validate(posting);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_MultipleInvalidFields_ReturnsAllErrors()
    {
        var posting = BuildPostingWithSourceUrlNull(
            source: " ",
            originalTitle: "",
            originalCompany: "",
            originalLocation: "");

        var result = JobPostingValidator.Validate(posting);

        Assert.False(result.IsValid);
        Assert.Equal(5, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.Source));
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.OriginalTitle));
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.OriginalCompany));
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.OriginalLocation));
        Assert.Contains(result.Errors, e => e.Field == nameof(posting.SourceUrl));
    }

    [Fact]
    public void Validate_DoesNotModifyJobPosting()
    {
        var posting = ValidJobPosting();
        var copy = BuildPosting(
            source: posting.Source,
            sourceId: posting.SourceId,
            sourceUrl: posting.SourceUrl,
            applicationUrl: posting.ApplicationUrl,
            originalTitle: posting.OriginalTitle,
            originalCompany: posting.OriginalCompany,
            originalLocation: posting.OriginalLocation,
            description: posting.Description);

        var result = JobPostingValidator.Validate(posting);

        Assert.Equal(copy.Source, posting.Source);
        Assert.Equal(copy.SourceId, posting.SourceId);
        Assert.Equal(copy.SourceUrl, posting.SourceUrl);
        Assert.Equal(copy.ApplicationUrl, posting.ApplicationUrl);
        Assert.Equal(copy.OriginalTitle, posting.OriginalTitle);
        Assert.Equal(copy.OriginalCompany, posting.OriginalCompany);
        Assert.Equal(copy.OriginalLocation, posting.OriginalLocation);
        Assert.Equal(copy.Description, posting.Description);
    }
}
