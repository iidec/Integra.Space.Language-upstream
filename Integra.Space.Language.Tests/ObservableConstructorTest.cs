// <copyright file="ObservableConstructorTest.cs" company="Integra.Space.Language">Copyright ©  2014</copyright>
using System;
using System.Linq.Expressions;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Integra.Space.Language.Runtime.Tests
{
    /// <summary>This class contains parameterized unit tests for ObservableConstructor</summary>
    [PexClass(typeof(ObservableConstructor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ObservableConstructorTest
    {
        /// <summary>Test stub for GenerateExpressionTree(PlanNode)</summary>
        [PexMethod]
        internal Expression GenerateExpressionTreeTest([PexAssumeUnderTest]ObservableConstructor target, PlanNode plan)
        {
            Expression result = target.GenerateExpressionTree(plan);
            return result;
            // TODO: add assertions to method ObservableConstructorTest.GenerateExpressionTreeTest(ObservableConstructor, PlanNode)
        }
    }
}
