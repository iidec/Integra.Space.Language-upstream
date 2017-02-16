//-----------------------------------------------------------------------
// <copyright file="ErrorResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    /// <summary>
    /// Error result class.
    /// </summary>
    public class ErrorResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResult"/> class.
        /// </summary>
        /// <param name="code">Result code.</param>
        /// <param name="message">Result message.</param>
        public ErrorResult(int code, string message) : base(code, message, ResultType.Warning)
        {
        }
    }
}
