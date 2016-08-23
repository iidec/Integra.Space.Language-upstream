//-----------------------------------------------------------------------
// <copyright file="AddCommandNode.cs" company="Integra.Space.Common">
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
    internal class AddCommandNode : CompiledCommand
    {
        /// <summary>
        /// Space permissions.
        /// </summary>
        private List<Tuple<string, SystemObjectEnum>> usersAndRoles;
        
        /// <summary>
        /// Space object to assign the permission.
        /// </summary>
        private string toIdentifier;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCommandNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjectType">Space object type..</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        /// <param name="userAndRoles">Space permissions.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AddCommandNode(ActionCommandEnum action, SystemObjectEnum spaceObjectType, string toIdentifier, List<Tuple<string, SystemObjectEnum>> userAndRoles, int line, int column, string nodeText) : base(action, spaceObjectType, toIdentifier, line, column, nodeText)
        {
            this.usersAndRoles = userAndRoles;
            this.toIdentifier = toIdentifier;
        }

        /// <inheritdoc />
        public override PermissionsEnum PermissionValue
        {
            get
            {
                if (this.Action == ActionCommandEnum.Add)
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
        public List<Tuple<string, SystemObjectEnum>> UsersAndRoles
        {
            get
            {
                return this.usersAndRoles;
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

            this.usersAndRoles
                .Where(x => !string.IsNullOrWhiteSpace(x.Item1))
                .Select(x => x.Item2)
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

            this.usersAndRoles
                .Where(x => !string.IsNullOrWhiteSpace(x.Item1))
                .Distinct(new PermissionComparer())
                .ToList()
                .ForEach(permission =>
                {
                    listOfUsedObjects.Add(Tuple.Create(permission.Item2, permission.Item1));
                });

            return listOfUsedObjects;
        }

        /// <summary>
        /// Object used comparer class.
        /// </summary>
        private class PermissionComparer : IEqualityComparer<Tuple<string, SystemObjectEnum>>
        {
            /// <inheritdoc />
            public bool Equals(Tuple<string, SystemObjectEnum> x, Tuple<string, SystemObjectEnum> y)
            {
                if (x.Item1 == y.Item1 && x.Item2 == y.Item2)
                {
                    return true;
                }

                return false;
            }

            /// <inheritdoc />
            public int GetHashCode(Tuple<string, SystemObjectEnum> obj)
            {
                return obj.Item1.GetHashCode() + obj.Item2.GetHashCode();
            }
        }
    }
}
