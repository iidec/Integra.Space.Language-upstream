//-----------------------------------------------------------------------
// <copyright file="MetadataQueryParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Exceptions;
    using Integra.Space.Language.Grammars;
    using Irony.Parsing;

    /// <summary>
    /// Class that implements the logic to parse commands
    /// </summary>
    internal sealed class MetadataQueryParser : SpaceParser<QueryGrammarForMetadata, QueryLanguageForMetadataRuntime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataQueryParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public MetadataQueryParser(string commandText) : base(commandText)
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
