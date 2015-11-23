using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;

namespace Integra.Space.LanguageUnitTests.Constants
{
    [TestClass]
    public class BooleanNodeTests
    {
        [TestMethod]
        public void ConstantBooleanTrue()
        {
            ExpressionParser parser = new ExpressionParser("true");
            PlanNode plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<bool> result = te.Compile<bool>(plan);

            Assert.AreEqual<object>(true, result(), "El plan obtenido difiere del plan esperado.");
        }

        [TestMethod]
        public void ConstantBooleanFalse()
        {
            ExpressionParser parser = new ExpressionParser("false");
            PlanNode plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<bool> result = te.Compile<bool>(plan);

            Assert.AreEqual<object>(false, result(), "El plan obtenido difiere del plan esperado.");
        }
    }
}
