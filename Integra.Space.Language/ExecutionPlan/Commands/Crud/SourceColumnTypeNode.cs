//-----------------------------------------------------------------------
// <copyright file="SourceColumnTypeNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;

    /// <summary>
    /// Source column type node class.
    /// </summary>
    internal class SourceColumnTypeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceColumnTypeNode"/> class.
        /// </summary>
        /// <param name="columnType">Column type.</param>
        /// <param name="length">Column type length</param>
        /// <param name="precision">Column type precision.</param>
        public SourceColumnTypeNode(Type columnType, uint? length, uint? precision)
        {
            this.ColumnType = columnType;
            this.Length = length;
            this.Precision = precision;
        }

        /// <summary>
        /// Gets the column type.
        /// </summary>
        public Type ColumnType { get; private set; }

        /// <summary>
        /// Gets the column type length.
        /// </summary>             
        public uint? Length { get; private set; }

        /// <summary>
        /// Gets the column precision.
        /// </summary>
        public uint? Precision { get; private set; }
    }
}
