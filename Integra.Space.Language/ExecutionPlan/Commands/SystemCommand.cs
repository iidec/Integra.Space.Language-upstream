//-----------------------------------------------------------------------
// <copyright file="SystemCommand.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// System command class.
    /// </summary>
    internal abstract class SystemCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObjects">Command objects.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public SystemCommand(ActionCommandEnum action, HashSet<CommandObject> commandObjects, string schemaName, string databaseName)
        {
            this.Action = action;
            this.CommandObjects = commandObjects;
            this.SchemaName = schemaName;
            this.DatabaseName = databaseName;

            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, schemaName, PermissionsEnum.None, false));
            }

            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, databaseName, PermissionsEnum.Connect, false));
            }
        }

        /// <summary>
        /// Gets command action.
        /// </summary>
        public ActionCommandEnum Action { get; private set; }

        /// <summary>
        /// Gets the schema of the object.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the database name for the command execution.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the objects specified in the command.
        /// </summary>
        public virtual HashSet<CommandObject> CommandObjects { get; private set; }
    }
}
