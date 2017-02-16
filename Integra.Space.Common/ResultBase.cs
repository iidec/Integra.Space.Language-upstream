//-----------------------------------------------------------------------
// <copyright file="ResultBase.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    /// <summary>
    /// The base class for all results in space.
    /// </summary>
    public abstract class ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBase"/> class.
        /// </summary>
        /// <param name="code">Result code.</param>
        /// <param name="message">Result message.</param>
        /// <param name="type">Result type.</param>
        public ResultBase(int code, string message, ResultType type)
        {
            this.Code = code;
            this.Message = message;
            this.Type = type;
        }

        /// <summary>
        /// Gets the result code.
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// Gets the result message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the result type.
        /// </summary>
        public ResultType Type { get; private set; }
    }
}
