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
        public SystemCommand(ActionCommandEnum action, HashSet<CommandObject> commandObjects)
        {
            this.Action = action;
            this.CommandObjects = new HashSet<CommandObject>();

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
        /// Gets command action.
        /// </summary>
        public ActionCommandEnum Action { get; private set; }

        /// <summary>
        /// Gets the objects specified in the command.
        /// </summary>
        public virtual HashSet<CommandObject> CommandObjects { get; private set; }
    }
}
