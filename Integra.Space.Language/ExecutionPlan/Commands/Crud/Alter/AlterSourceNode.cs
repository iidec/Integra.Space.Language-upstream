//-----------------------------------------------------------------------
// <copyright file="AlterSourceNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class AlterSourceNode : AlterObjectNode<SourceOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterSourceNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Source options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterSourceNode(CommandObject commandObject, Dictionary<SourceOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            if (options.ContainsKey(SourceOptionEnum.Name))
            {
                System.Diagnostics.Contracts.Contract.Assert(options[SourceOptionEnum.Name].ToString() != commandObject.Name);
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Source, commandObject.DatabaseName, commandObject.SchemaName, options[SourceOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterSourceNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="columns">Source columns to alter.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterSourceNode(CommandObject commandObject, Tuple<string, List<SourceColumnNode>> columns, int line, int column, string nodeText) : base(commandObject, new Dictionary<SourceOptionEnum, object>(), line, column, nodeText)
        {
            System.Diagnostics.Contracts.Contract.Assert(!string.IsNullOrWhiteSpace(columns.Item1));

            bool? isNew = null;        
            if (columns.Item1.Equals(ActionCommandEnum.Add.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                this.ColumnsToAdd = columns.Item2;
                isNew = true;
            }
            else if (columns.Item1.Equals(ActionCommandEnum.Remove.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                this.ColumnsToRemove = columns.Item2;
                isNew = false;
            }

            foreach (SourceColumnNode sourceColumn in columns.Item2)
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.SourceColumn, commandObject.DatabaseName, commandObject.SchemaName, commandObject.Name, sourceColumn.Name, PermissionsEnum.None, isNew));
            }
        }

        /// <summary>
        /// Gets the columns to add to the existing source.
        /// </summary>
        public List<SourceColumnNode> ColumnsToAdd { get; private set; }

        /// <summary>
        /// Gets the columns to remove to the existing source.
        /// </summary>
        public List<SourceColumnNode> ColumnsToRemove { get; private set; }
    }
}
