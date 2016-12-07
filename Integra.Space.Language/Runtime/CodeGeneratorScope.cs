//-----------------------------------------------------------------------
// <copyright file="CodeGeneratorScope.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System.Linq.Expressions;

    /// <summary>
    /// Compilation context class.
    /// </summary>
    internal class CodeGeneratorScope
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorScope"/> class.
        /// </summary>
        public CodeGeneratorScope()
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
