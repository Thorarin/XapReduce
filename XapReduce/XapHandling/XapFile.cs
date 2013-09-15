using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using MVeldhuizen.XapReduce.IO;

namespace MVeldhuizen.XapReduce.XapHandling
{
    public class XapFile
    {
        private static readonly XNamespace DeploymentNamespace = "http://schemas.microsoft.com/client/2007/deployment";
        protected static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
        private List<AssemblyPartInfo> _assemblyParts = new List<AssemblyPartInfo>();

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
            get
            {
                return _assemblyParts.AsReadOnly();
            }
        }

        //public long GetAssemblyPartSize(string assemblyPart)
        //{
        //    XElement element = AssemblyPartsElements.
        //        Where(el => el.Attribute(XamlNamespace + "Name") != null).
        //        Single(el => el.Attribute(XamlNamespace + "Name").Value == assemblyPart);

        //    string fileName = element.Attribute("Source").Value;

        //    return InputArchive.GetEntry(fileName).Length;
        //}

        protected IEnumerable<XElement> AssemblyPartsElements
        {
            get
            {
                return AppManifest.
                    Element(DeploymentNamespace + "Deployment").
                    Element(DeploymentNamespace + "Deployment.Parts").
                    Elements();                
            }
        }

        protected XDocument AppManifest { get; set; }

        protected ZipArchive InputArchive { get; set; }

        protected void ReadXapManifest(ZipArchive xap)
        {
            var appManifestEntry = xap.GetEntry("AppManifest.xaml");
            using (Stream stream = appManifestEntry.Open())
            {
                AppManifest = XDocument.Load(stream);
            }

            foreach (var e in AssemblyPartsElements.Where(e => e.Attribute(XamlNamespace + "Name") != null))
            {
                string name = e.Attribute(XamlNamespace + "Name").Value;
                string source = e.Attribute("Source").Value;
                long size = xap.GetEntry(source).Length;

                _assemblyParts.Add(new AssemblyPartInfo(name, source, size));
            }
        }
    }
}
