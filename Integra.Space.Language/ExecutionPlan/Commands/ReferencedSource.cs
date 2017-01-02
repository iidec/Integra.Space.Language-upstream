//-----------------------------------------------------------------------
// <copyright file="ReferencedSource.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Stream referenced sources class.
    /// </summary>
    internal class ReferencedSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferencedSource"/> class.
        /// </summary>
        /// <param name="databaseName">Database to which the source belongs.</param>
        /// <param name="schemaName">Schema to which the source belongs.</param>
        /// <param name="sourceName">Source referenced at the stream query.</param>
        /// <param name="alias">Source alias.</param>
        public ReferencedSource(string databaseName, string schemaName, string sourceName, string alias)
        {
            this.DatabaseName = databaseName;
            this.SchemaName = schemaName;
            this.SourceName = sourceName;
            this.Alias = alias;
        }

        /// <summary>
        /// Gets the source name.
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// Gets the schema name.
        /// </summary>
        public string SchemaName { get; private set; }

        /// <summary>
        /// Gets the database name.
        /// </summary>
        public string DatabaseName { get; private set; }

        /// <summary>
        /// Gets the source alias.
        /// </summary>
        public string Alias { get; private set; }
    }
}
