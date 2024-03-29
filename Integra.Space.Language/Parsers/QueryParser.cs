﻿//-----------------------------------------------------------------------
// <copyright file="QueryParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Common;
    using Grammars;

    /// <summary>
    /// Class that implements the logic to parse commands
    /// </summary>
    internal sealed class QueryParser : SpaceParser<QueryGrammar, QueryLanguageRuntime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public QueryParser(string commandText) : base(commandText)
        {
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <param name="parameters">Binding parameters.</param>
        /// <returns>Execution plan.</returns>
        public System.Tuple<PlanNode, CommandObject> Evaluate(params BindingParameter[] parameters)
        {
            return (System.Tuple<PlanNode, CommandObject>)this.EvaluateParseTree(parameters);
        }
    }
}
