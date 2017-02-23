using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using Ninject;
using System.Reactive.Concurrency;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class StringNodeTests
    {
        private CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGeneratorConfiguration config = new CodeGeneratorConfiguration(
                dsf,
                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Test"), AssemblyBuilderAccess.RunAndSave),
                kernel,
                printLog: printLog,
                debugMode: debugMode,
                measureElapsedTime: measureElapsedTime,
                isTestMode: isTestMode
                );

            return config;
        }

        [TestMethod]
        public void StringConstant()
        {
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithExpressionParser(context, "\"hello world! :D\"", dsf);
            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            Assert.AreEqual<string>("hello world! :D", (string)result.Invoke(queryObject, new object[] { dsf.TestScheduler }), "El plan obtenido difiere del plan esperado.");
        }
    }
}
