﻿//-----------------------------------------------------------------------
// <copyright file="DropObjectNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal class DropObjectNode : SpaceCrudCommandNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropObjectNode"/> class.
        /// </summary>
        /// <param name="spaceObjectType">Space object type.</param>
        /// <param name="name">Space object name.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public DropObjectNode(SpaceObjectEnum spaceObjectType, string name, int line, int column, string nodeText) : base(SpaceActionCommandEnum.Drop, spaceObjectType, name, line, column, nodeText)
        {
        }
    }
}
