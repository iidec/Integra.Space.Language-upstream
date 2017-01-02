//-----------------------------------------------------------------------
// <copyright file="PermissionsCommandNode.cs" company="Integra.Space.Language">
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
    internal class PermissionsCommandNode : DDLCommand
    {
        /// <summary>
        /// Space permissions.
        /// </summary>
        private PermissionNode permission;

        /// <summary>
        /// Space object to assign the permission.
        /// </summary>
        private HashSet<CommandObject> principals;

        /// <summary>
        /// With grant option flag.
        /// </summary>
        private bool permissionOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionsCommandNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="principals">Principals for the permissions.</param>
        /// <param name="databaseName">Database name where the command will be executed.</param>
        /// <param name="permission">Space permission.</param>
        /// <param name="permissionOption">Permission option.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public PermissionsCommandNode(ActionCommandEnum action, HashSet<CommandObject> principals, string databaseName, PermissionNode permission, bool permissionOption, int line, int column, string nodeText) : base(action, principals, databaseName, line, column, nodeText)
        {
            this.permission = permission;
            this.principals = new HashSet<CommandObject>(new CommandObjectComparer());
            foreach (CommandObject commandObject in principals)
            {
                this.principals.Add(commandObject);
            }

            this.permissionOption = permissionOption;

            // se agregan los objetos de los permisos al listado de objetos del comando.
            if (permission.CommandObject != null)
            {
                this.CommandObjects.Add(permission.CommandObject);
            }
        }

        /// <summary>
        /// Gets the space permissions.
        /// </summary>
        public PermissionNode Permission
        {
            get
            {
                return this.permission;
            }
        }

        /// <summary>
        /// Gets the space object to assign the permission.
        /// </summary>
        public IEnumerable<CommandObject> Principals
        {
            get
            {
                return this.principals;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the permission option is present at the command.
        /// </summary>
        public bool PermissionOption
        {
            get
            {
                return this.permissionOption;
            }
        }
    }
}
