//-----------------------------------------------------------------------
// <copyright file="DropUserNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class DropUserNode : DropObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropUserNode"/> class.
        /// </summary>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public DropUserNode(string identifier, int line, int column, string nodeText) : base(Common.SpaceObjectEnum.User, identifier, line, column, nodeText)
        {
        }
    }
}
