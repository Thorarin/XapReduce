using System;
using System.IO;
using System.IO.Compression;

namespace MVeldhuizen.XapReduce.IO
{
    public class RealFileSystem : IFileSystem
    {
        private static readonly RealFileSystem _instance = new RealFileSystem();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RealFileSystem()
        {
        }

        public static RealFileSystem Instance
        {
            get
            {
                return _instance;
            }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public long FileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public void FileDelete(string path)
        {
            File.Delete(path);
        }

        public ZipArchive OpenArchive(string path, ZipArchiveMode mode)
        {
            return ZipFile.Open(path, mode);
        }

        public void FileWriteAllBytes(string path, byte[] buffer)
        {
            File.WriteAllBytes(path, buffer);
        }
    }
}
