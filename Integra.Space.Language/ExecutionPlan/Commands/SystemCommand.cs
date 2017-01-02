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
        /// <param name="databaseName">Database name for the command execution.</param>
        public SystemCommand(ActionCommandEnum action, HashSet<CommandObject> commandObjects, string databaseName)
        {
            this.Action = action;
            this.CommandObjects = new HashSet<CommandObject>(new CommandObjectComparer());

            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                this.Database = new CommandObject(SystemObjectEnum.Database, databaseName, PermissionsEnum.Connect, false);
                this.CommandObjects.Add(this.Database);
            }

            foreach (CommandObject @object in commandObjects)
            {
                if (!string.IsNullOrWhiteSpace(@object.SchemaName))
                {
                    this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, @object.DatabaseName, null, @object.SchemaName, PermissionsEnum.None, false));
                }

                if (!string.IsNullOrWhiteSpace(@object.DatabaseName))
                {
                    this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, @object.DatabaseName, PermissionsEnum.Connect, false));
                }

                this.CommandObjects.Add(@object);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="mainObject">Command main object.</param>
        /// <param name="commandObjects">Command objects.</param>
        public SystemCommand(ActionCommandEnum action, CommandObject mainObject, HashSet<CommandObject> commandObjects)
        {
            this.Action = action;
            this.CommandObjects = new HashSet<CommandObject>(new CommandObjectComparer());
            this.MainCommandObject = mainObject;

            if (!string.IsNullOrWhiteSpace(mainObject.SchemaName))
            {
                this.Schema = new CommandObject(SystemObjectEnum.Schema, mainObject.DatabaseName, null, mainObject.SchemaName, PermissionsEnum.None, false);
                this.CommandObjects.Add(this.Schema);
            }

            if (!string.IsNullOrWhiteSpace(mainObject.DatabaseName))
            {
                this.Database = new CommandObject(SystemObjectEnum.Database, mainObject.DatabaseName, PermissionsEnum.Connect, false);
                this.CommandObjects.Add(this.Database);
            }

            foreach (CommandObject @object in commandObjects)
            {
                if (!string.IsNullOrWhiteSpace(@object.SchemaName))
                {                    
                    this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, @object.DatabaseName, null, @object.SchemaName, PermissionsEnum.None, false));
                }

                if (!string.IsNullOrWhiteSpace(@object.DatabaseName))
                {
                    this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, @object.DatabaseName, PermissionsEnum.Connect, false));
                }

                this.CommandObjects.Add(@object);
            }

            this.CommandObjects.Add(mainObject);
        }

        /// <summary>
        /// Gets command action.
        /// </summary>
        public ActionCommandEnum Action { get; private set; }

        /// <summary>
        /// Gets the objects specified in the command.
        /// </summary>
        public virtual HashSet<CommandObject> CommandObjects { get; private set; }
        
        /// <summary>
        /// Gets the main command object.
        /// </summary>
        public CommandObject MainCommandObject { get; private set; }

        /// <summary>
        /// Gets the database where the command will be executed.
        /// </summary>
        public CommandObject Database { get; private set; }

        /// <summary>
        /// Gets the schema where the command will be executed.
        /// </summary>
        public CommandObject Schema { get; private set; }
    }
}
