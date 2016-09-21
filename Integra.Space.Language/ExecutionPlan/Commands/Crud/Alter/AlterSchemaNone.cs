//-----------------------------------------------------------------------
// <copyright file="AlterSchemaNone.cs" company="Integra.Space.Common">
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
    internal sealed class AlterSchemaNone : AlterObjectNode<SchemaOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterSchemaNone"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public AlterSchemaNone(CommandObject commandObject, Dictionary<SchemaOptionEnum, object> options, int line, int column, string nodeText, string databaseName) : base(commandObject, options, line, column, nodeText, null, databaseName)
        {
            if (options.ContainsKey(SchemaOptionEnum.Name))
            {
                System.Diagnostics.Contracts.Contract.Assert(options[SchemaOptionEnum.Name].ToString() != commandObject.Name);
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Database, options[SchemaOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }
    }
}
