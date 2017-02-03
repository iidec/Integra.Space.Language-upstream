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
        /// <param name="databaseName">Database name for the command execution.</param>
        public DMLCommand(ActionCommandEnum action, int line, int column, string commandText, string databaseName) : base(action, new HashSet<CommandObject>(new CommandObjectComparer()), databaseName, line, column, commandText)
        {
            /*
            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, databaseName, null, schemaName, PermissionsEnum.None, false));
            }

            this.SchemaName = schemaName;
            */

            this.DatabaseName = databaseName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DMLCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObject">Command object.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="commandText">Text of the actual node.</param>
        public DMLCommand(ActionCommandEnum action, CommandObject commandObject, int line, int column, string commandText) : base(action, commandObject, new HashSet<CommandObject>(new CommandObjectComparer()), line, column, commandText)
        {
            System.Diagnostics.Contracts.Contract.Assert(commandObject != null);
            this.DatabaseName = commandObject.DatabaseName;
            this.SchemaName = commandObject.SchemaName;

            if (!string.IsNullOrWhiteSpace(this.SchemaName))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, commandObject.DatabaseName, null, commandObject.SchemaName, PermissionsEnum.None, false));
            }

            if (!string.IsNullOrWhiteSpace(this.DatabaseName))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, commandObject.DatabaseName, PermissionsEnum.Connect, false));
            }
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
