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
            PlanNode plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<string> result = te.Compile<string>(plan);

            Assert.AreEqual<string>("hello world! :D", result(), "El plan obtenido difiere del plan esperado.");
        }
    }
}
