//-----------------------------------------------------------------------
// <copyright file="TimeoutASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    /// <summary>
    /// TimeoutNode class
    /// </summary>
    internal sealed class TimeoutASTNode : TimeStatementASTNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutASTNode"/> class.
        /// </summary>
        public TimeoutASTNode() : base(PlanNodeTypeEnum.ObservableTimeout)
        {
        }
    }
}
