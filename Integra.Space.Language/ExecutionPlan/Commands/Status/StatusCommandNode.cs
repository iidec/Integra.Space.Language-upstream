//-----------------------------------------------------------------------
// <copyright file="StatusCommandNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Command action node class.
    /// </summary>
    internal abstract class StatusCommandNode : SpaceCommand
    {
        /// <summary>
        /// Space object identifier.
        /// </summary>
        private string identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCommandNode"/> class.
        /// </summary>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public StatusCommandNode(string identifier, int line, int column, string nodeText) : base(line, column, nodeText)
        {
            this.identifier = identifier;
        }
    }
}
