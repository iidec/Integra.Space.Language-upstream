﻿//-----------------------------------------------------------------------
// <copyright file="AlterRoleNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class AlterRoleNode : AlterObjectNode<RoleOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterRoleNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public AlterRoleNode(CommandObject commandObject, Dictionary<RoleOptionEnum, object> options, int line, int column, string nodeText, string databaseName) : base(commandObject, options, line, column, nodeText, null, databaseName)
        {
            if (options.ContainsKey(RoleOptionEnum.Name))
            {
                System.Diagnostics.Contracts.Contract.Assert(options[RoleOptionEnum.Name].ToString() != commandObject.Name);
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, options[RoleOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }
    }
}
