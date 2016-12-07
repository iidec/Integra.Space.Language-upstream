//-----------------------------------------------------------------------
// <copyright file="InputBase.cs" company="Integra.Space.Runtime">
//     Copyright (c) Integra.Space.Runtime. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    /// <summary>
    /// Event base.
    /// </summary>
    public abstract class InputBase
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
        /// Creation date and time.
        /// </summary>
        private System.DateTime sourceTimestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputBase"/> class.
        /// </summary>
        public InputBase()
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
            set
            {
                this.systemTimestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the creation date and time of the event.
        /// </summary>
        public System.DateTime SourceTimestamp
        {
            get
            {
                return this.sourceTimestamp;
            }

            set
            {
                this.sourceTimestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the date and time before enumerable join processing.
        /// </summary>
        public System.DateTime JoinProcessingTimestamp { get; set; }

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
