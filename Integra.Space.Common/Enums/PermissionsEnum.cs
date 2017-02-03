//-----------------------------------------------------------------------
// <copyright file="PermissionsEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    /// <summary>
    /// Space permissions enumerable.
    /// </summary>
    internal enum PermissionsEnum
    {
        /// <summary>
        /// Permission none.
        /// </summary>
        None = 0,

        /// <summary>
        /// Permission control server.
        /// </summary>
        ControlServer = 1,

        /// <summary>
        /// Permission create view.
        /// </summary>
        CreateView = 2,

        /// <summary>
        /// Permission create source.
        /// </summary>
        CreateSource = 3,

        /// <summary>
        /// Permission create stream.
        /// </summary>
        CreateStream = 4,

        /// <summary>
        /// Permission create schema.
        /// </summary>
        CreateSchema = 5,

        /// <summary>
        /// Permission create role.
        /// </summary>
        CreateRole = 6,

        /// <summary>
        /// Permission create endpoint.
        /// </summary>
        CreateEndpoint = 7,

        /// <summary>
        /// Permission create database.
        /// </summary>
        CreateDatabase = 8,

        /// <summary>
        /// Permission authenticate server.
        /// </summary>
        AutheticateServer = 9,

        /// <summary>
        /// Permission authenticate.
        /// </summary>
        Authenticate = 10,

        /// <summary>
        /// Permission view definition.
        /// </summary>
        ViewDefinition = 11,

        /// <summary>
        /// Permission take ownership.
        /// </summary>
        TakeOwnership = 12,

        /// <summary>
        /// Permission read.
        /// </summary>
        Read = 13,

        /// <summary>
        /// Permission control.
        /// </summary>
        Control = 14,

        /// <summary>
        /// Permission connect.
        /// </summary>
        Connect = 15,

        /// <summary>
        /// Permission alter.
        /// </summary>
        Alter = 16,
        
        /// <summary>
        /// Permission view any definition.
        /// </summary>
        ViewAnyDefinition = 17,

        /// <summary>
        /// Permission view any database.
        /// </summary>
        ViewAnyDatabase = 18,

        /// <summary>
        /// Permission connect any database.
        /// </summary>
        ConnectAnyDatabase = 19,

        /// <summary>
        /// Permission create any database.
        /// </summary>
        CreateAnyDatabase = 20,

        /// <summary>
        /// Permission view any user.
        /// </summary>
        AlterAnyUser = 21,

        /// <summary>
        /// Permission view any schema.
        /// </summary>
        AlterAnySchema = 22,

        /// <summary>
        /// Permission view any role.
        /// </summary>
        AlterAnyRole = 23,

        /// <summary>
        /// Permission view any login.
        /// </summary>
        AlterAnyLogin = 24,

        /// <summary>
        /// Permission view any endpoint.
        /// </summary>
        AlterAnyEndpoint = 25,

        /// <summary>
        /// Permission view any database.
        /// </summary>
        AlterAnyDatabase = 26,

        /// <summary>
        /// Permission status, for start and stop sources and streams.
        /// </summary>
        AlterStatus = 27,

        /// <summary>
        /// Connect SQL for logins.
        /// </summary>
        ConnectSQL = 28,

        /// <summary>
        /// Permission write.
        /// </summary>
        Write = 29
    }
}
