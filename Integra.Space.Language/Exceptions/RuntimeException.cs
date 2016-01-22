//-----------------------------------------------------------------------
// <copyright file="RuntimeException.cs" company="Ingetra.Space.Language">
//     Copyright (c) Ingetra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Exceptions
{
    using System;

    /// <summary>
    /// Compilation exception.
    /// </summary>
    internal class RuntimeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        public RuntimeException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public RuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeException"/> class
        /// </summary>
        /// <param name="innerException">Inner exception</param>
        public RuntimeException(Exception innerException) : base(string.Empty, innerException)
        {
        }
    }
}
