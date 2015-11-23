//-----------------------------------------------------------------------
// <copyright file="ParseException.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Exceptions
{
    using System;

    /// <summary>
    /// Parse exception.
    /// </summary>
    internal sealed class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        public ParseException(string message) : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
