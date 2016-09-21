//-----------------------------------------------------------------------
// <copyright file="ExpressionLanguageForMetadataRuntime.cs" company="Integra.Space.Language">
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
    internal class ExpressionLanguageForMetadataRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionLanguageForMetadataRuntime"/> class
        /// </summary>
        public ExpressionLanguageForMetadataRuntime()
            : base(new LanguageData(new ExpressionGrammarForMetadata()))
        { 
        }
    }
}
