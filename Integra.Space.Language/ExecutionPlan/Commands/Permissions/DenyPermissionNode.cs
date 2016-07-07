//-----------------------------------------------------------------------
// <copyright file="DenyPermissionNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Integra.Space.Common;

    /// <summary>
    /// Grant permission node class.
    /// </summary>
    internal class DenyPermissionNode : SpacePermissionsCommandNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DenyPermissionNode"/> class.
        /// </summary>
        /// <param name="spaceObjectType">Space object type..</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        /// <param name="permissions">Space permissions.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public DenyPermissionNode(SpaceObjectEnum spaceObjectType, string toIdentifier, List<SpacePermission> permissions, int line, int column, string nodeText) : base(SpaceActionCommandEnum.Deny, spaceObjectType, toIdentifier, permissions, line, column, nodeText)
        {
        }
    }
}
