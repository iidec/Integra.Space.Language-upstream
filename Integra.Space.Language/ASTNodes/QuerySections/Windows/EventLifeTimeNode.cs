//-----------------------------------------------------------------------
// <copyright file="EventLifeTimeNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    /// <summary>
    /// EventLifeTimeNode class
    /// </summary>
    internal sealed class EventLifeTimeNode : TimeStatementNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventLifeTimeNode"/> class.
        /// </summary>
        public EventLifeTimeNode() : base(PlanNodeTypeEnum.EventLifeTime)
        {
        }
    }
}
