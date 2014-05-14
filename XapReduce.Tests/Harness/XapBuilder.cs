using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace MVeldhuizen.XapReduce.Tests.Harness
{
    internal class XapBuilder
    {
        private static readonly XNamespace DeploymentNamespace = "http://schemas.microsoft.com/client/2007/deployment";
        protected static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

        private readonly MemoryStream _ms = new MemoryStream();
        private ZipArchive _archive;
        private XElement _parts;
        private Random _random = new Random();
        private CompressionLevel _compressionLevel = CompressionLevel.Optimal;

        public XapBuilder() : this(CompressionLevel.Optimal)
        {
        }

        public XapBuilder(CompressionLevel compressionLevel)
        {
            _compressionLevel = compressionLevel;

            AppManifest = new XDocument(
                new XElement(DeploymentNamespace + "Deployment",
                    new XAttribute("xmlns", DeploymentNamespace),
                    new XAttribute(XNamespace.Xmlns + "x", XamlNamespace),
                    _parts = new XElement(DeploymentNamespace + "Deployment.Parts")));

            _archive = new ZipArchive(_ms, ZipArchiveMode.Create, true);
        }

        private XDocument AppManifest { get; set; }

        public XapBuilder AddAssemblyPart(string name, int fileSize)
        {
            if (_archive == null)
            {
                throw new InvalidOperationException("XAP is already built.");
            }

            string fileName = name + ".dll";

            var element = new XElement(DeploymentNamespace + "AssemblyPart",
                new XAttribute(XamlNamespace + "Name", name),
                new XAttribute("Source", fileName));

            _parts.Add(element);

            CreateFileEntry(fileName, fileSize);

            return this;
        }

        public XapBuilder AddResourceAssemblyPart(string culture, string baseName, int fileSize = 2048)
        {
            if (_archive == null)
            {
                throw new InvalidOperationException("XAP is already built.");
            }

            string fileName = baseName + ".resources.dll";

            var element = new XElement(DeploymentNamespace + "AssemblyPart",
                new XAttribute("Source", culture + "/" + fileName));

            _parts.Add(element);

            CreateFileEntry(culture + @"\" + fileName, fileSize);

            return this;
        }

        private void CreateFileEntry(string entryName, int fileSize)
        {
            var entry = _archive.CreateEntry(entryName, _compressionLevel);
            using (var fileStream = entry.Open())
            {
                int remaining = fileSize;
                while (remaining > 0)
                {
                    byte[] buffer = new byte[Math.Min(remaining, 4096)];
                    _random.NextBytes(buffer);
                    fileStream.Write(buffer, 0, buffer.Length);
                    remaining -= buffer.Length;

                    if (remaining > buffer.Length)
                    {
                        fileStream.Write(buffer, 0, buffer.Length);
                        remaining -= buffer.Length;
                    }
                }
            }
        }

        public MemoryStream Build()
        {
            if (_archive != null)
            {
                using (_archive)
                {
                    // Create AppManifest.xaml
                    ZipArchiveEntry appManifestEntry = _archive.CreateEntry("AppManifest.xaml", _compressionLevel);
                    using (Stream stream = appManifestEntry.Open())
                    {
                        AppManifest.Save(stream);
                    }
                }

                _archive = null;
            }

            return _ms;
        }

        public ZipArchive GetArchive(ZipArchiveMode mode = ZipArchiveMode.Read)
        {
            var clonedStream = new MemoryStream(_ms.ToArray());
            return new ZipArchive(clonedStream, mode);
        }

        public long GetSize()
        {
            return _ms.Length;
        }
    }

}
