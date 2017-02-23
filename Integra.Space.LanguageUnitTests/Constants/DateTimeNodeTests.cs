using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Compiler;
using System.Reflection;
using Ninject;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class DateTimeNodeTests : BaseTest
    {
        [TestMethod]
        public void ConstantDateTimeValue()
        {
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithExpressionParser(context, "'01/01/2014'", dsf);            
            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            DateTime parsedDate;
            DateTime.TryParse("01/01/2014", out parsedDate);

            Assert.AreEqual<DateTime>(parsedDate, (DateTime)result.Invoke(queryObject, new object[] { dsf.TestScheduler }), "El plan obtenido difiere del plan esperado.");
        }

        [TestMethod]
        public void ConstantTimeSpanValue()
        {
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithExpressionParser(context, "'00:00:00:01'", dsf);
            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            TimeSpan parsedDate;
            TimeSpan.TryParse("00:00:00:01", out parsedDate);

            Assert.AreEqual<TimeSpan>(parsedDate, (TimeSpan)result.Invoke(queryObject, new object[] { dsf.TestScheduler }), "El plan obtenido difiere del plan esperado.");
        }
    }
}
