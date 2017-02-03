using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using Integra.Space.Compiler;
using Integra.Space.Database;
using System.Reflection.Emit;
using Ninject;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class BooleanNodeTests
    {
        private CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            Login login = new SpaceDbContext().Logins.First();
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGeneratorConfiguration config = new CodeGeneratorConfiguration(
                login,
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
        public void ConstantBooleanTrue()
        {
            string eql = "true";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);

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
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);

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
