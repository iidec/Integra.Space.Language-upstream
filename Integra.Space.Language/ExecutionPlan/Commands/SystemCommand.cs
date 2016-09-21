//-----------------------------------------------------------------------
// <copyright file="SystemCommand.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Common;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal abstract class SystemCommand
    {
        /// <summary>
        /// Command action.
        /// </summary>
        private ActionCommandEnum action;

        /// <summary>
        /// Objects specified in the command.
        /// </summary>
        private HashSet<CommandObject> commandObjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObject">Command object.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public SystemCommand(ActionCommandEnum action, CommandObject commandObject, string schemaName, string databaseName)
        {
            Contract.Assert(commandObject != null);

            this.action = action;
            this.SchemaName = schemaName;
            this.DatabaseName = databaseName;
            this.MainCommandObject = commandObject;
            this.commandObjects = new HashSet<CommandObject>(new CommandObjectComparer());
            this.commandObjects.Add(commandObject);

            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                this.commandObjects.Add(new CommandObject(SystemObjectEnum.Schema, schemaName, PermissionsEnum.None, false));
            }

            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                this.commandObjects.Add(new CommandObject(SystemObjectEnum.Database, databaseName, PermissionsEnum.Connect, false));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObjects">Command objects.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public SystemCommand(ActionCommandEnum action, HashSet<CommandObject> commandObjects, string schemaName, string databaseName)
        {
            Contract.Assert(commandObjects != null);
            Contract.Assert(commandObjects.Count > 0);
            Contract.Assert(commandObjects.All(x => !string.IsNullOrWhiteSpace(x.Name)));

            this.action = action;
            this.SchemaName = schemaName;
            this.DatabaseName = databaseName;
            this.commandObjects = commandObjects;

            if (!string.IsNullOrWhiteSpace(schemaName))
            {
                this.commandObjects.Add(new CommandObject(SystemObjectEnum.Schema, schemaName, PermissionsEnum.None, false));
            }

            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                this.commandObjects.Add(new CommandObject(SystemObjectEnum.Database, databaseName, PermissionsEnum.Connect, false));
            }
        }

        /// <summary>
        /// Gets the objects specified in the command.
        /// </summary>
        public virtual HashSet<CommandObject> CommandObjects
        {
            get
            {
                return this.commandObjects;
            }
        }

        /// <summary>
        /// Gets command action.
        /// </summary>
        public ActionCommandEnum Action
        {
            get
            {
                return this.action;
            }
        }

        /// <summary>
        /// Gets the schema of the object.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the database name for the command execution.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the main command object.
        /// </summary>
        public CommandObject MainCommandObject { get; private set; }
    }
}
