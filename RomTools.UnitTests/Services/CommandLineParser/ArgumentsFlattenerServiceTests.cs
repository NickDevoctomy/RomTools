using RomTools.Services.CommandLineParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RomTools.UnitTests.Services.CommandLineParser
{
    public class ArgumentsFlattenerServiceTests
    {
        [Theory]
        [InlineData("command:a=some text:b=some text:c=some text:d=false", "command a=\"some text\" b=\"some text\" c=\"some text\" d=false")]
        public void GivenArguments_WhenFlatten_ThenCorrectResultReturned(
            string args,
            string expectedFlatArguments)
        {
            // Arrange
            var arguments = args.Split(':');
            var sut = new ArgumentsFlattenerService();

            // Act
            var result = sut.Flatten(arguments);

            // Assert
            Assert.Equal(expectedFlatArguments, result);
        }
    }
}
