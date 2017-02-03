//-----------------------------------------------------------------------
// <copyright file="GrantPermissionToRoleNode.cs" company="Integra.Space.Common">
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
    internal class GrantPermissionToRoleNode : GrantPermissionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrantPermissionToRoleNode"/> class.
        /// </summary>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="permission">Space permission.</param>
        /// <param name="spaceObject">Space object.</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        public GrantPermissionToRoleNode(int line, int column, string nodeText, List<SpacePermission> permission, SpaceObjectEnum spaceObject, string toIdentifier) : base(SpaceObjectEnum.Role, toIdentifier, permission, line, column, nodeText)
        {
        }
    }
}
