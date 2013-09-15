using System;
using System.IO;
using System.IO.Compression;
using MVeldhuizen.XapReduce.IO;

namespace MVeldhuizen.XapReduce.XapHandling
{
    public class UpdateableXapFile : WritableXapFile
    {
        public UpdateableXapFile(string outputPath, IFileSystem fileSystem) : base(outputPath, fileSystem)
        {
        }

        public override void Save()
        {
            if (!HasChanges) return;

            ZipArchiveEntry oldManifestEntry = OutputArchive.GetEntry("AppManifest.xaml");
            oldManifestEntry.Delete();

            CreateAppManifestEntry();

            HasChanges = false;
        }

        protected override void RemoveFileEntry(string fileName)
        {
            OutputArchive.GetEntry(fileName).Delete();
        }

        /// <summary>
        ///     Attempts to recompress an existing XAP file. If this results in a smaller file, it is replaced.
        /// </summary>
        /// <returns>true if file was succesfully recompressed.</returns>
        public Tuple<long, long> Recompress()
        {
            if (HasChanges)
            {
                throw new InvalidOperationException("Archive has pending changes. Save it first before attempting recompression.");
            }

            using (var ms = new MemoryStream())
            {
                using (var recompressed = new ZipArchive(ms, ZipArchiveMode.Create))
                {
                    CopyZipEntries(OutputArchive, recompressed, entry => true);
                }

                byte[] buffer = ms.ToArray();
                long existingLength = FileSystem.FileSize(OutputPath);

                if (buffer.Length < existingLength)
                {
                    Close();
                    FileSystem.FileWriteAllBytes(OutputPath, buffer);
                    Load();

                    return Tuple.Create(existingLength, buffer.LongLength);
                }

                return Tuple.Create(existingLength, existingLength);
            }
        }
    }
}