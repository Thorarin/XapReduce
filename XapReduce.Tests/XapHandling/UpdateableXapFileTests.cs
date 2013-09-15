using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using MVeldhuizen.XapReduce.IO;
using MVeldhuizen.XapReduce.Tests.Harness;
using MVeldhuizen.XapReduce.XapHandling;
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

            var builder = new XapBuilder(CompressionLevel.NoCompression);
            builder.AddAssemblyPart("A", "A.dll", 10000);
            builder.AddAssemblyPart("B", "B.dll", 10000);
            builder.Build();

            fileSystem.FileExists("Input.xap").Returns(true);
            fileSystem.OpenArchive("Input.xap", ZipArchiveMode.Update).Returns(a => builder.GetArchive(ZipArchiveMode.Update));

            var target = new UpdateableXapFile("Input.xap", fileSystem);
            target.RemoveAssemblyPart(target.AssemblyParts[0]);
            target.Recompress();
        }
    }
}
