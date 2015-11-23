//-----------------------------------------------------------------------
// <copyright file="ProjectionLanguageRuntime.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// ProjectionLanguageRuntime class
    /// </summary>
    internal class ProjectionLanguageRuntime : LanguageRuntime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionLanguageRuntime"/> class
        /// </summary>
        public ProjectionLanguageRuntime()
            : base(new LanguageData(new ProjectionGrammar()))
        { 
        }
    }
}
