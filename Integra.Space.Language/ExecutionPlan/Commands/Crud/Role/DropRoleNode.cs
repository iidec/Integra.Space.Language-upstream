//-----------------------------------------------------------------------
// <copyright file="DropRoleNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class DropRoleNode : DropObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropRoleNode"/> class.
        /// </summary>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public DropRoleNode(string identifier, int line, int column, string nodeText) : base(Common.SpaceObjectEnum.Role, identifier, line, column, nodeText)
        {
        }
    }
}
