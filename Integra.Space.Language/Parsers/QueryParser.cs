//-----------------------------------------------------------------------
// <copyright file="QueryParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using Common;
    using Grammars;

    /// <summary>
    /// Class that implements the logic to parse commands
    /// </summary>
    internal sealed class QueryParser : SpaceParser<QueryGrammar, QueryLanguageRuntime, Tuple<PlanNode, CommandObject>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public QueryParser(string commandText) : base(commandText)
        {
        }
    }
}
