//-----------------------------------------------------------------------
// <copyright file="StatusCommandNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Common;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal class StatusCommandNode : CompiledCommand
    {
        /// <summary>
        /// Space object identifier.
        /// </summary>
        private string identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCommandNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjectType">Space object type.</param>
        /// <param name="name">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public StatusCommandNode(SpaceActionCommandEnum action, SpaceObjectEnum spaceObjectType, string name, int line, int column, string nodeText) : base(action, spaceObjectType, name, line, column, nodeText)
        {
            this.identifier = name;
        }
    }
}
