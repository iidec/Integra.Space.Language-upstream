//-----------------------------------------------------------------------
// <copyright file="CompilationContext.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System.Linq.Expressions;

    /// <summary>
    /// Compilation context class.
    /// </summary>
    internal class CompilationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationContext"/> class.
        /// </summary>
        public CompilationContext()
        {
        }

        /// <summary>
        /// Gets or sets the actual execution plan node.
        /// </summary>
        public PlanNode ActualNode { get; set; }

        /// <summary>
        /// Gets or sets the result expression.
        /// </summary>
        public Expression ResultExpression { get; set; }
    }
}
