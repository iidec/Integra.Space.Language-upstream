//-----------------------------------------------------------------------
// <copyright file="CommandObject.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Command object class.
    /// </summary>
    internal sealed class CommandObject
    {
        /// <summary>
        /// Value indicating whether the object is a new one or not.
        /// </summary>
        private bool? isNew;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandObject"/> class.
        /// </summary>
        /// <param name="securableClass">Securable class.</param>
        /// <param name="name">Name of the object.</param>
        /// <param name="granularPermission">Granular permission.</param>
        /// <param name="isNew">Value indicating whether the object is new or not.</param>
        public CommandObject(SystemObjectEnum securableClass, string name, PermissionsEnum granularPermission, bool? isNew)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(name));

            this.SecurableClass = securableClass;
            this.Name = name;
            this.GranularPermission = granularPermission;
            this.isNew = isNew;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandObject"/> class.
        /// </summary>
        /// <param name="securableClass">Securable class.</param>
        /// <param name="databaseName">Database name of the object.</param>
        /// <param name="schemaName">Schema name of the object.</param>
        /// <param name="name">Name of the object.</param>
        /// <param name="granularPermission">Granular permission.</param>
        /// <param name="isNew">Value indicating whether the object is new or not.</param>
        public CommandObject(SystemObjectEnum securableClass, string databaseName, string schemaName, string name, PermissionsEnum granularPermission, bool? isNew)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(name));

            this.SecurableClass = securableClass;
            this.Name = name;
            this.SchemaName = schemaName;
            this.DatabaseName = databaseName;
            this.GranularPermission = granularPermission;
            this.isNew = isNew;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandObject"/> class.
        /// </summary>
        /// <param name="securableClass">Securable class.</param>
        /// <param name="databaseName">Database name of the object.</param>
        /// <param name="schemaName">Schema name of the object.</param>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="granularObjectName">Name of the granular object contained inside the system object</param>
        /// <param name="granularPermission">Granular permission.</param>
        /// <param name="isNew">Value indicating whether the object is new or not.</param>
        public CommandObject(SystemObjectEnum securableClass, string databaseName, string schemaName, string objectName, string granularObjectName, PermissionsEnum granularPermission, bool? isNew)
        {
            Contract.Assert(!string.IsNullOrWhiteSpace(objectName));
            Contract.Assert(!string.IsNullOrWhiteSpace(granularObjectName));

            this.SecurableClass = securableClass;
            this.Name = objectName;
            this.GranularObjectName = granularObjectName;
            this.SchemaName = schemaName;
            this.DatabaseName = databaseName;
            this.GranularPermission = granularPermission;
            this.isNew = isNew;
        }

        /// <summary>
        /// Gets the securable class of the object.
        /// </summary>
        public SystemObjectEnum SecurableClass { get; private set; }

        /// <summary>
        /// Gets the object name.
        /// </summary>
        public string GranularObjectName { get; private set; }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the schema name of the object.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the database name of the object.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets or sets the required granular permission to execute the command that contains this object.
        /// </summary>
        public PermissionsEnum GranularPermission { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is new or not.
        /// </summary>
        public bool IsNew
        {
            get
            {
                if (this.isNew == null)
                {
                    throw new System.Exception("Must define if the object is new or not.");
                }

                return this.isNew.Value;
            }

            set
            {
                this.isNew = value;
            }
        }

        /// <summary>
        /// Gets the path of the system object.
        /// </summary>
        /// <returns>Path of the system object.</returns>
        public string GetStringPath()
        {
            string path = this.Name;

            if (!string.IsNullOrWhiteSpace(this.GranularObjectName))
            {
                path += "." + this.GranularObjectName;
            }

            if (!string.IsNullOrWhiteSpace(this.SchemaName))
            {
                path = string.Concat(this.SchemaName, ".", path);
            }

            if (!string.IsNullOrWhiteSpace(this.DatabaseName))
            {
                path = string.Concat(this.DatabaseName, ".", path);
            }

            return path;
        }
    }
}
