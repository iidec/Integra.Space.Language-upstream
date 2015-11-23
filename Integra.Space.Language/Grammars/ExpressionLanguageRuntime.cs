//-----------------------------------------------------------------------
// <copyright file="ExpressionLanguageRuntime.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// EQLLanguageRuntime class
    /// </summary>
    internal class ExpressionLanguageRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionLanguageRuntime"/> class
        /// </summary>
        public ExpressionLanguageRuntime()
            : base(new LanguageData(new ExpressionGrammar()))
        { 
        }
    }
}
