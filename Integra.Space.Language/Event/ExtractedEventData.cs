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
        /// Indicates whether the event matched with another event.
        /// </summary>
        private bool matched;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractedEventData"/> class.
        /// </summary>
        public ExtractedEventData()
        {
            this.matched = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event matched with another event.
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
    }
}
