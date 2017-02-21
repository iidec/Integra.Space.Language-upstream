using Integra.Space.Language;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Integra.Space.LanguageUnitTests.Commands
{
    [TestClass]
    public class BatchTests : BaseTest
    {
        [TestMethod]
        public void TestGo()
        {
            this.Process("go");
        }

        [TestMethod]
        public void TestBatchWithoutGo()
        {
            string command = $@"create login endpoint1 with password = ""pass123"";
                                create login login1 with password = ""pass123"", status = on;
                                create login login2 with password = ""pass123"", status = off";

            this.Process(command);
        }

        [TestMethod]
        public void TestBatchWothGo()
        {
            string command = $@"create login endpoint1 with password = ""pass123"";
                                create login login1 with password = ""pass123"", status = on;
                                create login login2 with password = ""pass123"", status = off;
                                go";

            this.Process(command);
        }

        [TestMethod]
        public void TestBatchWothGoWithCounter()
        {
            string command = $@"create login endpoint1 with password = ""pass123"";
                                create login login1 with password = ""pass123"", status = on;
                                create login login2 with password = ""pass123"", status = off;
                                go 3";

            this.Process(command);
        }


        [TestMethod]
        public void TestBatchWothGo2()
        {
            string command = $@"create login endpoint1 with password = ""pass123"";
                                create login login1 with password = ""pass123"", status = on;
                                go;
                                create login login2 with password = ""pass123"", status = off";

            this.Process(command);
        }
    }
}
