using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using RadarEstagios.Jobs.Analysis;
using RadarEstagios.Jobs.Processing;

namespace RadarEstagios.Jobs.Assessment;

public static class JobAssessor
{
    private static readonly JobCategory[] TieBreakOrder =
    {
        JobCategory.FullStackDevelopment,
        JobCategory.BackendDevelopment,
        JobCategory.FrontendDevelopment,
        JobCategory.TestAutomation,
        JobCategory.QualityAssuranceAndTesting,
        JobCategory.DataEngineering,
        JobCategory.DataAndBusinessIntelligence,
        JobCategory.ArtificialIntelligence,
        JobCategory.ProcessAutomationAndRpa,
        JobCategory.CorporateSystemsAndSap,
        JobCategory.SystemsIntegrationAndImplementation,
        JobCategory.ApplicationSupport,
        JobCategory.ServiceDeskAndInfrastructure,
        JobCategory.IndustrialTechnology,
        JobCategory.DigitalTransformationAndInnovation,
        JobCategory.GeneralSoftwareDevelopment,
        JobCategory.AdjacentTechnologyOpportunity,
        JobCategory.NotRelevant
    };

    public static JobAssessmentResult Assess(
        JobPosting posting,
        JobProcessingResult processing,
        JobOpportunityAnalysisResult analysis)
    {
        ArgumentNullException.ThrowIfNull(posting);
        ArgumentNullException.ThrowIfNull(processing);
        ArgumentNullException.ThrowIfNull(analysis);

        var title = Normalize(posting.OriginalTitle);
        var description = Normalize(posting.Description);
        var text = string.Join(" ", title, description);
        var classification = Classify(posting, processing, analysis, text);
        var score = Score(posting, processing, analysis, classification, text);

        return new JobAssessmentResult(posting, processing, analysis, classification, score);
    }

    private static JobClassificationResult Classify(
        JobPosting posting,
        JobProcessingResult processing,
        JobOpportunityAnalysisResult analysis,
        string text)
    {
        var signals = new Dictionary<JobCategory, int>();
        var reasons = new List<string>();
        var backend = Count(text, "backend", "back-end", "api", "rest", "asp.net core", ".net", "c#", "java", "go", "golang", "microsserviço", "banco de dados", "sql");
        var frontend = Count(text, "frontend", "front-end", "vue", "vue.js", "react", "angular", "javascript", "typescript", "html", "css", "interface web");
        var explicitFullStack = ContainsAny(text, "full stack", "fullstack", "full-stack");
        var development = Count(text, "desenvolvimento de software", "programação", "desenvolvedor", "sistemas", "aplicações", "engenharia de software");
        var automation = Count(text, "automação de processos", "rpa", "power automate", "vba", "scripts", "scripting", "sap gui scripting", "processamento em lote", "n8n");
        var data = Count(text, "análise de dados", "bi", "business intelligence", "power bi", "dashboard", "indicadores", "analytics", "pandas", "numpy");
        var dataEngineering = Count(text, "engenharia de dados", "etl", "pipeline de dados", "data warehouse", "data lake", "qualidade de dados");
        var quality = Count(text, "qa", "qualidade de software", "testes de software", "teste funcional", "teste unitário", "homologação");
        var testAutomation = Count(text, "automação de testes", "playwright", "selenium", "cypress");
        var corporate = Count(text, "sap", "erp", "sistemas corporativos");
        var integration = Count(text, "integração de sistemas", "implantação de sistemas", "implementação de sistemas", "integração via api", "suporte à implantação");
        var support = Count(text, "suporte de aplicações", "suporte a sistemas", "sustentação de sistemas", "suporte funcional", "atendimento relacionado a software corporativo");
        var infrastructure = Count(text, "service desk", "help desk", "infraestrutura", "redes", "administração de servidores", "suporte de hardware");
        var industrial = Count(text, "indústria 4.0", "tecnologia industrial", "iot", "edge computing", "lorawan", "digitalização industrial", "sistemas de manutenção", "manutenção digital");
        var digital = Count(text, "transformação digital", "inovação", "digitalização", "melhoria contínua com tecnologia", "tecnologia e inovação");
        var ai = Count(text, "inteligência artificial", "machine learning", "mineração de dados", "visão computacional", "modelos de linguagem", "ia generativa");

        Add(signals, JobCategory.BackendDevelopment, backend);
        Add(signals, JobCategory.FrontendDevelopment, frontend);
        if (explicitFullStack || backend > 0 && frontend > 0)
        {
            Add(signals, JobCategory.FullStackDevelopment, explicitFullStack ? 3 : 2);
            reasons.Add(explicitFullStack ? "A vaga menciona full stack." : "A vaga possui sinais de backend e frontend.");
        }
        Add(signals, JobCategory.GeneralSoftwareDevelopment, development);
        Add(signals, JobCategory.ProcessAutomationAndRpa, automation);
        Add(signals, JobCategory.DataAndBusinessIntelligence, data);
        Add(signals, JobCategory.DataEngineering, dataEngineering);
        Add(signals, JobCategory.QualityAssuranceAndTesting, quality);
        Add(signals, JobCategory.TestAutomation, testAutomation);
        Add(signals, JobCategory.CorporateSystemsAndSap, corporate);
        Add(signals, JobCategory.SystemsIntegrationAndImplementation, integration * 100);
        Add(signals, JobCategory.ApplicationSupport, support);
        Add(signals, JobCategory.ServiceDeskAndInfrastructure, infrastructure);
        Add(signals, JobCategory.IndustrialTechnology, industrial);
        Add(signals, JobCategory.DigitalTransformationAndInnovation, digital > 0 && HasTechnology(text, analysis) ? digital : 0);
        Add(signals, JobCategory.ArtificialIntelligence, ai);

        var hasTechnology = HasTechnology(text, analysis);
        var primary = integration > 0
            ? JobCategory.SystemsIntegrationAndImplementation
            : signals.Count == 0
                ? hasTechnology ? JobCategory.AdjacentTechnologyOpportunity : JobCategory.NotRelevant
                : signals.OrderByDescending(pair => pair.Value)
                    .ThenBy(pair => Array.IndexOf(TieBreakOrder, pair.Key))
                    .First().Key;

        if (primary == JobCategory.NotRelevant)
        {
            reasons.Add("Não foram encontrados sinais tecnológicos suficientes.");
        }
        else if (primary == JobCategory.AdjacentTechnologyOpportunity)
        {
            reasons.Add("Há tecnologia mencionada, mas não evidência suficiente para uma categoria específica.");
        }
        else
        {
            reasons.Add($"A vaga apresenta sinais de {primary}.");
        }

        var secondary = signals.Keys.Where(category => category != primary).ToArray();
        return new JobClassificationResult(primary, secondary, reasons);
    }

