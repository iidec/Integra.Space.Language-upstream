//-----------------------------------------------------------------------
// <copyright file="CreateAndAlterStreamNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class CreateAndAlterStreamNode : SpaceCrudCommandNode
    {
        /// <summary>
        /// Query string for that will be parsed with the query grammar.
        /// </summary>
        private string query;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAndAlterStreamNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="query">Query string for that will be parsed with the query grammar.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public CreateAndAlterStreamNode(SpaceActionCommandEnum action, string identifier, string query, int line, int column, string nodeText) : base(action, SpaceObjectEnum.Stream, identifier, line, column, nodeText)
        {
            this.query = query;
        }

        /// <summary>
        /// Gets the query of the stream.
        /// </summary>
        public string Query
        {
            get
            {
                return this.query;
            }
        }
    }
}
