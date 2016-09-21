//-----------------------------------------------------------------------
// <copyright file="AlterUserNode.cs" company="Integra.Space.Common">
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
    internal sealed class AlterUserNode : AlterObjectNode<UserOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterUserNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public AlterUserNode(CommandObject commandObject, Dictionary<UserOptionEnum, object> options, int line, int column, string nodeText, string databaseName) : base(commandObject, options, line, column, nodeText, null, databaseName)
        {
            if (options.ContainsKey(UserOptionEnum.Default_Schema))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, options[UserOptionEnum.Default_Schema].ToString(), PermissionsEnum.None, false));
            }

            if (options.ContainsKey(UserOptionEnum.Name))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.DatabaseUser, options[UserOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }
    }
}
