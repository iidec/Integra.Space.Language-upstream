//-----------------------------------------------------------------------
// <copyright file="SpaceParseTreeNode.cs" company="Integra.Space.Language.Analysis">
//     Copyright (c) Integra.Space.Language.Analysis. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;

    /// <summary>
    /// Space parse tree node class
    /// </summary>
    public class SpaceParseTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceParseTreeNode"/> class.
        /// </summary>
        /// <param name="type">Node type.</param>
        public SpaceParseTreeNode(SpaceParseTreeNodeTypeEnum type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets or sets the token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the token value data type
        /// </summary>
        public System.Type TokenValueDataType { get; set; } 

        /// <summary>
        /// Gets or sets the token value
        /// </summary>
        public string TokenValue { get; set; }

        /// <summary>
        /// Gets or sets the child nodes of the actual node
        /// </summary>
        public List<SpaceParseTreeNode> ChildNodes { get; set; }

        /// <summary>
        /// Gets the parse tree node type
        /// </summary>
        public SpaceParseTreeNodeTypeEnum Type { get; private set; }
    }
}
