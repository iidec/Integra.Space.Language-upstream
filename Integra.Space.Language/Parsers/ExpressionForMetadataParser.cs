//-----------------------------------------------------------------------
// <copyright file="ExpressionForMetadataParser.cs" company="Integra.Space.Language">
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
    internal sealed class ExpressionForMetadataParser : SpaceParser<ExpressionGrammarForMetadata, ExpressionLanguageForMetadataRuntime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionForMetadataParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public ExpressionForMetadataParser(string commandText) : base(commandText)
        {
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <returns>Execution plan.</returns>
        public PlanNode Evaluate()
        {
            return (PlanNode)this.EvaluateParseTree();
        }
    }
}
