using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVeldhuizen.XapReduce.IO;
using MVeldhuizen.XapReduce.Util;
using MVeldhuizen.XapReduce.XapHandling;

namespace MVeldhuizen.XapReduce
{
    internal static class Program
    {
        internal static IFileSystem FileSystem { get; set; }

        internal static IXapMinifier Minifier { get; set; }

        internal static TextWriter Output { get; set; }
        
        static Program()
        {
            FileSystem = RealFileSystem.Instance;
            Minifier = new XapMinifier(FileSystem);
            Output = Console.Out;
        }

        internal static int Main(string[] args)
        {
            Options options = Options.ParseCommandLine(args, Output);
            if (options.Exit)
            {
                return 1;
            }

            int errorCode = CheckCommandLineOptions(options, FileSystem);
            if (errorCode != 0)
            {
                Output.Flush();
                return errorCode;
            }

            try
            {
                Minifier.ReduceXap(options);
            }
            catch (Exception ex)
            {
                Output.WriteLine(Res.Output.UnexpectedException, ex.Message);
                return 1000;
            }

            return 0;
        }

        internal static int CheckCommandLineOptions(Options options, IFileSystem fileSystem)
        {
            if (!fileSystem.FileExists(options.Input))
            {
                Console.WriteLine(Res.Errors.InputFileDoesNotExist, options.Input);
                return 2;
            }

            if (options.Sources == null || options.Sources.Length == 0)
            {
                Console.WriteLine(Res.Errors.AtLeastOneSourceFileRequired);
                return 1;
            }

            var missingFiles = options.Sources.Where(f => !fileSystem.FileExists(f)).ToList();
            if (missingFiles.Count > 0 && !options.IgnoreMissing)
            {
                Console.WriteLine(Res.Errors.SourceFilesMissing, @"  " + String.Join("  \r\n", missingFiles));
                return 3;
            }

            return 0;
        }

        internal static string ReportFileSizeReduction(long oldSize, long newSize)
        {
            return String.Format(Res.Output.FileSizeReduction,
                StorageUtil.PrettyPrintBytes(oldSize),
                StorageUtil.PrettyPrintBytes(newSize),
                StorageUtil.PrettyPrintBytes(oldSize - newSize));
        }
    }
}
