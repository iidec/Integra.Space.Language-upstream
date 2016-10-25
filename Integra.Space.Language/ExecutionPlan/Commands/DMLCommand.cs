//-----------------------------------------------------------------------
// <copyright file="DMLCommand.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// DML command class.
    /// </summary>
    internal abstract class DMLCommand : CompiledCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DMLCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="commandText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public DMLCommand(ActionCommandEnum action, int line, int column, string commandText, string schemaName, string databaseName) : base(action, new HashSet<CommandObject>(new CommandObjectComparer()), line, column, commandText)
        {
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, databaseName, null, schemaName, PermissionsEnum.None, false));
            }

            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, databaseName, PermissionsEnum.Connect, false));
            }

            this.DatabaseName = databaseName;
            this.SchemaName = schemaName;
        }

        /// <summary>
        /// Gets the database name for the context.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the schema name for the context.
        /// </summary>
        public string SchemaName { get; private set; }
    }
}
