//-----------------------------------------------------------------------
// <copyright file="SpaceObjectEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;

    /// <summary>
    /// Space object enumerable.
    /// </summary>
    [Flags]
    internal enum SpaceObjectEnum
    {
        /// <summary>
        /// Space object source.
        /// </summary>
        Source = 1,

        /// <summary>
        /// Space object stream.
        /// </summary>
        Stream = 2,

        /// <summary>
        /// Space object user.
        /// </summary>
        User = 4,

        /// <summary>
        /// Space object role
        /// </summary>
        Role = 8,

        /// <summary>
        /// Objects that can assign permissions.
        /// </summary>
        PermissionAssignableObjects = User | Role
    }
}
