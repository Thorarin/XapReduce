using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MVeldhuizen.XapReduce.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MVeldhuizen.XapReduce.Util.Tests
{
    [TestClass]
    public class StorageUtilTests
    {
        [TestMethod]
        public void PrettyPrintBytes_NumberBelow1024_ShowsBytes()
        {
            string actual = StorageUtil.PrettyPrintBytes(1023);
            Assert.AreEqual("1023 B", actual);
        }

        [TestMethod]
        public void PrettyPrintBytes_NumberBelow1048576_ShowsKiloBytes()
        {
            string actual = StorageUtil.PrettyPrintBytes(1048575);
            Assert.AreEqual("1023 KB", actual);
        }

        [TestMethod]
        public void PrettyPrintBytes_Number1048576_ShowsMegaBytes()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            string actual = StorageUtil.PrettyPrintBytes(1048576);
            Assert.AreEqual("1.0 MB", actual);
        }
    }
}
