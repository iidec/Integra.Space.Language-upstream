//-----------------------------------------------------------------------
// <copyright file="PermissionNode.cs" company="Integra.Space">
//     Copyright (c) Integra.Space. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Integra.Space.Common;

    /// <summary>
    /// Space permission class.
    /// </summary>
    internal class PermissionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionNode"/> class.
        /// </summary>
        /// <param name="permission">Space permission.</param>
        public PermissionNode(PermissionsEnum permission)
        {
            this.Permission = permission;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionNode"/> class.
        /// </summary>
        /// <param name="permission">Space permission.</param>
        /// <param name="commandObject">Command object.</param>
        public PermissionNode(PermissionsEnum permission, CommandObject commandObject)
        {
            this.Permission = permission;
            this.CommandObject = commandObject;
        }

        /// <summary>
        /// Gets the permission.
        /// </summary>
        public PermissionsEnum Permission { get; private set; }
        
        /// <summary>
        /// Gets or sets the command object for the permission.
        /// </summary>
        public CommandObject CommandObject { get; set; }
    }
}
