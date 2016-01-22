//-----------------------------------------------------------------------
// <copyright file="QueryResult.cs" company="Ingetra.Space.Event">
//     Copyright (c) Ingetra.Space.Event. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    using System;

    /// <summary>
    /// Query result class
    /// </summary>
    /// <typeparam name="T">Event result type</typeparam>
    public class QueryResult<T> where T : EventResult
    {
        /// <summary>
        /// Query identifier
        /// </summary>
        private string queryId;

        /// <summary>
        /// Query result, contains a set of <see cref="EventResult"/> objects
        /// </summary>
        private T[] result;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
        /// </summary>
        /// <param name="queryId">Query identifier</param>
        /// <param name="result">Array of event results.</param>
        public QueryResult(string queryId, T[] result)
        {
            this.queryId = queryId;
            this.result = result;
        }

        /// <summary>
        /// Gets the query identifier
        /// </summary>
        public string QueryId
        {
            get
            {
                return this.queryId;
            }
        }

        /// <summary>
        /// Gets the query result
        /// </summary>
        public T[] Result
        {
            get
            {
                return this.result;
            }
        }
    }
}
