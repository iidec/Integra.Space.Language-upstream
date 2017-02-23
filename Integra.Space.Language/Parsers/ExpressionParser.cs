//-----------------------------------------------------------------------
// <copyright file="ExpressionParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Exceptions;
    using Integra.Space.Language.Grammars;
    using Irony.Parsing;

    /// <summary>
    /// Class that implements the logic to parse conditional expressions
    /// </summary>
    internal sealed class ExpressionParser : SpaceParser<ExpressionGrammar, ExpressionLanguageRuntime, PlanNode>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public ExpressionParser(string commandText) : base(commandText)
        {
        }
    }
}
