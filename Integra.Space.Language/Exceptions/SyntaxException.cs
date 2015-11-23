//-----------------------------------------------------------------------
// <copyright file="SyntaxException.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Exceptions
{
    using System;

    /// <summary>
    /// Syntax exception.
    /// </summary>
    internal sealed class SyntaxException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        public SyntaxException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public SyntaxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxException"/> class
        /// </summary>
        /// <param name="innerException">Inner exception</param>
        public SyntaxException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}
