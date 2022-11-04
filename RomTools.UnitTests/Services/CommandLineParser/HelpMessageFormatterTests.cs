using RomTools.Services.CommandLineParser;
using Xunit;

namespace RomTools.UnitTests.Services.CommandLineParser;

public class HelpMessageFormatterTests
{
    [Theory]
    [InlineData(typeof(CommandLineTestOptions), "Data/CommandLineTestOptionsHelpMessage.txt")]
    public void GivenOptionsType_WhenFormat_ThenHelpMessageGenerated(
        Type optionsType,
        string expectedMessagePath)
    {
        // Arrange
        var expected = File.ReadAllText(expectedMessagePath);
        var sut = new HelpMessageFormatter();

        // Act
        var result = sut.Format(optionsType);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GivenOptions_WithMissingHelpText_WhenFormat_ThenValueReturned()
    {
        // Arrange
        var sut = new HelpMessageFormatter();

        // Act
        var result = sut.Format(typeof(CommandLineTestBadOptions));

        // Assert
        Assert.Equal("Available Options\r\n", result);
    }
}
