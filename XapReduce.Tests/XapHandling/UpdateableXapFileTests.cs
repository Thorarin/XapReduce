using System;
using System.IO.Compression;
using MVeldhuizen.XapReduce.IO;
using MVeldhuizen.XapReduce.Tests.Harness;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MVeldhuizen.XapReduce.XapHandling.Tests
{
    [TestClass]
    public class UpdateableXapFileTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Recompress_FileHasPendingChanges_ThrowsException()
        {
            var fileSystem = Substitute.For<IFileSystem>();

            var builder = new XapBuilder(CompressionLevel.NoCompression).
                AddAssemblyPart("A", 10000).
                AddAssemblyPart("B", 10000);
            builder.Build();

            fileSystem.FileExists("Input.xap").Returns(true);
            fileSystem.OpenArchive("Input.xap", ZipArchiveMode.Update).Returns(a => builder.GetArchive(ZipArchiveMode.Update));

            var target = new UpdateableXapFile("Input.xap", fileSystem);
            target.RemoveAssemblyPart(target.AssemblyParts[0]);
            target.Recompress();
        }
    }
}
