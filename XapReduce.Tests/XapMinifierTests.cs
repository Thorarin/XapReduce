using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MVeldhuizen.XapReduce.IO;
using MVeldhuizen.XapReduce.Tests.Harness;
using MVeldhuizen.XapReduce.XapHandling;
using NSubstitute;

namespace MVeldhuizen.XapReduce.Tests
{
    [TestClass]
    public class XapMinifierTests
    {
        public XapMinifierTests()
        {
            // Force some jitting to be done prior to executing the actual tests
            // ReSharper disable EmptyGeneralCatchClause
            try
            {
                ReduceXap_UpdateExistingFile_Test();
            }
            catch {}
            // ReSharper restore EmptyGeneralCatchClause
        }

        [TestMethod]
        public void ReduceXap_UpdateExistingFile_Test()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var console = new StringWriter();

            var inputBuilder = CreateFakeInputXap(fileSystem, ZipArchiveMode.Update, "A", "B");
            var sourceBuilder = CreateFakeSourceXap(fileSystem, "A", "C");

            var options = new Options()
            {
                Input = "Input.xap",
                Sources = new[] {"Source.xap"}
            };

            var builder = new XapBuilder();
            builder.AddAssemblyPart("A", "A.dll", 1000);

            var minifier = new XapMinifier(fileSystem, console);
            minifier.ReduceXap(options);

            var output = inputBuilder.GetArchive();
            Assert.AreEqual(2, output.Entries.Count);
            Assert.IsNotNull(output.GetEntry("B.dll"));
        }


        [TestMethod]
        public void ReduceXap_UpdateExistingFileWithRecompress_RecompressionCanceled()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var console = new StringWriter();

            var inputBuilder = CreateFakeInputXap(fileSystem, ZipArchiveMode.Update, "A", "B");
            var sourceBuilder = CreateFakeSourceXap(fileSystem, "A", "C");

            var options = new Options()
            {
                Input = "Input.xap",
                Sources = new[] { "Source.xap" },
                Recompress = true
            };

            var builder = new XapBuilder();
            builder.AddAssemblyPart("A", "A.dll", 1000);

            var minifier = new XapMinifier(fileSystem, console);
            minifier.ReduceXap(options);

            var output = inputBuilder.GetArchive();
            Assert.AreEqual(2, output.Entries.Count);
            Assert.IsNotNull(output.GetEntry("B.dll"));
        }

        [TestMethod]
        public void ReduceXap_UpdateExistingFileWithRecompress_RecompressionSuccessful()
        {
            var fileSystem = Substitute.For<IFileSystem>();

            var consoleBuilder = new StringBuilder();
            var consoleOutput = new StringWriter(consoleBuilder);

            var inputBuilder = CreateFakeInputXap(fileSystem, ZipArchiveMode.Update, CompressionLevel.NoCompression, "A", "B");
            var sourceBuilder = CreateFakeSourceXap(fileSystem, "A", "C");

            fileSystem.FileSize("Input.xap").Returns(s => inputBuilder.GetSize());


            var options = new Options()
            {
                Input = "Input.xap",
                Sources = new[] { "Source.xap" },
                Recompress = true
            };

            var builder = new XapBuilder();
            builder.AddAssemblyPart("A", "A.dll", 1000);

            var minifier = new XapMinifier(fileSystem, consoleOutput);
            minifier.ReduceXap(options);

            string console = consoleBuilder.ToString();
            var output = inputBuilder.GetArchive();
            Assert.AreEqual(2, output.Entries.Count);
            Assert.IsNotNull(output.GetEntry("B.dll"));
        }

        [TestMethod]
        public void ReduceXap_CreateNewFile_Test()
        {
            var fileSystem = Substitute.For<IFileSystem>();
            var console = new StringWriter();

            var inputBuilder = CreateFakeInputXap(fileSystem, ZipArchiveMode.Read, "A", "B");
            var sourceBuilder = CreateFakeSourceXap(fileSystem, "A", "C");

            MemoryStream outputStream = new MemoryStream();
            fileSystem.FileExists("Output.xap").Returns(true);
            fileSystem.OpenArchive("Output.xap", ZipArchiveMode.Create).Returns(new ZipArchive(outputStream, ZipArchiveMode.Create, true));

            var options = new Options()
            {
                Input = "Input.xap",
                Sources = new[] { "Source.xap" },
                Output = "Output.xap"
            };

            var builder = new XapBuilder();
            builder.AddAssemblyPart("A", "A.dll", 1000);

            var minifier = new XapMinifier(fileSystem, console);
            minifier.ReduceXap(options);

            var output = new ZipArchive(outputStream, ZipArchiveMode.Read, true);
            Assert.AreEqual(2, output.Entries.Count);
            Assert.IsNotNull(output.GetEntry("B.dll"));
        }

        private XapBuilder CreateFakeInputXap(IFileSystem fileSystem, ZipArchiveMode mode, params string[] assemblies)
        {
            return CreateFakeInputXap(fileSystem, mode, CompressionLevel.Optimal, assemblies);
        }

        private XapBuilder CreateFakeInputXap(IFileSystem fileSystem, ZipArchiveMode mode, CompressionLevel compressionLevel, params string[] assemblies)
        {
            var builder = new XapBuilder(compressionLevel);

            foreach (string assembly in assemblies)
            {
                builder.AddAssemblyPart(assembly, assembly + ".dll", 10000);
            }

            fileSystem.FileExists("Input.xap").Returns(true);
            fileSystem.OpenArchive("Input.xap", mode).Returns(a => new ZipArchive(builder.Build(), mode, true));

            return builder;
        }

        private XapBuilder CreateFakeSourceXap(IFileSystem fileSystem, params string[] assemblies)
        {
            var builder = new XapBuilder();

            foreach (string assembly in assemblies)
            {
                builder.AddAssemblyPart(assembly, assembly + ".dll", 10000);
            }

            fileSystem.FileExists("Source.xap").Returns(true);
            fileSystem.OpenArchive("Source.xap", ZipArchiveMode.Read).Returns(new ZipArchive(builder.Build()));

            return builder;
        }

    }
}
