using RadarEstagios.Locations;

namespace RadarEstagios.Tests.Locations;

public class LocationNormalizerTests
{
    [Theory]
    [InlineData("Catalão", "Catalão, GO")]
    [InlineData("Catalão/GO", "Catalão, GO")]
    [InlineData("Catalão - GO", "Catalão, GO")]
    [InlineData("Catalão, Goiás", "Catalão, GO")]
    [InlineData("Catalao", "Catalão, GO")]
    [InlineData("Catalao/GO", "Catalão, GO")]
    [InlineData("Catalao - GO", "Catalão, GO")]
    [InlineData("Catalao, Goias", "Catalão, GO")]
    public void Normalize_WhenLocationIsCatalao_ReturnsCanonicalLocation(
        string input,
        string expected)
    {
        var result = LocationNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Uberlândia", "Uberlândia, MG")]
    [InlineData("Uberlândia/MG", "Uberlândia, MG")]
    [InlineData("Uberlândia - MG", "Uberlândia, MG")]
    [InlineData("Uberlândia, Minas Gerais", "Uberlândia, MG")]
    [InlineData("Uberlandia", "Uberlândia, MG")]
    [InlineData("Uberlandia/MG", "Uberlândia, MG")]
    [InlineData("Uberlandia - MG", "Uberlândia, MG")]
    [InlineData("Uberlandia, Minas Gerais", "Uberlândia, MG")]
    public void Normalize_WhenLocationIsUberlandia_ReturnsCanonicalLocation(
        string input,
        string expected)
    {
        var result = LocationNormalizer.Normalize(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Goiânia, GO")]
    [InlineData("São Paulo, SP")]
    [InlineData("Remoto")]
    [InlineData("UDI")]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_WhenLocationIsNotRecognized_ReturnsNull(string input)
    {
        var result = LocationNormalizer.Normalize(input);

        Assert.Null(result);
    }

    [Fact]
    public void Normalize_WhenLocationIsNull_ReturnsNull()
    {
        var result = LocationNormalizer.Normalize(null);

        Assert.Null(result);
    }
}