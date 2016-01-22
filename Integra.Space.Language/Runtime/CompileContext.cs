//-----------------------------------------------------------------------
// <copyright file="CompileContext.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using Integra.Space.Language.Scheduler;

    /// <summary>
    /// Space compile context
    /// </summary>
    internal class CompileContext
    {
        /// <summary>
        /// Query name
        /// </summary>
        private string queryName;
        
        /// <summary>
        /// Gets or sets a value indicating whether the print log is activated
        /// </summary>
        public bool PrintLog { get; set; }

        /// <summary>
        /// Gets or sets the query name
        /// </summary>
        public string QueryName
        {
            get
            {
                if (this.queryName == null)
                {
                    return string.Empty;
                }

                return this.queryName;
            }

            set
            {
                this.queryName = value;
            }
        }

        /// <summary>
        /// Gets or sets the space scheduler
        /// </summary>
        public IQuerySchedulerFactory Scheduler { get; set; }        
    }
}
