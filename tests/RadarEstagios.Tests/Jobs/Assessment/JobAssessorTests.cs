using RadarEstagios.Jobs;
using RadarEstagios.Jobs.Analysis;
using RadarEstagios.Jobs.Assessment;
using RadarEstagios.Jobs.Processing;

namespace RadarEstagios.Tests.Jobs.Assessment;

public class JobAssessorTests
{
    private static JobPosting CreatePosting(string title, string? description = "Descrição tecnológica")
    {
        return new JobPosting
        {
            Source = "Source",
            SourceUrl = new Uri("https://example.com/job"),
            OriginalTitle = title,
            OriginalCompany = "Company",
            OriginalLocation = "Catalão, GO",
            Description = description
        };
    }

    private static JobAssessmentResult Assess(
        string title,
        string description = "Descrição tecnológica",
        string location = "Catalão, GO")
    {
        var posting = new JobPosting
        {
            Source = "Source",
            SourceUrl = new Uri("https://example.com/job"),
            OriginalTitle = title,
            OriginalCompany = "Company",
            OriginalLocation = location,
            Description = description
        };
        var processing = JobPostingProcessor.Process(posting);
        var analysis = JobOpportunityAnalyzer.Analyze(posting);
        return JobAssessor.Assess(posting, processing, analysis);
    }

    [Fact]
    public void Assess_Backend_ReturnsBackendCategory()
    {
        Assert.Equal(JobCategory.BackendDevelopment, Assess("Backend API C#").PrimaryCategory);
    }

    [Fact]
    public void Assess_Frontend_ReturnsFrontendCategory()
    {
        Assert.Equal(JobCategory.FrontendDevelopment, Assess("Frontend Vue.js TypeScript").PrimaryCategory);
    }

    [Fact]
    public void Assess_FullStackWithBothSides_ReturnsFullStack()
    {
        Assert.Equal(JobCategory.FullStackDevelopment, Assess("Full stack", "Backend API e frontend com TypeScript").PrimaryCategory);
    }

    [Fact]
    public void Assess_GeneralDevelopment_ReturnsGeneralCategory()
    {
        Assert.Equal(JobCategory.GeneralSoftwareDevelopment, Assess("Desenvolvimento de software").PrimaryCategory);
    }

    [Fact]
    public void Assess_Automation_ReturnsAutomationCategory()
    {
        Assert.Equal(JobCategory.ProcessAutomationAndRpa, Assess("Automação de processos RPA").PrimaryCategory);
    }

    [Fact]
    public void Assess_DataAndBi_ReturnsDataBiCategory()
    {
        Assert.Equal(JobCategory.DataAndBusinessIntelligence, Assess("Análise de dados Power BI").PrimaryCategory);
    }

    [Fact]
    public void Assess_DataEngineering_ReturnsDataEngineeringCategory()
    {
        Assert.Equal(JobCategory.DataEngineering, Assess("Engenharia de dados ETL").PrimaryCategory);
    }

    [Fact]
    public void Assess_Qa_ReturnsQualityCategory()
    {
        Assert.Equal(JobCategory.QualityAssuranceAndTesting, Assess("QA e testes de software").PrimaryCategory);
    }

    [Fact]
    public void Assess_TestAutomation_ReturnsTestAutomationCategory()
    {
        Assert.Equal(JobCategory.TestAutomation, Assess("Automação de testes Playwright").PrimaryCategory);
    }

    [Fact]
    public void Assess_Sap_ReturnsCorporateSystemsCategory()
    {
        Assert.Equal(JobCategory.CorporateSystemsAndSap, Assess("Analista SAP e ERP").PrimaryCategory);
    }

    [Fact]
    public void Assess_Integration_ReturnsIntegrationCategory()
    {
        Assert.Equal(JobCategory.SystemsIntegrationAndImplementation, Assess("Integração de sistemas via API").PrimaryCategory);
    }

    [Fact]
    public void Assess_ApplicationSupport_ReturnsApplicationSupportCategory()
    {
        Assert.Equal(JobCategory.ApplicationSupport, Assess("Suporte de aplicações").PrimaryCategory);
    }

