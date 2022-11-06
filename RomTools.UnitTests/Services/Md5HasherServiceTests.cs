using RomTools.Models;
using RomTools.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RomTools.UnitTests.Services
{
    public class Md5HasherServiceTests
    {
        [Fact]
        public void GivenFiles_WhenHashAll_ThenAllFilesHashed_AndCorrectHashStoredInEnvelopeProperties()
        {
            // Arrange
            var roms = Directory.GetFiles("Data/Roms/", "*.*").OrderBy(x => x);
            var enveloped = roms.Select(x => new FileEnvelope(x)).ToList();
            var sut = new Md5HasherService();

            // Act
            sut.HashAll(enveloped);

            // Assert
            Assert.All(enveloped, x => x.Properties.ContainsKey("Md5Hash"));
            Assert.Equal("D8JRML/lGiknsGLvN2nCQg==", enveloped[0].Properties["Md5Hash"]);
            Assert.Equal("QmNjnEg5Ci5WJcNKyRddHg==", enveloped[1].Properties["Md5Hash"]);
            Assert.Equal("d8vX5VuXrD5bBeUksL1pug==", enveloped[2].Properties["Md5Hash"]);
        }
    }
}
