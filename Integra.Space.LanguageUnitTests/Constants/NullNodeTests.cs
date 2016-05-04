using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class NullNodeTests
    {
        [TestMethod]
        public void ConstantNull()
        {
            ExpressionParser parser = new ExpressionParser("null");
            PlanNode plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<object> result = te.Compile<object>(plan);

            Assert.AreEqual<object>(null, result(), "El plan obtenido difiere del plan esperado.");
        }
    }
}
