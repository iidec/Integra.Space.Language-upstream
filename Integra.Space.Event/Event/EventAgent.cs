//-----------------------------------------------------------------------
// <copyright file="EventAgent.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Event
{
    using System;

    /// <summary>
    /// Agent class
    /// </summary>
    [Serializable]
    public class EventAgent
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
        /// machineName
        /// Doc go here
        /// </summary>
        private string machineName;

        /// <summary>
        /// Gets the name of the agent
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

        /// <summary>
        /// Gets
        /// Doc go here
        /// </summary>
        public string MachineName
        {
            get
            {
                if (this.machineName == null)
                {
                    this.machineName = "Anonimo";
                }

                return this.machineName;
            }
        }
    }
}
