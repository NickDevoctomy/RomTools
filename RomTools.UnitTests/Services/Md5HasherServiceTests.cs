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
        public void GivenUnarchivedFiles_WhenHashAll_ThenAllFilesHashed_AndCorrectHashStoredInEnvelopeProperties()
        {
            // Arrange
            var roms = Directory.GetFiles("Data/Roms/", "*.rom").OrderBy(x => x);
            var enveloped = roms.Select(x => new FileEnvelope(x)).ToList();
            var sut = new Md5HasherService();

            // Act
            sut.HashAll(enveloped);

            // Assert
            Assert.Equal("D8JRML/lGiknsGLvN2nCQg==", enveloped.Single(x => x.FullPath.EndsWith("romfile1.rom")).Properties["RawMd5Hash"]);
            Assert.Equal("QmNjnEg5Ci5WJcNKyRddHg==", enveloped.Single(x => x.FullPath.EndsWith("romfile2.rom")).Properties["RawMd5Hash"]);
            Assert.Equal("d8vX5VuXrD5bBeUksL1pug==", enveloped.Single(x => x.FullPath.EndsWith("romfile3.rom")).Properties["RawMd5Hash"]);
        }

        [Fact]
        public void GivenArchivedFiles_WhenHashAll_ThenAllFilesHashed_AndCorrectHashStoredInEnvelopeProperties()
        {
            // Arrange
            var roms = Directory.GetFiles("Data/Roms/", "*.*").Where(x => !x.EndsWith(".rom")).OrderBy(x => x);
            var enveloped = roms.Select(x => new FileEnvelope(x)).ToList();
            var sut = new Md5HasherService();

            // Act
            sut.HashAll(enveloped);

            // Assert
            Assert.Equal("True", enveloped.Single(x => x.FullPath.EndsWith("romfile3.zip")).Properties["Archived"]);
            Assert.Equal("romfile3.rom", enveloped.Single(x => x.FullPath.EndsWith("romfile3.zip")).Properties["ArchivedRomName"]);
            Assert.Equal("d8vX5VuXrD5bBeUksL1pug==", enveloped.Single(x => x.FullPath.EndsWith("romfile3.zip")).Properties["ArchivedMd5Hash"]);
            Assert.Equal("cf+7O/j3vm7zmIxn4/gikg==", enveloped.Single(x => x.FullPath.EndsWith("romfile3.zip")).Properties["RawMd5Hash"]);

            // !!! Need to add support for 7zip files here
            Assert.Equal("True", enveloped.Single(x => x.FullPath.EndsWith("romfile2.7z")).Properties["Archived"]);
            Assert.Equal("romfile2.rom", enveloped.Single(x => x.FullPath.EndsWith("romfile2.7z")).Properties["ArchivedRomName"]);
            Assert.Equal("QmNjnEg5Ci5WJcNKyRddHg==", enveloped.Single(x => x.FullPath.EndsWith("romfile2.7z")).Properties["ArchivedMd5Hash"]);
            Assert.Equal("???", enveloped.Single(x => x.FullPath.EndsWith("romfile2.7z")).Properties["RawMd5Hash"]);
        }
    }
}
