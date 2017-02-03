//-----------------------------------------------------------------------
// <copyright file="SpaceQuery.cs" company="Integra.Space.Runtime">
//     Copyright (c) Integra.Space.Runtime. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    /// <summary>
    /// Space query class.
    /// </summary>
    public abstract class SpaceQuery
    {
        /// <summary>
        /// Query identifier.
        /// </summary>
        private string queryId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceQuery"/> class.
        /// </summary>
        /// <param name="queryId">Query identifier.</param>
        public SpaceQuery(string queryId)
        {
            this.queryId = queryId;
        }

        /// <summary>
        /// Gets the query identifier.
        /// </summary>
        public string QueryId
        {
            get
            {
                return this.queryId;
            }
        }
    }
}
