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
    public class QueryResult<T> : IQueryResultSerializable where T : EventResult
    {
        /// <summary>
        /// Query identifier
        /// </summary>
        private string queryId;

        /// <summary>
        /// Date and time the events were processed.
        /// </summary>
        private DateTime queryDateTime;

        /// <summary>
        /// Query result, contains a set of <see cref="EventResult"/> objects
        /// </summary>
        private T[] result;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
        /// </summary>
        /// <param name="queryId">Query identifier</param>
        /// <param name="queryDateTime">Date and time the event were processed.</param>
        /// <param name="result">Array of event results.</param>
        public QueryResult(string queryId, DateTime queryDateTime, T[] result)
        {
            this.queryId = queryId;
            this.queryDateTime = queryDateTime;
            this.result = result;
        }

        /// <summary>
        /// Gets the format version of the query result.
        /// </summary>
        public byte FormatVersion
        {
            get
            {
                return 1;
            }
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
        /// Gets the date and time the event were processed.
        /// </summary>
        public DateTime QueryDatetime
        {
            get
            {
                return this.queryDateTime;
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

        /// <summary>
        /// Serialize the query identifier/name.
        /// </summary>
        /// <param name="writer">Query Result Writer</param>
        public virtual void Serialize(IQueryResultWriter writer)
        {
            writer.WriteStartQueryResult();
            writer.WriteValue(this.FormatVersion);
            writer.WriteValue(this.queryId);
            writer.WriteValue(this.queryDateTime);
            foreach (T r in this.result)
            {
                writer.WriteStartQueryResultRow();
                r.Serialize(writer);
                writer.WriteEndQueryResultRow();
            }

            writer.WriteEndQueryResult();
        }
    }
}
