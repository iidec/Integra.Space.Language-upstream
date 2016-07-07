//-----------------------------------------------------------------------
// <copyright file="SpacePermissionsCommandNode.cs" company="Integra.Space.Common">
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
    internal class SpacePermissionsCommandNode : CompiledCommand
    {
        /// <summary>
        /// Space permissions.
        /// </summary>
        private List<SpacePermission> permissions;

        /// <summary>
        /// Space object.
        /// </summary>
        private SpaceObjectEnum spaceObject;

        /// <summary>
        /// Space object to assign the permission.
        /// </summary>
        private string toIdentifier;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SpacePermissionsCommandNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjectType">Space object type..</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        /// <param name="permissions">Space permissions.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public SpacePermissionsCommandNode(SpaceActionCommandEnum action, SpaceObjectEnum spaceObjectType, string toIdentifier, List<SpacePermission> permissions, int line, int column, string nodeText) : base(action, spaceObjectType, toIdentifier, line, column, nodeText)
        {
            this.permissions = permissions;
            this.spaceObject = spaceObjectType;
            this.toIdentifier = toIdentifier;
        }

        /// <summary>
        /// Gets the space permissions.
        /// </summary>
        public List<SpacePermission> Permissions
        {
            get
            {
                return this.permissions;
            }
        }

        /// <summary>
        /// Gets the space object.
        /// </summary>
        public SpaceObjectEnum SpaceObject
        {
            get
            {
                return this.spaceObject;
            }
        }

        /// <summary>
        /// Gets the space object to assign the permission.
        /// </summary>
        public string ToIdentifier
        {
            get
            {
                return this.toIdentifier;
            }
        }

        /// <inheritdoc />
        public override HashSet<SpaceObjectEnum> GetUsedSpaceObjectTypes()
        {
            HashSet<SpaceObjectEnum> listOfUsedObjects = base.GetUsedSpaceObjectTypes();

            this.permissions
                .Where(x => !string.IsNullOrWhiteSpace(x.ObjectName))
                .Select(x => x.ObjectType)
                .Except(listOfUsedObjects)
                .ToList()
                .ForEach(objectType =>
                {
                    listOfUsedObjects.Add(objectType);
                });

            return listOfUsedObjects;
        }

        /// <inheritdoc />
        public override HashSet<Tuple<SpaceObjectEnum, string>> GetUsedSpaceObjects()
        {
            HashSet<Tuple<SpaceObjectEnum, string>> listOfUsedObjects = base.GetUsedSpaceObjects();

            this.permissions
                .Where(x => !string.IsNullOrWhiteSpace(x.ObjectName))
                .Distinct(new PermissionComparer())
                .ToList()
                .ForEach(permission =>
                {
                    listOfUsedObjects.Add(Tuple.Create(permission.ObjectType, permission.ObjectName));
                });

            return listOfUsedObjects;
        }

        /// <summary>
        /// Object used comparer class.
        /// </summary>
        private class PermissionComparer : IEqualityComparer<SpacePermission>
        {
            /// <inheritdoc />
            public bool Equals(SpacePermission x, SpacePermission y)
            {
                if (x.ObjectName == y.ObjectName && x.ObjectType == y.ObjectType)
                {
                    return true;
                }

                return false;
            }

            /// <inheritdoc />
            public int GetHashCode(SpacePermission obj)
            {
                if (obj.ObjectName != null)
                {
                    return obj.ObjectType.GetHashCode() + obj.ObjectName.GetHashCode();
                }
                else
                {
                    return obj.ObjectType.GetHashCode();
                }
            }
        }
    }
}
