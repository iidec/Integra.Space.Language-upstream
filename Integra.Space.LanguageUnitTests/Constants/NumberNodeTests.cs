using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class NumberNodeTests
    {
        [TestMethod]
        public void ConstantIntegerValue()
        {
            ExpressionParser parser = new ExpressionParser("10");
            PlanNode plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor();
            Func<int> result = te.Compile<int>(plan);

            Assert.AreEqual<int>(10, result(), "El plan obtenido difiere del plan esperado.");
        }
    }
}
