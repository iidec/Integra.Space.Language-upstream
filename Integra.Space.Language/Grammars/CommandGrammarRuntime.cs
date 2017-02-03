//-----------------------------------------------------------------------
// <copyright file="CommandGrammarRuntime.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Command grammar runtime class.
    /// </summary>
    internal class CommandGrammarRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandGrammarRuntime"/> class.
        /// </summary>
        /// <param name="validator">Grammar rule validator.</param>
        public CommandGrammarRuntime(IGrammarRuleValidator validator) : base(new LanguageData(new CommandGrammar(validator)))
        {
        }
    }
}
