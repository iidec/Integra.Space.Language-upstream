﻿//-----------------------------------------------------------------------
// <copyright file="CommandObject.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Diagnostics.Contracts;
    using Integra.Space.Common;

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
        /// Gets the securable class of the object.
        /// </summary>
        public SystemObjectEnum SecurableClass { get; private set; }

        /// <summary>
        /// Gets the name of the object.
        /// </summary>
        public string Name { get; private set; }

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
    }
}
