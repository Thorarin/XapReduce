using System;
using System.IO.Compression;

namespace MVeldhuizen.XapReduce.IO
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        long FileSize(string path);
        void FileDelete(string path);
        void FileWriteAllBytes(string path, byte[] buffer);

        ZipArchive OpenArchive(string path, ZipArchiveMode mode);
    }
}
