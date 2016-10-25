//-----------------------------------------------------------------------
// <copyright file="CreateLoginNode.cs" company="Integra.Space.Common">
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
    internal sealed class CreateLoginNode : CreateObjectNode<LoginOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateLoginNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public CreateLoginNode(CommandObject commandObject, Dictionary<LoginOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            if (options.ContainsKey(LoginOptionEnum.Default_Database))
            {
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, options[LoginOptionEnum.Default_Database].ToString(), PermissionsEnum.None, false));
            }
        }
    }
}
