using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Compiler;
using Integra.Space.Database;
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
        public void StringConstant()
        {
            ExpressionParser parser = new ExpressionParser("\"hello world! :D\"");
            PlanNode plan = parser.Evaluate();
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGenerator te = new CodeGenerator(this.GetCodeGeneratorConfig(dsf));
            Func<IScheduler,string> result = (Func<IScheduler, string>)te.CompileDelegate(plan);

            Assert.AreEqual<string>("hello world! :D", result(dsf.TestScheduler), "El plan obtenido difiere del plan esperado.");
        }
    }
}