    private static JobScoreResult Score(
        JobPosting posting,
        JobProcessingResult processing,
        JobOpportunityAnalysisResult analysis,
        JobClassificationResult classification,
        string text)
    {
        var reasons = new List<JobScoreReason>();
        var typePoints = analysis.OpportunityType switch
        {
            OpportunityType.Internship => 35,
            OpportunityType.InternshipProgram => 25,
            OpportunityType.Trainee => 15,
            OpportunityType.EntryLevel => 10,
            OpportunityType.Apprentice when classification.PrimaryCategory != JobCategory.NotRelevant => 10,
            _ => 0
        };
        AddScore(reasons, "Tipo da oportunidade", typePoints);

        var normalizedLocation = Normalize(posting.OriginalLocation);
        if (processing.NormalizedLocation is "Catalão, GO" or "Uberlândia, MG")
        {
            AddScore(reasons, "Localização reconhecida em Catalão ou Uberlândia", 30);
        }
        else if (ContainsAny(normalizedLocation, "são paulo", "goiânia", "belo horizonte", "ribeirão preto", "brasilia"))
        {
            AddScore(reasons, "Localização indica outra cidade", -40);
        }
        else
        {
            AddScore(reasons, "Localização não determinada", -15);
        }

        var developmentMain = classification.PrimaryCategory is JobCategory.BackendDevelopment or JobCategory.FrontendDevelopment or JobCategory.FullStackDevelopment or JobCategory.GeneralSoftwareDevelopment;
        AddScore(reasons, "Desenvolvimento de software como atividade principal", developmentMain ? 30 : 0);
        AddScore(reasons, "Backend, APIs ou banco de dados relevantes", classification.SecondaryCategories.Contains(JobCategory.BackendDevelopment) || classification.PrimaryCategory == JobCategory.BackendDevelopment ? 25 : 0);
        AddScore(reasons, "Tecnologias de desenvolvimento relevantes", developmentMain && analysis.Technologies.Any(technology => new[] { "Go", "C#", ".NET", "Python", "SQL" }.Contains(technology)) ? 20 : 0);
        AddScore(reasons, "Frontend relevante", developmentMain && analysis.Technologies.Any(technology => new[] { "Vue.js", "JavaScript", "TypeScript", "HTML", "CSS" }.Contains(technology)) ? 15 : 0);
        AddScore(reasons, "Git ou plataforma Git relevante", developmentMain && analysis.Technologies.Any(technology => new[] { "Git", "GitHub", "GitLab" }.Contains(technology)) ? 10 : 0);

        var automationMain = classification.PrimaryCategory == JobCategory.ProcessAutomationAndRpa;
        AddScore(reasons, "Automação de processos como atividade principal", automationMain ? 30 : 0);
        AddScore(reasons, "RPA, scripts ou integração de sistemas", ContainsAny(text, "rpa", "scripts", "scripting", "integração de sistemas") ? 25 : 0);
        AddScore(reasons, "SAP, Power Platform ou sistemas corporativos", analysis.Technologies.Any(technology => new[] { "SAP", "SAP PM", "SAP HANA", "Power Automate", "Power Apps" }.Contains(technology)) ? 20 : 0);
        AddScore(reasons, "Melhoria de processos com tecnologia", ContainsAny(text, "melhoria de processos", "melhoria contínua com tecnologia") ? 15 : 0);

        var dataMain = classification.PrimaryCategory is JobCategory.DataAndBusinessIntelligence or JobCategory.DataEngineering or JobCategory.ArtificialIntelligence;
        AddScore(reasons, "Dados como atividade principal", dataMain ? 30 : 0);
        AddScore(reasons, "Tecnologias de dados relevantes", dataMain && analysis.Technologies.Any(technology => new[] { "Python", "SQL", "Pandas", "NumPy" }.Contains(technology)) ? 25 : 0);
        AddScore(reasons, "BI, dashboards ou indicadores", ContainsAny(text, "power bi", "dashboard", "indicadores") ? 20 : 0);
        AddScore(reasons, "Inteligência artificial ou mineração de dados", ContainsAny(text, "inteligência artificial", "mineração de dados") ? 15 : 0);

        var qualityMain = classification.PrimaryCategory is JobCategory.QualityAssuranceAndTesting or JobCategory.TestAutomation;
        AddScore(reasons, "QA ou testes", qualityMain ? 25 : 0);
        AddScore(reasons, "Automação de testes", classification.PrimaryCategory == JobCategory.TestAutomation ? 20 : 0);
        AddScore(reasons, "Playwright, Selenium ou Cypress", analysis.Technologies.Any(technology => new[] { "Playwright", "Selenium", "Cypress" }.Contains(technology)) ? 15 : 0);
        AddScore(reasons, "Logging ou observabilidade", ContainsAny(text, "logging", "observabilidade") ? 10 : 0);

        AddScore(reasons, "Vaga exclusivamente comercial", IsExclusive(text, analysis, "comercial") ? -30 : 0);
        AddScore(reasons, "Vaga exclusivamente administrativa sem tecnologia", IsExclusive(text, analysis, "administrativa") ? -30 : 0);
        AddScore(reasons, "Vaga exclusivamente mecânica, elétrica ou operacional", IsExclusive(text, analysis, "mecânica", "elétrica", "operacional") ? -30 : 0);
        AddScore(reasons, "Exige três anos ou mais de experiência", ContainsAny(text, "3 anos", "três anos", "3+ anos", "mais de 3 anos") ? -25 : 0);
        AddScore(reasons, "Exige graduação concluída", ContainsAny(text, "graduação concluída", "ensino superior completo") ? -25 : 0);
        AddScore(reasons, "Descrição ausente", string.IsNullOrWhiteSpace(posting.Description) ? -20 : 0);
        AddScore(reasons, "Vaga encerrada", ContainsAny(text, "vaga encerrada", "processo encerrado", "oportunidade encerrada") ? -40 : 0);
        AddScore(reasons, "Exige perfil pleno", ContainsAny(text, "pleno") ? -40 : 0);
        AddScore(reasons, "Exige perfil sênior ou liderança", ContainsAny(text, "sênior", "senior", "especialista", "coordenação", "coordenador", "liderança", "gerência", "gerente") ? -50 : 0);

        return new JobScoreResult(reasons);
    }

