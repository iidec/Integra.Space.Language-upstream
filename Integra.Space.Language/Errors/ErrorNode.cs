//-----------------------------------------------------------------------
// <copyright file="ErrorNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Errors
{
    using Integra.Space.Language.Exceptions;

    /// <summary>
    /// ErrorNode class
    /// </summary>
    internal sealed class ErrorNode
    {
        /// <summary>
        /// Gets or sets the line of the error
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the column of the error
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the title of the error
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the message of the error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the text of the error
        /// </summary>
        public string NodeText { get; set; }
    }
}
