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
    public class NumberNodeTests
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
        public void ConstantIntegerValue()
        {
            ExpressionParser parser = new ExpressionParser("10");
            PlanNode plan = parser.Evaluate();
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            CodeGenerator te = new CodeGenerator(this.GetCodeGeneratorConfig(dsf));
            Func<IScheduler, int> result = (Func<IScheduler, int>)te.CompileDelegate(plan);

            Assert.AreEqual<int>(10, result(dsf.TestScheduler), "El plan obtenido difiere del plan esperado.");
        }
    }
}
