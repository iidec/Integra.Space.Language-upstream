//-----------------------------------------------------------------------
// <copyright file="EventObject.cs" company="Ingetra.Vision.EventObject">
//     Copyright (c) Ingetra.Vision.EventObject. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Messaging;

    /// <summary>
    /// Event object class
    /// </summary>
    [Serializable]
    public class EventObject
    {
        /// <summary>
        /// Agent 
        /// Doc go here
        /// </summary>
        private EventAgent agent;

        /// <summary>
        /// adapter
        /// Doc go here
        /// </summary>
        private EventAdapter adapter;

        /// <summary>
        /// message
        /// Doc go here
        /// </summary>
        private Message message;

        /// <summary>
        /// List of queries
        /// </summary>
        private IEnumerable<string> queries;

        /// <summary>
        /// Query reference count
        /// </summary>
        private int refcount = 0;

        /// <summary>
        /// Indicates whether the event matched with another event
        /// </summary>
        private bool matched;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventObject"/> class.
        /// </summary>
        /// <param name="queries">Queries assigned to the source of events.</param>
        public EventObject(IEnumerable<string> queries)
        {
            this.queries = queries;
            this.refcount = queries.Count();
            this.matched = false;
        }

        /// <summary>
        /// Gets or sets the agent
        /// </summary>
        public EventAgent Agent
        {
            get
            {
                if (this.agent == null)
                {
                    this.agent = new EventAgent();
                }

                return this.agent;
            }

            set
            {
                this.agent = value;
            }
        }

        /// <summary>
        /// Gets or sets the adapter
        /// </summary>
        public EventAdapter Adapter
        {
            get
            {
                if (this.adapter == null)
                {
                    this.adapter = new EventAdapter();
                }

                return this.adapter;
            }

            set
            {
                this.adapter = value;
            }
        }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public Message Message
        {
            get
            {
                return this.message;
            }

            set
            {
                this.message = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event matched with another event
        /// </summary>
        public bool Matched
        {
            get
            {
                return this.matched;
            }

            set
            {
                this.matched = value;
            }
        }
        
        /// <summary>
        /// Increment the reference counter
        /// </summary>
        /// <param name="queryName">Query name</param>
        /// <returns>Indicate if the query name exists en the hash set.</returns>
        public bool Lock(string queryName)
        {
            try
            {
                return this.queries.Contains(queryName);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Decrement the reference counter and if is equal to 0 
        /// </summary>
        /// <param name="queryName">Query name</param>
        public void Unlock(string queryName)
        {
            if (this.queries.Contains(queryName))
            {
                if (System.Threading.Interlocked.Decrement(ref this.refcount) == 0)
                {
                    this.message.Dispose();
                    this.queries = null;
                }
            }
        }
    }
}
