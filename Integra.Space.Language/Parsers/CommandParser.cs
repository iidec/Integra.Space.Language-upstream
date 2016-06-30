﻿//-----------------------------------------------------------------------
// <copyright file="CommandParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using CommandContext;
    using Exceptions;
    using Integra.Space.Language.Grammars;
    using Irony.Parsing;

    /// <summary>
    /// Class that implements the logic to parse conditional expressions
    /// </summary>
    internal sealed class CommandParser : SpaceParser<CommandGrammar, CommandGrammarRuntime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public CommandParser(string commandText) : base(commandText)
        {
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <returns>Execution plan.</returns>
        public PipelineCommandContext Evaluate()
        {
            return (PipelineCommandContext)this.EvaluateParseTree();
        }
    }
}