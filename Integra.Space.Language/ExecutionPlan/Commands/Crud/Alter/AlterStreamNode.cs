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
        /// Initializes a new instance of the <see cref="AlterStreamNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public AlterStreamNode(CommandObject commandObject, Dictionary<StreamOptionEnum, object> options, int line, int column, string nodeText, string schemaName, string databaseName) : base(commandObject, options, line, column, nodeText, schemaName, databaseName)
        {
            if (options.ContainsKey(StreamOptionEnum.Name))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Stream, options[StreamOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }
    }
}
