//-----------------------------------------------------------------------
// <copyright file="EventAdapter.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Event
{
    using System;

    /// <summary>
    /// Adapter class
    /// </summary>
    [Serializable]
    public class EventAdapter
    {
        /// <summary>
        /// timestamp
        /// Doc go here
        /// </summary>
        private DateTime timestamp = DateTime.Now;

        /// <summary>
        /// name
        /// Doc go here
        /// </summary>
        private string name;

        /// <summary>
        /// Gets the name of the adapter
        /// </summary>
        public string Name
        {
            get
            {
                if (this.name == null)
                {
                    this.name = "Anonimo";
                }

                return this.name;
            }
        }

        /// <summary>
        /// Gets
        /// Doc go here
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }
    }
}
