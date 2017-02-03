//-----------------------------------------------------------------------
// <copyright file="OnASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    /// <summary>
    /// On node class
    /// </summary>
    internal sealed class OnASTNode : ConditionASTNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnASTNode"/> class
        /// </summary>
        public OnASTNode() : base(PlanNodeTypeEnum.ObservableWhere)
        {
        }
    }
}
