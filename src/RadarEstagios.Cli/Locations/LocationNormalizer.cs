using System.Globalization;
using System.Text;

namespace RadarEstagios.Locations;

public static class LocationNormalizer
{
    public static string? Normalize(string? location)
    {
        if (string.IsNullOrWhiteSpace(location))
        {
            return null;
        }

        var normalizedLocation = PrepareForComparison(location);

        return normalizedLocation switch
        {
            "catalao" => "Catalão, GO",
            "catalao go" => "Catalão, GO",
            "catalao goias" => "Catalão, GO",

            "uberlandia" => "Uberlândia, MG",
            "uberlandia mg" => "Uberlândia, MG",
            "uberlandia minas gerais" => "Uberlândia, MG",

            _ => null
        };
    }

    private static string PrepareForComparison(string location)
    {
        var decomposedLocation = location.Normalize(NormalizationForm.FormD);
        var charactersWithoutAccents = new StringBuilder();

        foreach (var character in decomposedLocation)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character)
                != UnicodeCategory.NonSpacingMark)
            {
                charactersWithoutAccents.Append(character);
            }
        }

        var normalizedSeparators = charactersWithoutAccents
            .ToString()
            .ToLowerInvariant()
            .Replace("/", " ")
            .Replace("-", " ")
            .Replace(",", " ");

        return string.Join(
            " ",
            normalizedSeparators.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));
    }
}