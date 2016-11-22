//-----------------------------------------------------------------------
// <copyright file="TemporalStreamLanguageRuntime.cs" company="Integra.Space.Language">
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
    internal class TemporalStreamLanguageRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemporalStreamLanguageRuntime"/> class
        /// </summary>
        public TemporalStreamLanguageRuntime()
            : base(new LanguageData(new TemporalStreamGrammar()))
        { 
        }
    }
}
