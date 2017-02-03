//-----------------------------------------------------------------------
// <copyright file="ProjectionColumn.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;

    /// <summary>
    /// Projection column class.
    /// </summary>
    internal class ProjectionColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectionColumn"/> class.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="alias">Column alias.</param>
        /// <param name="sourceAlias">Source alias.</param>
        /// <param name="sourceName">Source name.</param>
        /// <param name="columnType">Type of the column.</param>
        public ProjectionColumn(string propertyName, string alias, string sourceAlias, string sourceName, Type columnType)
        {
            this.PropertyName = propertyName;
            this.Alias = alias;
            this.SourceAlias = sourceAlias;
            this.ColumnType = columnType;
            this.SourceName = sourceName;
        }

        /// <summary>
        /// Gets the alias of the column.
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets the property name of the column.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the source alias of the property name.
        /// </summary>
        public string SourceAlias { get; private set; }

        /// <summary>
        /// Gets the type of the column.
        /// </summary>
        public Type ColumnType { get; private set; }

        /// <summary>
        /// Gets the source name of the property name.
        /// </summary>
        public string SourceName { get; private set; }
    }
}
