//-----------------------------------------------------------------------
// <copyright file="RevokePermissionNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Grant permission node class.
    /// </summary>
    internal class RevokePermissionNode : SpacePermissionsCommandNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RevokePermissionNode"/> class.
        /// </summary>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="permission">Space permission.</param>
        /// <param name="spaceObject">Space object.</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        /// <param name="streamIdentifier">Stream identifier.</param>
        public RevokePermissionNode(int line, int column, string nodeText, SpacePermissionsEnum permission, SpaceObjectEnum spaceObject, string toIdentifier, string streamIdentifier = "") : base(line, column, nodeText, permission, spaceObject, toIdentifier, streamIdentifier)
        {
        }
    }
}
