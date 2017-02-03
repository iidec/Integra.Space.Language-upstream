//-----------------------------------------------------------------------
// <copyright file="QueryCommandForMetadataNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Command object class.
    /// </summary>
    internal sealed class QueryCommandForMetadataNode : DMLCommand
    {
        /// <summary>
        /// Referenced sources.
        /// </summary>
        private List<ReferencedSource> inputSources;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommandForMetadataNode"/> class.
        /// </summary>
        /// <param name="action">Command action.</param>
        /// <param name="executionPlan">Execution plan.</param>
        /// <param name="outputSource">Source where events will be written</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public QueryCommandForMetadataNode(Common.ActionCommandEnum action, PlanNode executionPlan, Common.CommandObject outputSource, int line, int column, string nodeText, string databaseName) : base(action, line, column, nodeText, databaseName)
        {
            Contract.Assert(executionPlan != null);
            this.ExecutionPlan = executionPlan;            
            this.OutputSource = outputSource;

            this.inputSources = new List<ReferencedSource>();
            List<PlanNode> fromNodes = NodesFinder.FindNode(executionPlan, new PlanNodeTypeEnum[] { PlanNodeTypeEnum.ObservableFrom });
            foreach (PlanNode fromNode in fromNodes)
            {
                Common.CommandObject inputSource = (Common.CommandObject)fromNode.Properties["Source"];
                this.CommandObjects.Add(inputSource);
                string sourceAlias = null;
                if (fromNode.Properties.ContainsKey("SourceAlias"))
                {
                    sourceAlias = (string)fromNode.Properties["SourceAlias"];
                }

                this.inputSources.Add(new ReferencedSource(inputSource.DatabaseName, inputSource.SchemaName, inputSource.Name, sourceAlias));
            }
        }

        /// <summary>
        /// Gets the referenced sources at the stream query.
        /// </summary>
        public ReferencedSource[] InputSources
        {
            get
            {
                return this.inputSources.ToArray();
            }
        }

        /// <summary>
        /// Gets the execution plan.
        /// </summary>
        public PlanNode ExecutionPlan { get; private set; }

        /// <summary>
        /// Gets the source where events will be written
        /// </summary>
        public Common.CommandObject OutputSource { get; private set; }        
    }
}
