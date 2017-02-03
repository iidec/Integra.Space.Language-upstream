//-----------------------------------------------------------------------
// <copyright file="SystemObjectEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    using System;

    /// <summary>
    /// Space object enumerable.
    /// </summary>
    internal enum SystemObjectEnum
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
        /// Space object view.
        /// </summary>
        View = 4,

        /// <summary>
        /// Space object schema
        /// </summary>
        Schema = 8,

        /// <summary>
        /// Space object user.
        /// </summary>
        DatabaseUser = 16,

        /// <summary>
        /// Space object role
        /// </summary>
        DatabaseRole = 32,

        /// <summary>
        /// Space object database.
        /// </summary>
        Database = 64,

        /// <summary>
        /// Space object server role.
        /// </summary>
        ServerRole = 128,

        /// <summary>
        /// Space object login.
        /// </summary>
        Login = 256,

        /// <summary>
        /// Space object endpoint.
        /// </summary>
        Endpoint = 512,

        /// <summary>
        /// Space object server.
        /// </summary>
        Server = 1024,

        /// <summary>
        /// Space object: source column.
        /// </summary>
        SourceColumn = 2048,

        /// <summary>
        /// Space object: stream column.
        /// </summary>
        StreamColumn = 4096
    }
}
