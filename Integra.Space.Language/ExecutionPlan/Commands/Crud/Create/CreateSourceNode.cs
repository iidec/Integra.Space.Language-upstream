//-----------------------------------------------------------------------
// <copyright file="CreateSourceNode.cs" company="Integra.Space.Common">
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
    internal sealed class CreateSourceNode : CreateObjectNode<SourceOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSourceNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="sourceColumns">Source column list.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public CreateSourceNode(CommandObject commandObject, Dictionary<string, System.Type> sourceColumns, Dictionary<SourceOptionEnum, object> options, int line, int column, string nodeText) : base(commandObject, options, line, column, nodeText)
        {
            this.Columns = sourceColumns;
        }

        /// <summary>
        /// Gets the column list of the source.
        /// </summary>
        public Dictionary<string, System.Type> Columns { get; private set; }
    }
}
