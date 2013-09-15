using System.Collections.Generic;
using MVeldhuizen.XapReduce.XapHandling;

namespace MVeldhuizen.XapReduce
{
    public interface IXapMinifier
    {
        void ReduceXap(Options options);
        IList<AssemblyPartInfo> GetRedundantAssemblyParts(string[] sources, XapFile xap);
        void RemoveAssemblyParts(WritableXapFile xap, IEnumerable<AssemblyPartInfo> assemblyParts);
        void RecompressXap(UpdateableXapFile xap);
    }
}