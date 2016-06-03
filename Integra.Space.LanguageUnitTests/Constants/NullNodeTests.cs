using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using System.Reflection;
using System.Linq;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class NullNodeTests
    {
        [TestMethod]
        public void ConstantNull()
        {
            string eql = "null";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            CompileContext context = new CompileContext() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = true };

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithExpressionParser(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            object queryObject = Activator.CreateInstance(types.Last());
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            Assert.AreEqual<object>(null, result.Invoke(queryObject, new object[] { dsf.TestScheduler }), "El plan obtenido difiere del plan esperado.");
        }
    }
}
