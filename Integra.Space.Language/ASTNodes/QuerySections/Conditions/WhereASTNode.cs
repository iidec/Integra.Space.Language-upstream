//-----------------------------------------------------------------------
// <copyright file="WhereASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    /// <summary>
    /// WhereNode class
    /// </summary>
    internal sealed class WhereASTNode : ConditionASTNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhereASTNode"/> class
        /// </summary>
        public WhereASTNode() : base(PlanNodeTypeEnum.ObservableWhere)
        {
        }
    }
}
