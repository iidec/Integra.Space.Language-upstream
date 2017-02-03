//-----------------------------------------------------------------------
// <copyright file="EventResult.cs" company="Ingetra.Space.Event">
//     Copyright (c) Ingetra.Space.Event. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    using System;

    /// <summary>
    /// Event result class
    /// </summary>
    public class EventResult : EventBase, IEventResultSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventResult"/> class.
        /// </summary>
        public EventResult()
        {
        }

        /// <inheritdoc />
        public virtual void Serialize(IQueryResultWriter writer)
        {
        }
    }
}
