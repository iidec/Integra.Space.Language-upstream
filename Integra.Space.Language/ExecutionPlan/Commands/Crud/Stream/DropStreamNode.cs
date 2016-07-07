//-----------------------------------------------------------------------
// <copyright file="DropStreamNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class DropStreamNode : DropObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropStreamNode"/> class.
        /// </summary>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public DropStreamNode(string identifier, int line, int column, string nodeText) : base(Common.SpaceObjectEnum.Stream, identifier, line, column, nodeText)
        {
        }
    }
}
