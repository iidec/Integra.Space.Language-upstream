namespace Integra.Space.LanguageUnitTests
{
    using Integra.Space.Language;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

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
                foreach (var batch in context.Batches)
                {
                    if (batch.HasErrors())
                    {
                        Assert.Fail();
                        throw new Exception();
                    }
                }
            }
        }
    }
}
