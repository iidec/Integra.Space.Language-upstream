//-----------------------------------------------------------------------
// <copyright file="TimeoutNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    /// <summary>
    /// TimeoutNode class
    /// </summary>
    internal sealed class TimeoutNode : TimeStatementNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutNode"/> class.
        /// </summary>
        public TimeoutNode() : base(PlanNodeTypeEnum.ObservableTimeout)
        {
        }
    }
}
