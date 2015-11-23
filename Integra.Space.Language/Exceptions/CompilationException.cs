//-----------------------------------------------------------------------
// <copyright file="CompilationException.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Exceptions
{
    using System;

    /// <summary>
    /// Compilation exception.
    /// </summary>
    internal sealed class CompilationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        public CompilationException(string message) : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public CompilationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> class
        /// </summary>
        /// <param name="innerException">Inner exception</param>
        public CompilationException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}
