//-----------------------------------------------------------------------
// <copyright file="OnNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    /// <summary>
    /// On node class
    /// </summary>
    internal sealed class OnNode : ConditionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OnNode"/> class
        /// </summary>
        public OnNode() : base(PlanNodeTypeEnum.ObservableWhere)
        {
        }
    }
}
