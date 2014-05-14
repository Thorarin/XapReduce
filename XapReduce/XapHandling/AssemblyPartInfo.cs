using System;

namespace MVeldhuizen.XapReduce.XapHandling
{
    public struct AssemblyPartInfo
    {
        private readonly string _fileName;
        private readonly string _assemblyName;
        private readonly long _size;

        public AssemblyPartInfo(string assemblyName, string fileName, long size)
        {
            _assemblyName = assemblyName;
            _fileName = fileName;
            _size = size;
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public long Size
        {
            get { return _size; }
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
