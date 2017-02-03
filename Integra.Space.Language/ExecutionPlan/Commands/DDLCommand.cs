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
        public DDLCommand(ActionCommandEnum action, CommandObject commandObject, int line, int column, string commandText) : base(action, commandObject, new HashSet<CommandObject>(new CommandObjectComparer()), line, column, commandText)
        {
            Contract.Assert(commandObject != null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DDLCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObjects">Command objects.</param>
        /// <param name="databaseName">Database name where the command will be executed.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="commandText">Text of the actual node.</param>
        public DDLCommand(ActionCommandEnum action, HashSet<CommandObject> commandObjects, string databaseName, int line, int column, string commandText) : base(action, commandObjects, databaseName, line, column, commandText)
        {
            Contract.Assert(commandObjects != null);
            Contract.Assert(commandObjects.Count > 0);
            Contract.Assert(commandObjects.All(x => !string.IsNullOrWhiteSpace(x.Name)));
        }
    }
}
