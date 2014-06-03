using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVeldhuizen.XapReduce.IO;
using MVeldhuizen.XapReduce.Res;
using MVeldhuizen.XapReduce.Util;
using MVeldhuizen.XapReduce.XapHandling;

namespace MVeldhuizen.XapReduce
{
    internal class XapMinifier : IXapMinifier
    {
        private readonly IFileSystem _fileSystem;
        private readonly TextWriter _console;

        public XapMinifier(IFileSystem fileSystem) : this(fileSystem, Console.Out)
        {
        }

        public XapMinifier(IFileSystem fileSystem, TextWriter console)
        {
            _fileSystem = fileSystem;
            _console = console;
        }

        public void ReduceXap(Options options)
        {
            long oldSize = _fileSystem.FileSize(options.Input);

            WritableXapFile xap;

            if (options.Output != null)
            {
                xap = new RecreatedXapFile(options.Input, options.Output, _fileSystem);
            }
            else
            {
                xap = new UpdateableXapFile(options.Input, _fileSystem);
            }
            
            using (xap)
            {
                // Get a list of redundant assemblies: identical files that exist in both input and source XAPs
                List<AssemblyPartInfo> redundantAssemblyParts = GetRedundantAssemblyParts(options.Sources, xap).ToList();

                // Get satellite assemblies (resources) for redundant assemblies                
                var redundantResources = redundantAssemblyParts.SelectMany(ap => GetResourceAssemblyParts(ap, xap)).ToList();
                
                // Merge the two lists. Note that it can contain duplicates when a resource file exists in both
                // input and source XAPs, while at the same time the main assembly is also redundant.
                redundantAssemblyParts = redundantAssemblyParts.Union(redundantResources).ToList();
                
                _console.WriteLine(Output.RedundantAssemblyParts, redundantAssemblyParts.Count);
                _console.Write(Environment.NewLine);

                RemoveAssemblyParts(xap, redundantAssemblyParts);
                xap.Save();
                xap.Close();

                long newSize = _fileSystem.FileSize(options.Input);

                _console.Write(Environment.NewLine);
                _console.WriteLine(Program.ReportFileSizeReduction(oldSize, newSize));

                if (options.Recompress)
                {
                    RecompressXap((UpdateableXapFile)xap);
                }
            }            
        }

        public IList<AssemblyPartInfo> GetRedundantAssemblyParts(string[] sources, XapFile xap)
        {
            var sourceXaps =
                sources.Where(_fileSystem.FileExists).
                    Select(file => new XapFile(file, _fileSystem)).ToList();

            var redundantAssemblyParts = sourceXaps.
                SelectMany(source => source.AssemblyParts).Distinct().Intersect(xap.AssemblyParts).
                ToList();

            return redundantAssemblyParts.Distinct().ToList();
        }

        public List<AssemblyPartInfo> GetResourceAssemblyParts(AssemblyPartInfo assemblyPart, XapFile xap)
        {
            if (assemblyPart.AssemblyName == null)
            {
                return new List<AssemblyPartInfo>();
            }

            var resourceFileName = "/" + Path.GetFileNameWithoutExtension(assemblyPart.FileName) + ".resources.dll";
            return xap.AssemblyParts.Where(ap => ap.FileName.EndsWith(resourceFileName)).ToList();
        }

        public void RemoveAssemblyParts(WritableXapFile xap, IEnumerable<AssemblyPartInfo> assemblyParts)
        {
            foreach (AssemblyPartInfo assemblyPart in assemblyParts)
            {
                xap.RemoveAssemblyPart(assemblyPart);

                _console.WriteLine(
                    Output.RemovedAssemblyPart,
                    assemblyPart.FileName, StorageUtil.PrettyPrintBytes(assemblyPart.Size));
            }            
        }

        public void RecompressXap(UpdateableXapFile xap)
        {
            xap.Load();
            Tuple<long, long> recompressed = xap.Recompress();
            if (recompressed.Item1 > recompressed.Item2)
            {
                _console.WriteLine(Output.RecompressSucceeded, 
                    StorageUtil.PrettyPrintBytes(recompressed.Item1),
                    StorageUtil.PrettyPrintBytes(recompressed.Item1 - recompressed.Item2));
            }
            else
            {
                _console.WriteLine(Output.RecompressCanceled);
            }            
        }
    }
}