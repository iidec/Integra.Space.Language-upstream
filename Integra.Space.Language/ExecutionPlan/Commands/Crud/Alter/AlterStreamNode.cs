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
        private List<ReferencedSource> inputSources;

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
            this.inputSources = new List<ReferencedSource>();
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
        /// <param name="sourceInto">Source where events will be written</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterStreamNode(CommandObject commandObject, PlanNode executionPlan, Dictionary<StreamOptionEnum, object> options, CommandObject sourceInto, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            this.inputSources = new List<ReferencedSource>();

            if (options.ContainsKey(StreamOptionEnum.Name))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Stream, commandObject.DatabaseName, commandObject.SchemaName, options[StreamOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }

            if (options.ContainsKey(StreamOptionEnum.Query))
            {
                this.ExecutionPlan = executionPlan;
                this.inputSources = new List<ReferencedSource>();
                List<PlanNode> fromNodes = NodesFinder.FindNode(executionPlan, new PlanNodeTypeEnum[] { PlanNodeTypeEnum.ObservableFrom });
                foreach (PlanNode fromNode in fromNodes)
                {
                    CommandObject inputSource = (CommandObject)fromNode.Properties["Source"];
                    this.CommandObjects.Add(inputSource);
                    string sourceAlias = null;
                    if (fromNode.Properties.ContainsKey("SourceAlias"))
                    {
                        sourceAlias = (string)fromNode.Properties["SourceAlias"];
                    }

                    this.inputSources.Add(new ReferencedSource(inputSource.DatabaseName, inputSource.SchemaName, inputSource.Name, sourceAlias));
                }
            }

            // agrego la fuente si fue especificada.
            this.CommandObjects.Add(sourceInto);
            this.OutputSource = sourceInto;
        }

        /// <summary>
        /// Gets the execution plan of the query.
        /// </summary>
        public PlanNode ExecutionPlan { get; private set; }

        /// <summary>
        /// Gets the source where events will be written
        /// </summary>
        public CommandObject OutputSource { get; private set; }

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
    }
}
