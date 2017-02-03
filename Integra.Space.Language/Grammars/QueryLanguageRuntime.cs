//-----------------------------------------------------------------------
// <copyright file="QueryLanguageRuntime.cs" company="Integra.Space.Language">
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
    internal class QueryLanguageRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryLanguageRuntime"/> class
        /// </summary>
        public QueryLanguageRuntime()
            : base(new LanguageData(new QueryGrammar()))
        { 
        }
    }
}
