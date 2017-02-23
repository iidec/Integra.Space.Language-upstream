//-----------------------------------------------------------------------
// <copyright file="ParseContextBase.cs" company="Integra.Space.Language">
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
    /// <typeparam name="TPayload">Data type of the objects returned from the parse process.</typeparam>
    internal class ParseContextBase<TPayload>
    {
        /// <summary>
        /// Batch array.
        /// </summary>
        private TPayload payload;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseContextBase{TPayload}"/> class.
        /// </summary>
        public ParseContextBase()
        {
            this.Results = new List<ResultBase>();
        }

        /// <summary>
        /// Gets or sets the batch objects resultants of the incoming string parse execution.
        /// </summary>
        public TPayload Payload
        {
            get
            {
                return this.payload;
            }

            set
            {
                if (this.payload == null)
                {
                    this.payload = value;
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
