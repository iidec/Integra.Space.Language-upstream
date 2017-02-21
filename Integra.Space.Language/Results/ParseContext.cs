//-----------------------------------------------------------------------
// <copyright file="ParseContext.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;

    /// <summary>
    /// Parse context class.
    /// </summary>
    internal class ParseContext
    {
        /// <summary>
        /// Batch array.
        /// </summary>
        private BatchNode[] batches;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseContext"/> class.
        /// </summary>
        public ParseContext()
        {
            this.Results = new List<ResultBase>();
        }

        /// <summary>
        /// Gets or sets the batch objects resultants of the incoming string parse execution.
        /// </summary>
        public BatchNode[] Batches
        {
            get
            {
                return this.batches;
            }

            set
            {
                if (this.batches == null)
                {
                    this.batches = value;
                }
            }
        }

        /// <summary>
        /// Gets the parse execution results.
        /// </summary>
        public List<ResultBase> Results { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the parse process finished with errors.
        /// </summary>
        /// <returns>A value indicating whether the parse process finished with errors</returns>
        public bool HasErrors()
        {
            return this.Results.Any(x => x.Type == ResultType.Error);
        }
    }
}
