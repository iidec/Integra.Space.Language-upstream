//-----------------------------------------------------------------------
// <copyright file="SourceColumnType.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Helpers
{
    using System;

    /// <summary>
    /// Source column type class.
    /// </summary>
    internal class SourceColumnType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceColumnType"/> class.
        /// </summary>
        /// <param name="columnType">Column type.</param>
        /// <param name="length">Column length.</param>
        public SourceColumnType(Type columnType, int length)
        {
            this.ColumnType = columnType;
            this.Length = length;
        }

        /// <summary>
        /// Gets the column type.
        /// </summary>
        public Type ColumnType { get; private set; }

        /// <summary>
        /// Gets the length of the type.
        /// </summary>
        public int Length { get; private set; }
    }
}
