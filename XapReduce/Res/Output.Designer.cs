﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVeldhuizen.XapReduce.Res {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Output {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Output() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MVeldhuizen.XapReduce.Res.Output", typeof(Output).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to B.
        /// </summary>
        internal static string Bytes {
            get {
                return ResourceManager.GetString("Bytes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Old file size: {0}, New file size: {1}, Saved: {2}.
        /// </summary>
        internal static string FileSizeReduction {
            get {
                return ResourceManager.GetString("FileSizeReduction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to KB.
        /// </summary>
        internal static string KibiBytes {
            get {
                return ResourceManager.GetString("KibiBytes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MB.
        /// </summary>
        internal static string MebiBytes {
            get {
                return ResourceManager.GetString("MebiBytes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recompression did not further reduce the file size..
        /// </summary>
        internal static string RecompressCanceled {
            get {
                return ResourceManager.GetString("RecompressCanceled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File recompressed. New file size: {0}, Saved: {1}.
        /// </summary>
        internal static string RecompressSucceeded {
            get {
                return ResourceManager.GetString("RecompressSucceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Found {0} redundant assembly parts..
        /// </summary>
        internal static string RedundantAssemblyParts {
            get {
                return ResourceManager.GetString("RedundantAssemblyParts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Removed {0} ({1} uncompressed).
        /// </summary>
        internal static string RemovedAssemblyPart {
            get {
                return ResourceManager.GetString("RemovedAssemblyPart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred while processing the file:
        ///{0}.
        /// </summary>
        internal static string UnexpectedException {
            get {
                return ResourceManager.GetString("UnexpectedException", resourceCulture);
            }
        }
    }
}
