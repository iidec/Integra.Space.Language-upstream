//-----------------------------------------------------------------------
// <copyright file="DDLCommand.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Common;

    /// <summary>
    /// DDL command class.
    /// </summary>
    internal abstract class DDLCommand : CompiledCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DDLCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObject">Command object.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="commandText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public DDLCommand(ActionCommandEnum action, CommandObject commandObject, int line, int column, string commandText, string schemaName, string databaseName) : base(action, new HashSet<CommandObject>(new CommandObjectComparer()), line, column, commandText, schemaName, databaseName)
        {
            Contract.Assert(commandObject != null);
            
            this.MainCommandObject = commandObject;
            this.CommandObjects.Add(commandObject);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DDLCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObjects">Command objects.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="commandText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public DDLCommand(ActionCommandEnum action, HashSet<CommandObject> commandObjects, int line, int column, string commandText, string schemaName, string databaseName) : base(action, commandObjects, line, column, commandText, schemaName, databaseName)
        {
            Contract.Assert(commandObjects != null);
            Contract.Assert(commandObjects.Count > 0);
            Contract.Assert(commandObjects.All(x => !string.IsNullOrWhiteSpace(x.Name)));
        }
        
        /// <summary>
        /// Gets the main command object.
        /// </summary>
        public CommandObject MainCommandObject { get; private set; }
    }
}