    [Fact]
    public void Assess_ServiceDesk_ReturnsInfrastructureCategory()
    {
        Assert.Equal(JobCategory.ServiceDeskAndInfrastructure, Assess("Service desk e redes").PrimaryCategory);
    }

    [Fact]
    public void Assess_IndustrialTechnology_ReturnsIndustrialCategory()
    {
        Assert.Equal(JobCategory.IndustrialTechnology, Assess("Tecnologia industrial com IoT").PrimaryCategory);
    }

    [Fact]
    public void Assess_DigitalTransformationWithTechnology_ReturnsDigitalCategory()
    {
        Assert.Equal(JobCategory.DigitalTransformationAndInnovation, Assess("Transformação digital e inovação", "Tecnologia para melhoria contínua").PrimaryCategory);
    }

    [Fact]
    public void Assess_ArtificialIntelligence_ReturnsAiCategory()
    {
        Assert.Equal(JobCategory.ArtificialIntelligence, Assess("Inteligência artificial e machine learning").PrimaryCategory);
    }

    [Fact]
    public void Assess_AdjacentTechnology_ReturnsAdjacentCategory()
    {
        Assert.Equal(JobCategory.AdjacentTechnologyOpportunity, Assess("Analista", "Atuação com Python").PrimaryCategory);
    }

    [Fact]
    public void Assess_NonRelevant_ReturnsNotRelevant()
    {
        Assert.Equal(JobCategory.NotRelevant, Assess("Vendedor comercial", "Atendimento ao cliente").PrimaryCategory);
    }

    [Fact]
    public void Assess_PrimaryCategoryIsNotSecondary()
    {
        var result = Assess("Full stack", "Backend API e frontend TypeScript");

        Assert.DoesNotContain(result.PrimaryCategory, result.SecondaryCategories);
    }

    [Fact]
    public void Assess_SecondaryCategoriesHaveNoDuplicates()
    {
        var result = Assess("Backend e frontend", "Backend API frontend TypeScript");

        Assert.Equal(result.SecondaryCategories.Count, result.SecondaryCategories.Distinct().Count());
    }

    [Fact]
    public void Assess_ClassificationReasonsAreExplainable()
    {
        var result = Assess("Backend API C#");

        Assert.NotEmpty(result.ClassificationReasons);
        Assert.All(result.ClassificationReasons, reason => Assert.False(string.IsNullOrWhiteSpace(reason)));
    }

    [Fact]
    public void Assess_InternshipReceivesPoints()
    {
        Assert.Contains(Assess("Estágio Backend API").ScoreReasons, reason => reason.Points == 35);
    }

    [Fact]
    public void Assess_InternshipProgramReceivesPoints()
    {
        Assert.Contains(Assess("Programa de estágio Backend API").ScoreReasons, reason => reason.Points == 25);
    }

    [Fact]
    public void Assess_CatalaoReceivesPoints()
    {
        Assert.Contains(Assess("Backend API").ScoreReasons, reason => reason.Points == 30);
    }

    [Fact]
    public void Assess_UberlandiaReceivesPoints()
    {
        Assert.Contains(Assess("Backend API", location: "Uberlândia, MG").ScoreReasons, reason => reason.Points == 30);
    }

    [Fact]
    public void Assess_BackendDevelopmentReceivesContributions()
    {
        Assert.Contains(Assess("Desenvolvimento de software Backend API C#").ScoreReasons, reason => reason.Points == 30);
        Assert.Contains(Assess("Desenvolvimento de software Backend API C#").ScoreReasons, reason => reason.Points == 25);
    }

