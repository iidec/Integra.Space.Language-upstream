﻿//-----------------------------------------------------------------------
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
        private List<PermissionNode> permissions;

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
        public SpacePermissionsCommandNode(ActionCommandEnum action, SystemObjectEnum spaceObjectType, string toIdentifier, List<PermissionNode> permissions, int line, int column, string nodeText) : base(action, spaceObjectType, toIdentifier, line, column, nodeText)
        {
            this.permissions = permissions;
            this.toIdentifier = toIdentifier;
        }

        /// <inheritdoc />
        public override PermissionsEnum PermissionValue
        {
            get
            {
                if (this.Action == ActionCommandEnum.Grant)
                {
                    return PermissionsEnum.Control;
                }
                else if (this.Action == ActionCommandEnum.Revoke)
                {
                    return PermissionsEnum.Control;
                }
                else if (this.Action == ActionCommandEnum.Deny)
                {
                    return PermissionsEnum.Control;
                }
                else
                {
                    throw new System.Exception("Invalid action for the command.");
                }
            }
        }

        /// <summary>
        /// Gets the space permissions.
        /// </summary>
        public List<PermissionNode> Permissions
        {
            get
            {
                return this.permissions;
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
        public override HashSet<SystemObjectEnum> GetUsedSpaceObjectTypes()
        {
            HashSet<SystemObjectEnum> listOfUsedObjects = base.GetUsedSpaceObjectTypes();

            this.permissions
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
        public override HashSet<Tuple<SystemObjectEnum, string>> GetUsedSpaceObjects()
        {
            HashSet<Tuple<SystemObjectEnum, string>> listOfUsedObjects = base.GetUsedSpaceObjects();

            // se limpia la lista para que no tenga el usuario o role especificado en el comando de permiso
            // para que solo tenga los objetos de la lista de permisos del comando.
            // listOfUsedObjects.Clear();
            this.permissions
                .Where(x => x.ObjectName != null)
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
        private class PermissionComparer : IEqualityComparer<PermissionNode>
        {
            /// <inheritdoc />
            public bool Equals(PermissionNode x, PermissionNode y)
            {
                if (x.ObjectName == y.ObjectName && x.ObjectType == y.ObjectType)
                {
                    return true;
                }

                return false;
            }

            /// <inheritdoc />
            public int GetHashCode(PermissionNode obj)
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
