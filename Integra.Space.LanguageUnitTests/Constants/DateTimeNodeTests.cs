﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;

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

            CodeGenerator te = new CodeGenerator(new CompilerConfiguration() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
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

            CodeGenerator te = new CodeGenerator(new CompilerConfiguration() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<TimeSpan> result = (Func<TimeSpan>)te.CompileDelegate(plan);

            TimeSpan parsedDate;
            TimeSpan.TryParse("00:00:00:01", out parsedDate);

            Assert.AreEqual<TimeSpan>(parsedDate, result(), "El plan obtenido difiere del plan esperado.");
        }
    }
}
