//-----------------------------------------------------------------------
// <copyright file="ParseSuccessResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Parse result class.
    /// </summary>
    internal class ParseSuccessResult : Common.SuccessResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseSuccessResult"/> class.
        /// </summary>
        /// <param name="code">Result code.</param>
        /// <param name="message">Result message.</param>
        /// <param name="line">Line where the result was generated.</param>
        /// <param name="column">Column where the result was generated.</param>
        public ParseSuccessResult(int code, string message, int line, int column) : base(code, message)
        {
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Gets the column where the result was generated.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the line where the result was generated.
        /// </summary>
        public int Line { get; private set; }
    }
}
