using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RadarEstagios.Jobs.Analysis;

public static partial class JobOpportunityAnalyzer
{
    private static readonly TechnologyPattern[] TechnologyPatterns =
    {
        new("ASP.NET Core", new[] { @"\basp\s*\.\s*net\s+core\b" }),
        new("SAP PM", new[] { @"\bsap\s+pm\b" }),
        new("SAP HANA", new[] { @"\bsap\s+hana\b" }),
        new("Vue.js", new[] { @"\bvue\s*\.\s*js\b" }),
        new("C#", new[] { @"(?<![\p{L}\p{N}])c\s*#(?![\p{L}\p{N}])" }),
        new("C++", new[] { @"(?<![\p{L}\p{N}])c\s*\+\+(?![\p{L}\p{N}])" }),
        new("C", new[] { @"(?<![\p{L}\p{N}])c(?![\p{L}\p{N}])" }),
        new(".NET", new[] { @"(?<![\p{L}\p{N}])(?:\.\s*net|dotnet)(?![\p{L}\p{N}])" }),
        new("JavaScript", new[] { @"\bjava\s*script\b" }),
        new("TypeScript", new[] { @"\btype\s*script\b" }),
        new("Power BI", new[] { @"\bpower\s+bi\b" }),
        new("Power Automate", new[] { @"\bpower\s+automate\b" }),
        new("Power Apps", new[] { @"\bpower\s+apps?\b" }),
        new("Edge Computing", new[] { @"\bedge\s+computing\b" }),
        new("LoRaWAN", new[] { @"\blora\s*wan\b" }),
        new("GitHub", new[] { @"\bgit\s*hub\b" }),
        new("GitLab", new[] { @"\bgit\s*lab\b" }),
        new("Playwright", new[] { @"\bplaywright\b" }),
        new("Selenium", new[] { @"\bselenium\b" }),
        new("Cypress", new[] { @"\bcypress\b" }),
        new("Python", new[] { @"\bpython\b" }),
        new("Java", new[] { @"\bjava\b" }),
        new("Golang", new[] { @"\bgolang\b" }),
        new("Go", new[] { @"(?<![\p{L}\p{N}])go(?![\p{L}\p{N}])" }),
        new("HTML", new[] { @"\bhtml\b" }),
        new("CSS", new[] { @"\bcss\b" }),
        new("SQL", new[] { @"\bsql\b" }),
        new("Git", new[] { @"\bgit\b" }),
        new("Docker", new[] { @"\bdocker\b" }),
        new("Linux", new[] { @"\blinux\b" }),
        new("Pandas", new[] { @"\bpandas\b" }),
        new("NumPy", new[] { @"\bnumpy\b" }),
        new("SAP", new[] { @"\bsap\b" }),
        new("VBA", new[] { @"\bvba\b" }),
        new("IoT", new[] { @"\biot\b" }),
        new("n8n", new[] { @"(?<![\p{L}\p{N}])n8n(?![\p{L}\p{N}])" })
    };

    public static JobOpportunityAnalysisResult Analyze(JobPosting posting)
    {
        ArgumentNullException.ThrowIfNull(posting);

        var title = NormalizeText(posting.OriginalTitle);
        var description = NormalizeText(posting.Description);
        var combinedText = string.Join(" ", title, description);
        var reasons = new List<string>();
        var opportunityType = DetectOpportunityType(title, description, reasons);
        var technologies = ExtractTechnologies(combinedText);

        return new JobOpportunityAnalysisResult(opportunityType, technologies, reasons);
    }

    private static OpportunityType DetectOpportunityType(
        string title,
        string description,
        List<string> reasons)
    {
        if (ContainsPhrase(title, description, "programa de estagio"))
        {
            reasons.Add("Título ou descrição menciona programa de estágio.");
            return OpportunityType.InternshipProgram;
        }

        if (ContainsAny(title, description, "estagio", "estagiario", "estagiaria"))
        {
            reasons.Add("Título ou descrição menciona estágio.");
            return OpportunityType.Internship;
        }

        if (ContainsPhrase(title, description, "trainee"))
        {
            reasons.Add("Título ou descrição menciona trainee.");
            return OpportunityType.Trainee;
        }

        if (ContainsPhrase(title, description, "jovem aprendiz")
            || ContainsPhrase(title, description, "aprendiz"))
        {
            reasons.Add("Título ou descrição menciona aprendiz.");
            return OpportunityType.Apprentice;
        }

        if (ContainsPhrase(title, description, "posicao de entrada")
            || ContainsPhrase(title, description, "nivel de entrada"))
        {
            reasons.Add("Título ou descrição menciona posição de entrada.");
            return OpportunityType.EntryLevel;
        }

        reasons.Add("Nenhum sinal conhecido de tipo de oportunidade foi encontrado.");
        return OpportunityType.Unknown;
    }

    private static IReadOnlyList<string> ExtractTechnologies(string text)
    {
        var technologies = new List<string>();
        var foundGitHubOrGitLab = false;

        foreach (var pattern in TechnologyPatterns)
        {
            if (!pattern.Patterns.Any(patternText => Regex.IsMatch(text, patternText, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)))
            {
                continue;
            }

            if (pattern.Name == "Git" && foundGitHubOrGitLab)
            {
                continue;
            }

            if (pattern.Name is "GitHub" or "GitLab")
            {
                foundGitHubOrGitLab = true;
            }

            technologies.Add(pattern.Name == "Golang" ? "Go" : pattern.Name);
        }

        return technologies;
    }

    private static bool ContainsAny(string title, string description, params string[] phrases)
        => phrases.Any(phrase => ContainsPhrase(title, description, phrase));

    private static bool ContainsPhrase(string title, string description, string phrase)
        => title.Contains(phrase, StringComparison.OrdinalIgnoreCase)
            || description.Contains(phrase, StringComparison.OrdinalIgnoreCase);

    private static string NormalizeText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var decomposed = value.Normalize(NormalizationForm.FormD);
        var withoutAccents = new StringBuilder();

        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                withoutAccents.Append(character);
            }
        }

        return string.Join(
            " ",
            withoutAccents
                .ToString()
                .ToLowerInvariant()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }

    private sealed record TechnologyPattern(string Name, string[] Patterns);
}
