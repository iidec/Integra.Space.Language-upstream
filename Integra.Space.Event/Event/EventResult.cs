//-----------------------------------------------------------------------
// <copyright file="EventResult.cs" company="Ingetra.Space.Event">
//     Copyright (c) Ingetra.Space.Event. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Event
{
    using System;

    /// <summary>
    /// Event result class
    /// </summary>
    public class EventResult
    {
        /// <summary>
        /// Date and time the event was processed.
        /// </summary>
        private DateTime eventDateTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventResult"/> class.
        /// </summary>
        /// <param name="eventDateTime">Date and time the event was processed.</param>
        public EventResult(DateTime eventDateTime)
        {
            this.eventDateTime = eventDateTime;
        }   
        
        /// <summary>
        /// Gets the date and time the event was processed.
        /// </summary>
        public DateTime EventDatetime
        {
            get
            {
                return this.eventDateTime;
            }
        }  
    }
}
