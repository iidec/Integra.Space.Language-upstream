//-----------------------------------------------------------------------
// <copyright file="SpaceASTNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Space abstract syntax tree node class.
    /// </summary>
    internal abstract class SpaceASTNode : ISpaceASTNode
    {
        /// <summary>
        /// Line of the evaluated sentence.
        /// </summary>
        private int line;

        /// <summary>
        /// Column evaluated sentence column.
        /// </summary>
        private int column;

        /// <summary>
        /// Text of the actual node.
        /// </summary>
        private string nodeText;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceASTNode"/> class.
        /// </summary>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public SpaceASTNode(int line, int column, string nodeText)
        {
            this.line = line;
            this.column = column;
            this.nodeText = nodeText;
        }

        /// <summary>
        /// Gets the line of the evaluated sentence.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column evaluated sentence column.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets the text of the actual node.
        /// </summary>
        public string NodeText { get; }

        /// <summary>
        /// Gets or sets the actual node Children
        /// </summary>
        public System.Collections.Generic.List<ISpaceASTNode> Children { get; set; }
    }
}
