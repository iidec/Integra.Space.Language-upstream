﻿//-----------------------------------------------------------------------
// <copyright file="StopSourceNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Command action node class.
    /// </summary>
    internal sealed class StopSourceNode : StopNode 
    {
        /// <summary>
        /// Space object identifier.
        /// </summary>
        private string identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="StopSourceNode"/> class.
        /// </summary>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public StopSourceNode(string identifier, int line, int column, string nodeText) : base(Common.SpaceObjectEnum.Source, identifier, line, column, nodeText)
        {
            this.identifier = identifier;
        }
    }
}
