//-----------------------------------------------------------------------
// <copyright file="ExtractedEventData.cs" company="Ingetra.Vision.EventObject">
//     Copyright (c) Ingetra.Vision.EventObject. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Integra.Space
{
    /// <summary>
    /// Extracted event data class.
    /// </summary>
    public class ExtractedEventData
    {
        /// <summary>
        /// Object to lock the change of the event state.
        /// </summary>
        private static object lockState = new object();

        /// <summary>
        /// Reception date and time.
        /// </summary>
        private System.DateTime systemTimestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractedEventData"/> class.
        /// </summary>
        public ExtractedEventData()
        {
            this.systemTimestamp = System.DateTime.Now;
            this.State = ExtractedEventDataStateEnum.Created;
        }

        /// <summary>
        /// Gets the actual state of the event
        /// </summary>
        public ExtractedEventDataStateEnum State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the reception date and time of the event.
        /// </summary>
        public System.DateTime SystemTimestamp
        {
            get
            {
                return this.systemTimestamp;
            }
        }

        /// <summary>
        /// Set the state of the event.
        /// </summary>
        /// <param name="state">State to set.</param>
        /// <returns>Return a value indicating whether the event state changed.</returns>
        public bool SetState(ExtractedEventDataStateEnum state)
        {
            lock (lockState)
            {
                if (state == ExtractedEventDataStateEnum.Matched && (this.State == ExtractedEventDataStateEnum.Created || this.State == ExtractedEventDataStateEnum.Matched))
                {
                    this.State = state;
                    return true;
                }

                if (state == ExtractedEventDataStateEnum.Expired && this.State == ExtractedEventDataStateEnum.Created)
                {
                    this.State = state;
                    return true;
                }
            }

            return false;
        }
    }
}