//-----------------------------------------------------------------------
// <copyright file="SystemSourceEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    using System;

    /// <summary>
    /// Space object enumerable.
    /// </summary>
    internal enum SystemSourceEnum
    {
        /// <summary>
        /// Space object source.
        /// </summary>
        Sources = 1,

        /// <summary>
        /// Space object stream.
        /// </summary>
        Streams = 2,

        /// <summary>
        /// Space object view.
        /// </summary>
        Views = 4,

        /// <summary>
        /// Space object schema
        /// </summary>
        Schemas = 8,

        /// <summary>
        /// Space object user.
        /// </summary>
        Users = 16,

        /// <summary>
        /// Space object role
        /// </summary>
        DatabaseRoles = 32,

        /// <summary>
        /// Space object database.
        /// </summary>
        Databases = 64,

        /// <summary>
        /// Space object server role.
        /// </summary>
        ServerRoles = 128,

        /// <summary>
        /// Space object login.
        /// </summary>
        Logins = 256,

        /// <summary>
        /// Space object endpoint.
        /// </summary>
        Endpoints = 512,

        /// <summary>
        /// Space object server.
        /// </summary>
        Servers = 1024,

        /// <summary>
        /// Space object: source column.
        /// </summary>
        SourceColumns = 2048,

        /// <summary>
        /// Space object: stream column.
        /// </summary>
        StreamColumns = 4096
    }
}
