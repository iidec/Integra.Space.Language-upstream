//-----------------------------------------------------------------------
// <copyright file="CreateStreamNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class CreateStreamNode : CreateObjectNode<StreamOptionEnum>
    {
        /// <summary>
        /// Referenced sources.
        /// </summary>
        private List<ReferencedSource> referencedSources;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreamNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="query">Query of the stream.</param>
        /// <param name="executionPlan">Execution plan.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public CreateStreamNode(CommandObject commandObject, string query, PlanNode executionPlan, Dictionary<StreamOptionEnum, object> options, int line, int column, string nodeText, string schemaName, string databaseName) : base(commandObject, options, line, column, nodeText, schemaName, databaseName)
        {
            System.Diagnostics.Contracts.Contract.Assert(!string.IsNullOrWhiteSpace(query));
            this.Query = query;
            this.ExecutionPlan = executionPlan;

            this.referencedSources = new List<ReferencedSource>();
            List<PlanNode> fromNodes = Language.Runtime.NodesFinder.FindNode(executionPlan, new PlanNodeTypeEnum[] { PlanNodeTypeEnum.ObservableFrom });
            foreach (PlanNode fromNode in fromNodes)
            {
                string sourceName = fromNode.Properties["SourceName"].ToString();
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Source, sourceName, PermissionsEnum.Read, false));

                string schemaNameOfSource = null;
                if (fromNode.Properties.ContainsKey("SchemaName") && fromNode.Properties["SchemaName"] != null)
                {
                    schemaNameOfSource = fromNode.Properties["SchemaName"].ToString();
                }

                this.referencedSources.Add(new ReferencedSource(schemaNameOfSource, sourceName));
            }
        }

        /// <summary>
        /// Gets the query of the stream.
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Gets the execution plan of the query.
        /// </summary>
        public PlanNode ExecutionPlan { get; private set; }

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
        /// Stream referenced sources class.
        /// </summary>
        internal class ReferencedSource
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ReferencedSource"/> class.
            /// </summary>
            /// <param name="schemaName">Schema to witch the source belongs.</param>
            /// <param name="sourceName">Source referenced at the stream query.</param>
            public ReferencedSource(string schemaName, string sourceName)
            {
                this.SchemaName = schemaName;
                this.SourceName = sourceName;
            }

            /// <summary>
            /// Gets the source name.
            /// </summary>
            protected string SourceName { get; private set; }

            /// <summary>
            /// Gets the schema name.
            /// </summary>
            protected string SchemaName { get; private set; }
        }
    }
}
