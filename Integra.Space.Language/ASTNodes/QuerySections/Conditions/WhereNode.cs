//-----------------------------------------------------------------------
// <copyright file="WhereNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    /// <summary>
    /// WhereNode class
    /// </summary>
    internal sealed class WhereNode : ConditionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhereNode"/> class
        /// </summary>
        public WhereNode() : base(PlanNodeTypeEnum.ObservableWhere)
        {
        }
    }
}
