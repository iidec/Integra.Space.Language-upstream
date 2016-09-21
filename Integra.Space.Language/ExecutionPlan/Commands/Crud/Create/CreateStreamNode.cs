//-----------------------------------------------------------------------
// <copyright file="CreateStreamNode.cs" company="Integra.Space.Common">
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
    internal sealed class CreateStreamNode : CreateObjectNode<StreamOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreamNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="query">Query of the stream.</param>
        /// <param name="options">Login options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public CreateStreamNode(CommandObject commandObject, string query, Dictionary<StreamOptionEnum, object> options, int line, int column, string nodeText, string schemaName, string databaseName) : base(commandObject, options, line, column, nodeText, schemaName, databaseName)
        {
            System.Diagnostics.Contracts.Contract.Assert(!string.IsNullOrWhiteSpace(query));
            this.Query = query;
        }

        /// <summary>
        /// Gets the query of the stream.
        /// </summary>
        public string Query { get; private set; }
    }
}
