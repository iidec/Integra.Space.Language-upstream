//-----------------------------------------------------------------------
// <copyright file="QueryLanguageForMetadataRuntime.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// QueryLanguageForMetadataRuntime class
    /// </summary>
    internal class QueryLanguageForMetadataRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryLanguageForMetadataRuntime"/> class
        /// </summary>
        public QueryLanguageForMetadataRuntime()
            : base(new LanguageData(new QueryGrammarForMetadata()))
        { 
        }
    }
}
