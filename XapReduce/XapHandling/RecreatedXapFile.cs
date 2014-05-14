using System;
using System.Collections.Generic;
using System.IO.Compression;
using MVeldhuizen.XapReduce.IO;

namespace MVeldhuizen.XapReduce.XapHandling
{
    public class RecreatedXapFile : WritableXapFile
    {
        private readonly List<string> _deletedEntries;

        public RecreatedXapFile(string inputPath, string outputPath, IFileSystem fileSystem) : base(inputPath, outputPath, fileSystem)
        {
            _deletedEntries = new List<string>();
        }

        public override void Save()
        {
            if (!HasChanges) return;

            CreateAppManifestEntry();

            using (ZipArchive inputFile = FileSystem.OpenArchive(InputPath, ZipArchiveMode.Read))
            {
                CopyZipEntries(inputFile, OutputArchive, FilterNewXapContent);
            }

            HasChanges = false;
        }

        protected override void RemoveFileEntry(string fileName)
        {
            _deletedEntries.Add(ManifestSourceToEntryName(fileName));
        }

        /// <summary>
        ///     Filters out the undesired entries from the copying process when calling CopyZipEntries method.
        /// </summary>
        /// <param name="entry">ZipArchiveEntry to filter.</param>
        /// <returns>True if entry should be included in the archive.</returns>
        private bool FilterNewXapContent(ZipArchiveEntry entry)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(entry.FullName, "AppManifest.xaml"))
                return false;

            if (_deletedEntries.Contains(entry.FullName))
                return false;

            return true;
        }
    }
}