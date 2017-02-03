//-----------------------------------------------------------------------
// <copyright file="CodeGeneratorContext.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using Language;
    using System.Linq.Expressions;

    /// <summary>
    /// Compilation context class.
    /// </summary>
    internal class CodeGeneratorContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorContext"/> class.
        /// </summary>
        public CodeGeneratorContext()
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
