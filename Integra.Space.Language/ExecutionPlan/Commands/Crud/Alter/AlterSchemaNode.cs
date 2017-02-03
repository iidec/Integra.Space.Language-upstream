//-----------------------------------------------------------------------
// <copyright file="AlterSchemaNode.cs" company="Integra.Space.Common">
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
    internal sealed class AlterSchemaNode : AlterObjectNode<SchemaOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterSchemaNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterSchemaNode(CommandObject commandObject, Dictionary<SchemaOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            if (options.ContainsKey(SchemaOptionEnum.Name))
            {
                System.Diagnostics.Contracts.Contract.Assert(options[SchemaOptionEnum.Name].ToString() != commandObject.Name);
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Schema, commandObject.DatabaseName, commandObject.SchemaName, options[SchemaOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }
    }
}
