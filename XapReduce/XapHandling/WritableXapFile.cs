using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using MVeldhuizen.XapReduce.IO;

namespace MVeldhuizen.XapReduce.XapHandling
{
    public abstract class WritableXapFile : XapFile, IDisposable
    {
        protected WritableXapFile(string outputPath, IFileSystem fileSystem) : base(outputPath, fileSystem.OpenArchive(outputPath, ZipArchiveMode.Update))
        {
            FileSystem = fileSystem;
            OutputPath = outputPath;
            OutputArchive = InputArchive;
        }

        protected WritableXapFile(string inputPath, string outputPath, IFileSystem fileSystem) : base(inputPath, fileSystem)
        {
            FileSystem = fileSystem;
            OutputPath = outputPath;

            if (fileSystem.FileExists(outputPath))
                fileSystem.FileDelete(outputPath);
            
            OutputArchive = fileSystem.OpenArchive(OutputPath, ZipArchiveMode.Create);
            Debug.Assert(OutputArchive != null);
        }

        public bool HasChanges { get; protected set; }

        public string OutputPath { get; private set; }

        public ZipArchive OutputArchive { get; private set; }

        protected IFileSystem FileSystem { get; private set; }

        public void Dispose()
        {
            Save();
        }

        public void RemoveAssemblyPart(AssemblyPartInfo assemblyPart)
        {
            XElement element = AssemblyPartsElements.
                Where(el => el.Attribute("Source") != null).
                Single(el => el.Attribute("Source").Value == assemblyPart.FileName);

            element.Remove();
            RemoveFileEntry(assemblyPart.FileName);
            HasChanges = true;
        }

        protected abstract void RemoveFileEntry(string fileName);

        public abstract void Save();

        /// <summary>
        ///     Create a new AppManifest.xaml in the archive.
        /// </summary>
        protected void CreateAppManifestEntry()
        {
            ZipArchiveEntry appManifestEntry = OutputArchive.CreateEntry("AppManifest.xaml", CompressionLevel.Optimal);
            using (Stream stream = appManifestEntry.Open())
            {
                AppManifest.Save(stream);
            }
        }

        protected static void CopyZipEntries(ZipArchive source, ZipArchive target, Func<ZipArchiveEntry, bool> filter)
        {
            foreach (ZipArchiveEntry sourceEntry in source.Entries.Where(filter))
            {
                ZipArchiveEntry targetEntry = target.CreateEntry(sourceEntry.FullName);
                targetEntry.LastWriteTime = sourceEntry.LastWriteTime;

                using (Stream inStream = sourceEntry.Open())
                using (Stream outStream = targetEntry.Open())
                {
                    inStream.CopyTo(outStream);
                }
            }
        }

        public void Load()
        {
            OutputArchive = FileSystem.OpenArchive(OutputPath, ZipArchiveMode.Update);
        }

        public void Close()
        {
            OutputArchive.Dispose();
            OutputArchive = null;
        }
    }
}