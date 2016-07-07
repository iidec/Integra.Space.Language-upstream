//-----------------------------------------------------------------------
// <copyright file="DenyPermissionToUserNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Common;

    /// <summary>
    /// Grant permission node class.
    /// </summary>
    internal class DenyPermissionToUserNode : DenyPermissionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenyPermissionToUserNode"/> class.
        /// </summary>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="permission">Space permission.</param>
        /// <param name="spaceObject">Space object.</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        public DenyPermissionToUserNode(int line, int column, string nodeText, SpacePermissionsEnum permission, SpaceObjectEnum spaceObject, string toIdentifier) : base(SpaceObjectEnum.User, toIdentifier, permission, line, column, nodeText)
        {
        }
    }
}
