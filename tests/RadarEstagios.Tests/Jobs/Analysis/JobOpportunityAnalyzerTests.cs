using RadarEstagios.Jobs;
using RadarEstagios.Jobs.Analysis;

namespace RadarEstagios.Tests.Jobs.Analysis;

public class JobOpportunityAnalyzerTests
{
    private static JobPosting CreatePosting(string title, string? description = null)
    {
        return new JobPosting
        {
            Source = "Source",
            SourceUrl = new Uri("https://example.com/job"),
            OriginalTitle = title,
            OriginalCompany = "Company",
            OriginalLocation = "Catalão",
            Description = description
        };
    }

    [Fact]
    public void Analyze_InternshipProgramHasPriorityOverInternship()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Programa de estágio"));

        Assert.Equal(OpportunityType.InternshipProgram, result.OpportunityType);
    }

    [Fact]
    public void Analyze_InternshipInTitle_ReturnsInternship()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Estágio em tecnologia"));

        Assert.Equal(OpportunityType.Internship, result.OpportunityType);
    }

    [Fact]
    public void Analyze_InternshipOnlyInDescription_ReturnsInternship()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Desenvolvedor", "Vaga para estagiária."));

        Assert.Equal(OpportunityType.Internship, result.OpportunityType);
    }

    [Fact]
    public void Analyze_Trainee_ReturnsTrainee()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Programa trainee"));

        Assert.Equal(OpportunityType.Trainee, result.OpportunityType);
    }

    [Fact]
    public void Analyze_Apprentice_ReturnsApprentice()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Jovem aprendiz"));

        Assert.Equal(OpportunityType.Apprentice, result.OpportunityType);
    }

    [Fact]
    public void Analyze_EntryPosition_ReturnsEntryLevel()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Posição de entrada"));

        Assert.Equal(OpportunityType.EntryLevel, result.OpportunityType);
    }

    [Fact]
    public void Analyze_WithoutKnownSignal_ReturnsUnknown()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Desenvolvedor de software"));

        Assert.Equal(OpportunityType.Unknown, result.OpportunityType);
    }

    [Fact]
    public void Analyze_IgnoresAccentsCaseAndExtraSpaces()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("  ESTÁGIO   em tecnologia  "));

        Assert.Equal(OpportunityType.Internship, result.OpportunityType);
    }

    [Fact]
    public void Analyze_ExtractsMultipleTechnologies()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Estágio C# e Python", "Usamos Docker, SQL e GitHub."));

        Assert.Contains("C#", result.Technologies);
        Assert.Contains("Python", result.Technologies);
        Assert.Contains("Docker", result.Technologies);
        Assert.Contains("SQL", result.Technologies);
        Assert.Contains("GitHub", result.Technologies);
    }

    [Fact]
    public void Analyze_DuplicateTechnologiesAreReturnedOnce()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Python Python", "Python e PYTHON."));

        Assert.Equal(1, result.Technologies.Count(technology => technology == "Python"));
    }

    [Fact]
    public void Analyze_GoiasDoesNotRecognizeGo()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Oportunidade em Goiás"));

        Assert.DoesNotContain("Go", result.Technologies);
    }

    [Fact]
    public void Analyze_GolangRecognizesGo()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Desenvolvimento com Golang"));

        Assert.Contains("Go", result.Technologies);
    }

    [Fact]
    public void Analyze_JavaScriptDoesNotRecognizeJavaAccidentally()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Desenvolvimento JavaScript"));

        Assert.Contains("JavaScript", result.Technologies);
        Assert.DoesNotContain("Java", result.Technologies);
    }

    [Fact]
    public void Analyze_CIsNotFoundInsideCommonWords()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Experiência com comunicação e tecnologia"));

        Assert.DoesNotContain("C", result.Technologies);
    }

    [Fact]
    public void Analyze_CSharpCppAndCSeparately()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("C#, C++ e C"));

        Assert.Contains("C#", result.Technologies);
        Assert.Contains("C++", result.Technologies);
        Assert.Contains("C", result.Technologies);
    }

    [Fact]
    public void Analyze_GitHubAndGitLabHaveNoGitDuplicate()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("GitHub e GitLab"));

        Assert.Contains("GitHub", result.Technologies);
        Assert.Contains("GitLab", result.Technologies);
        Assert.DoesNotContain("Git", result.Technologies);
        Assert.Equal(result.Technologies.Count, result.Technologies.Distinct().Count());
    }

    [Fact]
    public void Analyze_SapVariantsAreHandledIndependently()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("SAP, SAP PM e SAP HANA"));

        Assert.Contains("SAP", result.Technologies);
        Assert.Contains("SAP PM", result.Technologies);
        Assert.Contains("SAP HANA", result.Technologies);
        Assert.Equal(1, result.Technologies.Count(technology => technology == "SAP"));
    }

    [Fact]
    public void Analyze_UsesTitleAndDescriptionTogether()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Oportunidade", "Experiência com TypeScript."));

        Assert.Contains("TypeScript", result.Technologies);
    }

    [Fact]
    public void Analyze_WithoutTechnologies_ReturnsEmptyCollection()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Estágio administrativo"));

        Assert.Empty(result.Technologies);
    }

    [Fact]
    public void Analyze_ReturnsExplainableReasons()
    {
        var result = JobOpportunityAnalyzer.Analyze(CreatePosting("Trainee em Python"));

        Assert.Contains(result.Reasons, reason => reason.Contains("trainee", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Analyze_DoesNotModifyOriginalPosting()
    {
        var posting = CreatePosting("  ESTÁGIO  ", "Golang e C#");
        var title = posting.OriginalTitle;
        var description = posting.Description;

        JobOpportunityAnalyzer.Analyze(posting);

        Assert.Equal(title, posting.OriginalTitle);
        Assert.Equal(description, posting.Description);
    }
}
