//-----------------------------------------------------------------------
// <copyright file="AlterStreamNode.cs" company="Integra.Space.Common">
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
    internal sealed class AlterStreamNode : AlterObjectNode<StreamOptionEnum>
    {
        /// <summary>
        /// Referenced sources.
        /// </summary>
        private List<ReferencedSource> referencedSources;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterStreamNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterStreamNode(CommandObject commandObject, Dictionary<StreamOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            this.referencedSources = new List<ReferencedSource>();
            if (options.ContainsKey(StreamOptionEnum.Name))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Stream, commandObject.DatabaseName, commandObject.SchemaName, options[StreamOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterStreamNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="executionPlan">Execution plan.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterStreamNode(CommandObject commandObject, PlanNode executionPlan, Dictionary<StreamOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            this.referencedSources = new List<ReferencedSource>();

            if (options.ContainsKey(StreamOptionEnum.Name))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Stream, commandObject.DatabaseName, commandObject.SchemaName, options[StreamOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }

            if (options.ContainsKey(StreamOptionEnum.Query))
            {
                this.ExecutionPlan = executionPlan;
                this.referencedSources = new List<ReferencedSource>();
                List<PlanNode> fromNodes = Language.Runtime.NodesFinder.FindNode(executionPlan, new PlanNodeTypeEnum[] { PlanNodeTypeEnum.ObservableFrom });
                foreach (PlanNode fromNode in fromNodes)
                {
                    string schemaNameOfSource = commandObject.SchemaName;
                    string databaseNameOfSource = commandObject.DatabaseName;
                    if (fromNode.Properties.ContainsKey("SchemaName") && fromNode.Properties["SchemaName"] != null)
                    {
                        schemaNameOfSource = fromNode.Properties["SchemaName"].ToString();
                    }

                    if (fromNode.Properties.ContainsKey("DatabaseName") && fromNode.Properties["DatabaseName"] != null)
                    {
                        databaseNameOfSource = fromNode.Properties["DatabaseName"].ToString();
                    }

                    string sourceName = fromNode.Properties["SourceName"].ToString();
                    this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Source, databaseNameOfSource, schemaNameOfSource, sourceName, PermissionsEnum.Read, false));

                    this.referencedSources.Add(new ReferencedSource(databaseNameOfSource, schemaNameOfSource, sourceName));
                }
            }
        }

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
