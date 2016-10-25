//-----------------------------------------------------------------------
// <copyright file="AlterDatabaseNode.cs" company="Integra.Space.Common">
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
    internal sealed class AlterDatabaseNode : AlterObjectNode<DatabaseOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterDatabaseNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterDatabaseNode(CommandObject commandObject, Dictionary<DatabaseOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            if (options.ContainsKey(DatabaseOptionEnum.Name))
            {
                System.Diagnostics.Contracts.Contract.Assert(options[DatabaseOptionEnum.Name].ToString() != commandObject.Name);
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, commandObject.DatabaseName, commandObject.SchemaName, options[DatabaseOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }
    }
}
