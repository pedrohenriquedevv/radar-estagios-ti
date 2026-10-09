using RadarEstagios.Jobs;
using RadarEstagios.Jobs.Processing;

namespace RadarEstagios.Tests.Jobs.Processing;

public class JobPostingProcessorTests
{
    private static JobPosting CreateValidPosting(string location = "Catalão, GO")
    {
        return new JobPosting
        {
            Source = "SomeSource",
            SourceId = "123",
            SourceUrl = new Uri("https://example.com/job/123"),
            ApplicationUrl = new Uri("https://example.com/apply/123"),
            OriginalTitle = "Estagiário de TI",
            OriginalCompany = "Empresa X",
            OriginalLocation = location,
            Description = "Descrição da vaga"
        };
    }

    [Fact]
    public void Process_CatalaoPosting_SucceedsAndNormalizesLocation()
    {
        var posting = CreateValidPosting();

        var result = JobPostingProcessor.Process(posting);

        Assert.True(result.IsSuccessful);
        Assert.Equal("Catalão, GO", result.NormalizedLocation);
        Assert.Empty(result.Errors);
        Assert.Equal("Catalão, GO", posting.OriginalLocation);
    }

    [Fact]
    public void Process_UberlandiaPosting_SucceedsAndNormalizesLocation()
    {
        var posting = CreateValidPosting("Uberlândia, MG");

        var result = JobPostingProcessor.Process(posting);

        Assert.True(result.IsSuccessful);
        Assert.Equal("Uberlândia, MG", result.NormalizedLocation);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Process_UnaccentedLocation_NormalizesCorrectly()
    {
        var posting = CreateValidPosting("Catalao");

        var result = JobPostingProcessor.Process(posting);

        Assert.True(result.IsSuccessful);
        Assert.Equal("Catalão, GO", result.NormalizedLocation);
        Assert.Equal("Catalao", posting.OriginalLocation);
    }

    [Fact]
    public void Process_InvalidPosting_DoesNotSucceed()
    {
        var posting = new JobPosting
        {
            Source = "",
            SourceUrl = null!,
            OriginalTitle = "",
            OriginalCompany = "",
            OriginalLocation = ""
        };

        var result = JobPostingProcessor.Process(posting);

        Assert.False(result.IsSuccessful);
        Assert.Null(result.NormalizedLocation);
    }

    [Fact]
    public void Process_InvalidPosting_PreservesAllValidationErrors()
    {
        var posting = new JobPosting
        {
            Source = "",
            SourceUrl = null!,
            OriginalTitle = "",
            OriginalCompany = "",
            OriginalLocation = ""
        };

        var result = JobPostingProcessor.Process(posting);

        Assert.Equal(5, result.Errors.Count);
        Assert.Equal(5, result.Validation.Errors.Count);
        Assert.Equal(
            result.Validation.Errors.Select(error => error.Field),
            result.Errors.Select(error => error.Field));
    }

    [Fact]
    public void Process_UnknownLocation_DoesNotSucceedAndExplainsReason()
    {
        var posting = CreateValidPosting("São Paulo, SP");

        var result = JobPostingProcessor.Process(posting);

        Assert.False(result.IsSuccessful);
        Assert.Null(result.NormalizedLocation);
        var error = Assert.Single(result.Errors);
        Assert.Equal(nameof(posting.OriginalLocation), error.Field);
        Assert.Equal("OriginalLocation is not recognized.", error.Message);
    }

    [Fact]
    public void Process_PreservesOriginalLocation()
    {
        var posting = CreateValidPosting("Catalao/GO");

        var result = JobPostingProcessor.Process(posting);

        Assert.Equal("Catalao/GO", result.Posting.OriginalLocation);
        Assert.Equal("Catalao/GO", posting.OriginalLocation);
    }

    [Fact]
    public void Process_DoesNotModifyReceivedPosting()
    {
        var posting = CreateValidPosting("Uberlandia");
        var originalValues = new
        {
            posting.Source,
            posting.SourceId,
            posting.SourceUrl,
            posting.ApplicationUrl,
            posting.OriginalTitle,
            posting.OriginalCompany,
            posting.OriginalLocation,
            posting.Description
        };

        JobPostingProcessor.Process(posting);

        Assert.Equal(originalValues.Source, posting.Source);
        Assert.Equal(originalValues.SourceId, posting.SourceId);
        Assert.Equal(originalValues.SourceUrl, posting.SourceUrl);
        Assert.Equal(originalValues.ApplicationUrl, posting.ApplicationUrl);
        Assert.Equal(originalValues.OriginalTitle, posting.OriginalTitle);
        Assert.Equal(originalValues.OriginalCompany, posting.OriginalCompany);
        Assert.Equal(originalValues.OriginalLocation, posting.OriginalLocation);
        Assert.Equal(originalValues.Description, posting.Description);
    }

    [Fact]
    public void Process_SuccessfulResult_HasNoValidationErrors()
    {
        var result = JobPostingProcessor.Process(CreateValidPosting());

        Assert.True(result.Validation.IsValid);
        Assert.Empty(result.Validation.Errors);
        Assert.Empty(result.Errors);
    }
}
