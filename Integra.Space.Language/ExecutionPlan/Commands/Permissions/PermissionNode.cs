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
        /// <param name="objectType">Space objet type.</param>
        /// <param name="objectName">Object name.</param>
        public PermissionNode(PermissionsEnum permission, SystemObjectEnum objectType, string objectName)
        {
            this.Permission = permission;
            this.ObjectType = objectType;
            this.ObjectName = objectName;
        }

        /// <summary>
        /// Gets the permission.
        /// </summary>
        public PermissionsEnum Permission { get; private set; }

        /// <summary>
        /// Gets or sets the object type.
        /// </summary>
        public SystemObjectEnum? ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the object name.
        /// </summary>
        public string ObjectName { get; set; }
    }
}
