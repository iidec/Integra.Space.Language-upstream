using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class StringNodeTests
    {
        [TestMethod]
        public void StringConstant()
        {
            ExpressionParser parser = new ExpressionParser("\"hello world! :D\"");
            PlanNode plan = parser.Evaluate();

            CodeGenerator te = new CodeGenerator(new CompilerConfiguration() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<string> result = (Func<string>)te.CompileDelegate(plan);

            Assert.AreEqual<string>("hello world! :D", result(), "El plan obtenido difiere del plan esperado.");
        }
    }
}
