//-----------------------------------------------------------------------
// <copyright file="TemporalStreamNode.cs" company="Integra.Space.Language">
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
    internal sealed class TemporalStreamNode : DMLCommand
    {
        /// <summary>
        /// Referenced sources.
        /// </summary>
        private List<ReferencedSource> referencedSources;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporalStreamNode"/> class.
        /// </summary>
        /// <param name="action">Command action.</param>
        /// <param name="executionPlan">Execution plan.</param>
        /// <param name="source">Source where events will be written</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public TemporalStreamNode(Common.ActionCommandEnum action, PlanNode executionPlan, CommandObject source, int line, int column, string nodeText, string schemaName, string databaseName) : base(action, line, column, nodeText, schemaName, databaseName)
        {
            Contract.Assert(executionPlan != null);
            this.ExecutionPlan = executionPlan;
            this.referencedSources = new List<ReferencedSource>();
            List<PlanNode> fromNodes = Language.Runtime.NodesFinder.FindNode(executionPlan, new PlanNodeTypeEnum[] { PlanNodeTypeEnum.ObservableFrom });
            foreach (PlanNode fromNode in fromNodes)
            {
                string schemaNameOfSource = schemaName;
                string databaseNameOfSource = databaseName;
                if (fromNode.Properties.ContainsKey("SchemaName") && fromNode.Properties["SchemaName"] != null)
                {
                    schemaNameOfSource = fromNode.Properties["SchemaName"].ToString();
                }

                if (fromNode.Properties.ContainsKey("DatabaseName") && fromNode.Properties["DatabaseName"] != null)
                {
                    databaseNameOfSource = fromNode.Properties["DatabaseName"].ToString();
                }

                string sourceName = fromNode.Properties["SourceName"].ToString();
                this.CommandObjects.Add(new CommandObject(Common.SystemObjectEnum.Source, databaseNameOfSource, schemaNameOfSource, sourceName, Common.PermissionsEnum.Read, false));

                this.referencedSources.Add(new ReferencedSource(databaseNameOfSource, schemaNameOfSource, sourceName));
            }

            // agrego la fuente si fue especificada.
            if (source != null)
            {
                this.CommandObjects.Add(source);
                this.Source = source;
            }
        }

        /// <summary>
        /// Gets the referenced sources at the stream query.
        /// </summary>
        public ReferencedSource[] ReferencedSources
        {
            get
            {
                return this.referencedSources.ToArray();
            }
        }

        /// <summary>
        /// Gets the execution plan.
        /// </summary>
        public PlanNode ExecutionPlan { get; private set; }

        /// <summary>
        /// Gets the source where events will be written
        /// </summary>
        public CommandObject Source { get; private set; }

        /// <summary>
        /// Stream referenced sources class.
        /// </summary>
        internal class ReferencedSource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReferencedSource"/> class.
            /// </summary>
            /// <param name="databaseName">Database to which the source belongs.</param>
            /// <param name="schemaName">Schema to which the source belongs.</param>
            /// <param name="sourceName">Source referenced at the stream query.</param>
            public ReferencedSource(string databaseName, string schemaName, string sourceName)
            {
                this.DatabaseName = databaseName;
                this.SchemaName = schemaName;
                this.SourceName = sourceName;
            }

            /// <summary>
            /// Gets the source name.
            /// </summary>
            public string SourceName { get; private set; }

            /// <summary>
            /// Gets the schema name.
            /// </summary>
            public string SchemaName { get; private set; }

            /// <summary>
            /// Gets the database name.
            /// </summary>
            public string DatabaseName { get; private set; }
        }
    }
}
