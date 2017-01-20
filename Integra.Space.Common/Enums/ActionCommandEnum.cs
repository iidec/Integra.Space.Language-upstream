//-----------------------------------------------------------------------
// <copyright file="ActionCommandEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    using System;

    /// <summary>
    /// Command action enumerable.
    /// </summary>
    [Flags]
    internal enum ActionCommandEnum
    {
        /// <summary>
        /// Unspecified command action.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Action command drop.
        /// </summary>
        Drop = 1,

        /// <summary>
        /// Action command create.
        /// </summary>
        Create = 2,

        /// <summary>
        /// Action command alter.
        /// </summary>
        Alter = 4,

        /// <summary>
        /// Action command start.
        /// </summary>
        Start = 8,

        /// <summary>
        /// Action command stop.
        /// </summary>
        Stop = 16,

        /// <summary>
        /// Action command grant.
        /// </summary>
        Grant = 32,

        /// <summary>
        /// Action command deny.
        /// </summary>
        Deny = 64,

        /// <summary>
        /// Action command revoke.
        /// </summary>
        Revoke = 128,

        /// <summary>
        /// Action command read.
        /// </summary>
        Read = 256,

        /// <summary>
        /// Action command add.
        /// </summary>
        Add = 512,

        /// <summary>
        /// Action command use.
        /// </summary>
        Use = 1024,

        /// <summary>
        /// Action take ownership.
        /// </summary>
        TakeOwnership = 2048,

        /// <summary>
        /// Action command query of metadata.
        /// </summary>
        ViewDefinition = 4096,

        /// <summary>
        /// Action command remove.
        /// </summary>
        Remove = 8192,

        /// <summary>
        /// Action command truncate.
        /// </summary>
        Truncate = 16384,

        /// <summary>
        /// Action command insert.
        /// </summary>
        Insert = 32768,

        /// <summary>
        /// Commands for add, delete o edit a space object.
        /// </summary>
        CrudCommands = Create | Alter | Drop,

        /// <summary>
        /// Commands to change the space object status.
        /// </summary>
        StatusCommands = Start | Stop,

        /// <summary>
        /// Commands to manage permissions over space objects.
        /// </summary>
        CommandsPermissions = Grant | Deny | Revoke,
    }
}
