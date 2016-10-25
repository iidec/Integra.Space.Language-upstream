//-----------------------------------------------------------------------
// <copyright file="AddOrRemoveCommandNode.cs" company="Integra.Space.Common">
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
    internal class AddOrRemoveCommandNode : DDLCommand
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
        /// Initializes a new instance of the <see cref="AddOrRemoveCommandNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObjects">Command objects</param>
        /// <param name="roles">Roles objects to assign the permission.</param>
        /// <param name="users">Database users.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AddOrRemoveCommandNode(ActionCommandEnum action, HashSet<CommandObject> commandObjects, HashSet<CommandObject> roles, HashSet<CommandObject> users, int line, int column, string nodeText) : base(action, commandObjects, line, column, nodeText)
        {
            Contract.Assert(roles != null);
            Contract.Assert(users != null);

            this.users = users;
            this.roles = roles;

            // agrego la lista de roles al hashset de objetos del comando.
            roles.ToList().ForEach(x => this.CommandObjects.Add(x));

            // agrego la lista de usuarios al hashset de objetos del comando.
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