    private static void Add(Dictionary<JobCategory, int> signals, JobCategory category, int value)
    {
        if (value > 0)
        {
            signals[category] = value;
        }
    }

    private static void AddScore(List<JobScoreReason> reasons, string description, int points)
    {
        if (points != 0)
        {
            reasons.Add(new JobScoreReason(description, points));
        }
    }

    private static int Count(string text, params string[] signals)
        => signals.Count(signal => ContainsAny(text, signal));

    private static bool ContainsAny(string text, params string[] signals)
        => signals.Any(signal => ContainsSignal(text, signal));

    private static bool ContainsSignal(string text, string signal)
    {
        var normalizedSignal = Normalize(signal);
        if (normalizedSignal.Contains(' '))
        {
            return text.Contains(normalizedSignal, StringComparison.Ordinal);
        }

        return Regex.IsMatch(
            text,
            $"(?<![\\p{{L}}\\p{{N}}]){Regex.Escape(normalizedSignal)}(?![\\p{{L}}\\p{{N}}])",
            RegexOptions.CultureInvariant);
    }

    private static bool IsExclusive(string text, JobOpportunityAnalysisResult analysis, params string[] signals)
        => ContainsAny(text, signals) && !HasTechnology(text, analysis);

    private static bool HasTechnology(string text, JobOpportunityAnalysisResult analysis)
        => analysis.Technologies.Count > 0 || ContainsAny(text, "api", "software", "sistemas", "dados", "tecnologia", "automação", "rpa", "sap", "qa", "teste", "infraestrutura");

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var decomposed = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return string.Join(" ", builder.ToString().ToLowerInvariant().Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
