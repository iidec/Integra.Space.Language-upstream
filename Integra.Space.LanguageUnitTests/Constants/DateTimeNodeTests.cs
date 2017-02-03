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
    public class DateTimeNodeTests
    {
        [TestMethod]
        public void ConstantDateTimeValue()
        {
            ExpressionParser parser = new ExpressionParser("'01/01/2014'");
            PlanNode plan = parser.Evaluate();
            
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGenerator te = new CodeGenerator(new CodeGeneratorConfiguration(new DefaultSchedulerFactory(), AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Test"), System.Reflection.Emit.AssemblyBuilderAccess.Run), kernel, printLog: true));
            Func<DateTime> result = (Func<DateTime>)te.CompileDelegate(plan);

            DateTime parsedDate;
            DateTime.TryParse("01/01/2014", out parsedDate);

            Assert.AreEqual<DateTime>(parsedDate, result(), "El plan obtenido difiere del plan esperado.");
        }

        [TestMethod]
        public void ConstantTimeSpanValue()
        {
            ExpressionParser parser = new ExpressionParser("'00:00:00:01'");
            PlanNode plan = parser.Evaluate();
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());

            CodeGenerator te = new CodeGenerator(new CodeGeneratorConfiguration(new DefaultSchedulerFactory(), AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Test"), System.Reflection.Emit.AssemblyBuilderAccess.Run), kernel, printLog: true));
            Func<TimeSpan> result = (Func<TimeSpan>)te.CompileDelegate(plan);

            TimeSpan parsedDate;
            TimeSpan.TryParse("00:00:00:01", out parsedDate);

            Assert.AreEqual<TimeSpan>(parsedDate, result(), "El plan obtenido difiere del plan esperado.");
        }
    }
}
