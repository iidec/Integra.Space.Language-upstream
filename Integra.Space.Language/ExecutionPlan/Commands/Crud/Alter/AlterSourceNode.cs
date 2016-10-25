//-----------------------------------------------------------------------
// <copyright file="AlterSourceNode.cs" company="Integra.Space.Common">
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
    internal sealed class AlterSourceNode : AlterObjectNode<SourceOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterSourceNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterSourceNode(CommandObject commandObject, Dictionary<SourceOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            if (options.ContainsKey(SourceOptionEnum.Name))
            {
                System.Diagnostics.Contracts.Contract.Assert(options[SourceOptionEnum.Name].ToString() != commandObject.Name);
                this.CommandObjects.Add(new CommandObject(SystemObjectEnum.Source, commandObject.DatabaseName, commandObject.SchemaName, options[SourceOptionEnum.Name].ToString(), PermissionsEnum.None, true));
            }
        }
    }
}
