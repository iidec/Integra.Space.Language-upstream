using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using System.Reflection;
using System.Linq;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class BooleanNodeTests
    {
        [TestMethod]
        public void ConstantBooleanTrue()
        {
            string eql = "true";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            CompilerConfiguration context = new CompilerConfiguration() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = true };

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithExpressionParser(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            Assert.AreEqual<object>(true, (bool)result.Invoke(queryObject, new object[] { dsf.TestScheduler }), "El plan obtenido difiere del plan esperado.");
        }

        [TestMethod]
        public void ConstantBooleanFalse()
        {
            string eql = "false";
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CompilerConfiguration context = new CompilerConfiguration() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = true };

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithExpressionParser(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            Assert.AreEqual<object>(false, (bool)result.Invoke(queryObject, new object[] { dsf.TestScheduler }), "El plan obtenido difiere del plan esperado.");
        }
    }
}
