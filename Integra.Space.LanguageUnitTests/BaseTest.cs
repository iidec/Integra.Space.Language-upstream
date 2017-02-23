namespace Integra.Space.LanguageUnitTests
{
    using Compiler;
    using Integra.Space.Language;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Ninject;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [TestClass]
    public abstract class BaseTest
    {
        public void Process(string command)
        {
            CommandParser cp = new CommandParser(command, new TestRuleValidator());
            ParseContext context = cp.Evaluate();

            if (context.HasErrors())
            {
                Assert.Fail();
                throw new Exception();
            }
            else
            {
                foreach (var batch in context.Payload)
                {
                    if (batch.HasErrors())
                    {
                        Assert.Fail();
                        throw new Exception();
                    }
                }
            }
        }

        internal CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf, bool printLog = false, bool debugMode = false, bool measureElapsedTime = false, bool isTestMode = true)
        {
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
    }
}
