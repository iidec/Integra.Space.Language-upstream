//-----------------------------------------------------------------------
// <copyright file="SourceColumnNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Source column node class.
    /// </summary>
    internal class SourceColumnNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceColumnNode"/> class.
        /// </summary>
        /// <param name="name">Column name.</param>
        /// <param name="type">Column type.</param>
        public SourceColumnNode(string name, SourceColumnTypeNode type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        /// Gets the column type.
        /// </summary>
        public SourceColumnTypeNode Type { get; private set; }

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string Name { get; private set; }
    }
}
