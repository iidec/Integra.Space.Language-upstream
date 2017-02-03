//-----------------------------------------------------------------------
// <copyright file="GroupByLanguageRuntime.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// GroupByLanguageRuntime class
    /// </summary>
    internal class GroupByLanguageRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupByLanguageRuntime"/> class
        /// </summary>
        public GroupByLanguageRuntime()
            : base(new LanguageData(new GroupByGrammar()))
        { 
        }
    }
}
