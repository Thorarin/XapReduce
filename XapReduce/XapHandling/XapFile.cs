using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using MVeldhuizen.XapReduce.IO;

namespace MVeldhuizen.XapReduce.XapHandling
{
    public class XapFile
    {
        private static readonly XNamespace DeploymentNamespace = "http://schemas.microsoft.com/client/2007/deployment";
        protected static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
        private readonly List<AssemblyPartInfo> _assemblyParts = new List<AssemblyPartInfo>();

        protected XapFile(string xapPath, ZipArchive archive)
        {
            InputPath = xapPath;
            InputArchive = archive;
            ReadXapManifest(archive);
        }

        public XapFile(string xapPath, IFileSystem fileSystem)
        {
            InputPath = xapPath;
            using (InputArchive = fileSystem.OpenArchive(xapPath, ZipArchiveMode.Read))
            {
                ReadXapManifest(InputArchive);
            }
        }

        public string InputPath { get; private set; }

        public IList<AssemblyPartInfo> AssemblyParts
        {
            get { return _assemblyParts.AsReadOnly(); }
        }

        protected IEnumerable<XElement> AssemblyPartsElements
        {
            get
            {
                XElement deploymentEl = AppManifest.Element(DeploymentNamespace + "Deployment");
                if (deploymentEl == null)
                {
                    throw new InvalidDataException("The AppManifest.xaml does not contain a Deployment element.");
                }

                XElement deploymentPartsEl = deploymentEl.Element(DeploymentNamespace + "Deployment.Parts");
                if (deploymentPartsEl == null)
                {
                    throw new InvalidDataException("The AppManifest.xaml does not contain a Deployment.Parts element.");
                }

                return deploymentPartsEl.Elements();
            }
        }

        protected XDocument AppManifest { get; private set; }

        protected ZipArchive InputArchive { get; set; }

        protected void ReadXapManifest(ZipArchive xap)
        {
            var appManifestEntry = xap.GetEntry("AppManifest.xaml");
            using (Stream stream = appManifestEntry.Open())
            {
                AppManifest = XDocument.Load(stream);
            }

            foreach (var e in AssemblyPartsElements)
            {
                var nameAttribute = e.Attribute(XamlNamespace + "Name");
                string name = nameAttribute != null ? nameAttribute.Value : null;
                string source = e.Attribute("Source").Value;
                long size = xap.GetEntry(ManifestSourceToEntryName(source)).Length;

                _assemblyParts.Add(new AssemblyPartInfo(name, source, size));
            }
        }

        protected static string ManifestSourceToEntryName(string manifestSource)
        {
            return manifestSource.Replace('/', '\\');
        }
    }
}
