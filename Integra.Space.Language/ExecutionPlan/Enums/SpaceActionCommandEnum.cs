//-----------------------------------------------------------------------
// <copyright file="SpaceActionCommandEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;

    /// <summary>
    /// Command action enumerable.
    /// </summary>
    [Flags]
    internal enum SpaceActionCommandEnum
    {
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
