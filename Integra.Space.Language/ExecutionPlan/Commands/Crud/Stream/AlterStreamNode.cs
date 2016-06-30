//-----------------------------------------------------------------------
// <copyright file="AlterStreamNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class AlterStreamNode : AlterObjectNode
    {
        /// <summary>
        /// Query string for that will be parsed with the query grammar.
        /// </summary>
        private string query;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterStreamNode"/> class.
        /// </summary>
        /// <param name="query">Query string for that will be parsed with the query grammar.</param>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterStreamNode(string query, string identifier, int line, int column, string nodeText) : base(identifier, line, column, nodeText)
        {
            this.query = query;
        }
    }
}
