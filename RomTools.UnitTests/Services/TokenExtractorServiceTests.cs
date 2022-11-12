using RomTools.Models;
using RomTools.Services;
using Xunit;

namespace RomTools.UnitTests.Services
{
    public class TokenExtractorServiceTests
    {
        [Theory]
        [InlineData("c:/dir/file (a) (b) (c).zip", "(:)", "(a):(b):(c)")]
        [InlineData("c:/dir/file (a) (b) (c,d,e,f).zip", "(:)", "(a):(b):(c,d,e,f)")]
        [InlineData("c:/dir/file (a) (b) (c,d,e,f) (some - thing).zip", "(:)", "(a):(b):(c,d,e,f):(some - thing)")]
        [InlineData("c:/dir/file (a) (b) (c,d,e,f) (some - thing) (1234pop4321).zip", "(:)", "(a):(b):(c,d,e,f):(some - thing):(1234pop4321)")]
        [InlineData("c:/dir/file (a 1234 - 7887y4 -+ ;lsdklpop!@3)", "(:)", "(a 1234 - 7887y4 -+ ;lsdklpop!@3)")]
        public void GivenFile_WhenExtractTokens_ThenCorrectTokensExtracted(
            string fullPath,
            string braces,
            string expectedTokens)
        {
            // Arrange
            var input = new FileEnvelope(fullPath);
            var sut = new TokenExtractorService();

            // Act
            var result = sut.ExtractTokens(
                input,
                braces.Split(':'));

            // Assert
            Assert.True(result.SequenceEqual(expectedTokens.Split(':')));
        }
    }
}
