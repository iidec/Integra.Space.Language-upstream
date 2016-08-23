//-----------------------------------------------------------------------
// <copyright file="SystemCommand.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Common;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal abstract class SystemCommand
    {
        /// <summary>
        /// Command action.
        /// </summary>
        private ActionCommandEnum action;

        /// <summary>
        /// Space object type.
        /// </summary>
        private SystemObjectEnum spaceObjectType;

        /// <summary>
        /// Space object name.
        /// </summary>
        private string objectName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjectType">Space object type.</param>
        /// <param name="objectName">Object name.</param>
        public SystemCommand(ActionCommandEnum action, SystemObjectEnum spaceObjectType, string objectName)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(objectName));

            this.action = action;
            this.spaceObjectType = spaceObjectType;
            this.objectName = objectName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjectType">Space object type.</param>
        /// <param name="objectName">Object name.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public SystemCommand(ActionCommandEnum action, SystemObjectEnum spaceObjectType, string objectName, string schemaName = null, string databaseName = null)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(objectName));

            this.action = action;
            this.spaceObjectType = spaceObjectType;
            this.objectName = objectName;
            this.SchemaName = schemaName;
            this.DatabaseName = databaseName;
        }

        /// <summary>
        /// Gets the permission value needed to execute the command.
        /// </summary>
        public virtual PermissionsEnum PermissionValue
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets command action.
        /// </summary>
        public ActionCommandEnum Action
        {
            get
            {
                return this.action;
            }
        }

        /// <summary>
        /// Gets the space object type.
        /// </summary>
        public SystemObjectEnum SpaceObjectType
        {
            get
            {
                return this.spaceObjectType;
            }
        }

        /// <summary>
        /// Gets the object name.
        /// </summary>
        public string ObjectName
        {
            get
            {
                return this.objectName;
            }
        }

        /// <summary>
        /// Gets the schema of the object.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the database name for the command execution.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the list of used space object types by the command.
        /// </summary>
        /// <returns>The list of used objects by the command.</returns>
        public virtual HashSet<SystemObjectEnum> GetUsedSpaceObjectTypes()
        {
            HashSet<SystemObjectEnum> listOfUsedObjectTypes = new HashSet<SystemObjectEnum>();
            listOfUsedObjectTypes.Add(this.spaceObjectType);

            return listOfUsedObjectTypes;
        }

        /// <summary>
        /// Gets the list of used space objects by the command.
        /// </summary>
        /// <returns>The list of used objects by the command.</returns>
        public virtual HashSet<Tuple<SystemObjectEnum, string>> GetUsedSpaceObjects()
        {
            HashSet<Tuple<SystemObjectEnum, string>> listOfUsedObjects = new HashSet<Tuple<SystemObjectEnum, string>>(new ObjectUsedComparer());
            if (this.action == ActionCommandEnum.Create)
            {
                listOfUsedObjects.Add(Tuple.Create<SystemObjectEnum, string>(this.spaceObjectType, this.SchemaName));
            }
            else
            {
                listOfUsedObjects.Add(Tuple.Create(this.spaceObjectType, this.objectName));
            }

            return listOfUsedObjects;
        }

        /// <summary>
        /// Object used comparer class.
        /// </summary>
        private class ObjectUsedComparer : IEqualityComparer<Tuple<SystemObjectEnum, string>>
        {
            /// <inheritdoc />
            public bool Equals(Tuple<SystemObjectEnum, string> x, Tuple<SystemObjectEnum, string> y)
            {
                if (x.Item1 == y.Item1 && x.Item2 == y.Item2)
                {
                    return true;
                }

                return false;
            }

            /// <inheritdoc />
            public int GetHashCode(Tuple<SystemObjectEnum, string> obj)
            {
                if (obj.Item2 != null)
                {
                    return obj.Item1.GetHashCode() + obj.Item2.GetHashCode();
                }
                else
                {
                    return obj.Item1.GetHashCode();
                }
            }
        }
    }
}