    [Fact]
    public void Assess_AutomationReceivesContributions()
    {
        var result = Assess("Automação de processos RPA Power Automate");
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 30);
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 20);
    }

    [Fact]
    public void Assess_DataReceivesContributions()
    {
        var result = Assess("Engenharia de dados ETL Python Power BI");
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 30);
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 25);
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 20);
    }

    [Fact]
    public void Assess_QaReceivesContributions()
    {
        var result = Assess("QA automação de testes Playwright");
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 25);
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 20);
        Assert.Contains(result.ScoreReasons, reason => reason.Points == 15);
    }

    [Fact]
    public void Assess_UnknownLocationReceivesPenalty()
    {
        Assert.Contains(Assess("Backend API", location: "Remoto").ScoreReasons, reason => reason.Points == -15);
    }

    [Fact]
    public void Assess_MissingDescriptionReceivesPenalty()
    {
        Assert.Contains(Assess("Backend API", description: null!).ScoreReasons, reason => reason.Points == -20);
    }

    [Fact]
    public void Assess_CompletedDegreeReceivesPenalty()
    {
        Assert.Contains(Assess("Backend API", "Ensino superior completo").ScoreReasons, reason => reason.Points == -25);
    }

    [Fact]
    public void Assess_ThreeYearsReceivesPenalty()
    {
        Assert.Contains(Assess("Backend API", "Requer 3 anos de experiência").ScoreReasons, reason => reason.Points == -25);
    }

    [Fact]
    public void Assess_MidLevelReceivesPenalty()
    {
        Assert.Contains(Assess("Desenvolvedor pleno Backend API").ScoreReasons, reason => reason.Points == -40);
    }

    [Fact]
    public void Assess_SeniorReceivesPenalty()
    {
        Assert.Contains(Assess("Desenvolvedor sênior Backend API").ScoreReasons, reason => reason.Points == -50);
    }

    [Fact]
    public void Assess_ClosedPostingReceivesPenalty()
    {
        Assert.Contains(Assess("Backend API", "Vaga encerrada").ScoreReasons, reason => reason.Points == -40);
    }

    [Fact]
    public void Assess_MultipleRulesSumCorrectly()
    {
        var result = Assess("Estágio Backend API C#", "Descrição tecnológica");

        Assert.Equal(result.ScoreReasons.Sum(reason => reason.Points), result.TotalScore);
    }

    [Fact]
    public void Assess_RepeatedSignalsDoNotApplyRuleMoreThanOnce()
    {
        var result = Assess("Estágio estágio Backend backend API API", "Python Python SQL SQL");

        Assert.Equal(1, result.ScoreReasons.Count(reason => reason.Points == 35));
        Assert.Equal(result.ScoreReasons.Sum(reason => reason.Points), result.TotalScore);
    }

    [Theory]
    [InlineData(25, FitBand.AdjacentOpportunity)]
    [InlineData(50, FitBand.ModerateFit)]
    [InlineData(75, FitBand.HighFit)]
    [InlineData(100, FitBand.ImmediateApplication)]
    public void JobScoreResult_UsesExpectedBoundaries(int points, FitBand expectedBand)
    {
        var result = new JobScoreResult(new[] { new JobScoreReason("test", points) });

        Assert.Equal(expectedBand, result.FitBand);
    }

    [Fact]
    public void Assess_ScoreReasonsSumExactlyToTotalScore()
    {
        var result = Assess("Estágio Backend API C#", "Requer 3 anos de experiência");

        Assert.Equal(result.TotalScore, result.ScoreReasons.Sum(reason => reason.Points));
    }

    [Fact]
    public void Assess_DoesNotModifyOriginalPosting()
    {
        var posting = CreatePosting("  ESTÁGIO Backend  ", "Python original");
        var title = posting.OriginalTitle;
        var description = posting.Description;

        var processing = JobPostingProcessor.Process(posting);
        var analysis = JobOpportunityAnalyzer.Analyze(posting);
        JobAssessor.Assess(posting, processing, analysis);

        Assert.Equal(title, posting.OriginalTitle);
        Assert.Equal(description, posting.Description);
    }

    [Fact]
    public void Assess_IsDeterministic()
    {
        var first = Assess("Estágio Full stack", "Backend API frontend TypeScript");
        var second = Assess("Estágio Full stack", "Backend API frontend TypeScript");

        Assert.Equal(first.PrimaryCategory, second.PrimaryCategory);
        Assert.Equal(first.SecondaryCategories, second.SecondaryCategories);
        Assert.Equal(first.TotalScore, second.TotalScore);
        Assert.Equal(first.ScoreReasons.Select(reason => (reason.Description, reason.Points)), second.ScoreReasons.Select(reason => (reason.Description, reason.Points)));
    }
}
