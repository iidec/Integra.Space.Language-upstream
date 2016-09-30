//-----------------------------------------------------------------------
// <copyright file="QueryCommandForMetadataNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Command object class.
    /// </summary>
    internal sealed class QueryCommandForMetadataNode : DMLCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandForMetadataNode"/> class.
        /// </summary>
        /// <param name="action">Command action.</param>
        /// <param name="executionPlan">Execution plan.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public QueryCommandForMetadataNode(Common.ActionCommandEnum action, PlanNode executionPlan, int line, int column, string nodeText, string schemaName, string databaseName) : base(action, line, column, nodeText, schemaName, databaseName)
        {
            Contract.Assert(executionPlan != null);
            this.ExecutionPlan = executionPlan;
        }

        /// <summary>
        /// Gets the execution plan.
        /// </summary>
        public PlanNode ExecutionPlan { get; private set; }
    }
}
