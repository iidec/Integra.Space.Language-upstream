//-----------------------------------------------------------------------
// <copyright file="AddCommandNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Integra.Space.Common;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal class AddCommandNode : CompiledCommand
    {
        /// <summary>
        /// Database users.
        /// </summary>
        private HashSet<CommandObject> users;

        /// <summary>
        /// Database roles.
        /// </summary>
        private HashSet<CommandObject> roles;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommandNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="roles">Roles objects to assign the permission.</param>
        /// <param name="users">Database users.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public AddCommandNode(ActionCommandEnum action, HashSet<CommandObject> roles, HashSet<CommandObject> users, int line, int column, string nodeText, string schemaName, string databaseName) : base(action, roles, line, column, nodeText, schemaName, databaseName)
        {
            Contract.Assert(roles != null);
            Contract.Assert(users != null);

            this.users = users;
            this.roles = roles;

            // agrego la lista de usuarios al hashset de objetos del comando. En este momento solo tenia en el hashset los roles especificados en el comando.
            users.ToList().ForEach(x => this.CommandObjects.Add(x));
        }

        /// <summary>
        /// Gets the database roles.
        /// </summary>
        public HashSet<CommandObject> Roles
        {
            get
            {
                return this.roles;
            }
        }

        /// <summary>
        /// Gets the database users.
        /// </summary>
        public HashSet<CommandObject> Users
        {
            get
            {
                return this.users;
            }
        }
    }
}
