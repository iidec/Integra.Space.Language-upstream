//-----------------------------------------------------------------------
// <copyright file="DenyPermissionToRoleNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Integra.Space.Common;

    /// <summary>
    /// Grant permission node class.
    /// </summary>
    internal class DenyPermissionToRoleNode : DenyPermissionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenyPermissionToRoleNode"/> class.
        /// </summary>
        /// <param name="spaceObject">Space object.</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        /// <param name="permission">Space permission.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public DenyPermissionToRoleNode(SpaceObjectEnum spaceObject, string toIdentifier, SpacePermissionsEnum permission, int line, int column, string nodeText) : base(SpaceObjectEnum.Role, toIdentifier, permission, line, column, nodeText)
        {
        }
    }
}
