//-----------------------------------------------------------------------
// <copyright file="SuccessResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    /// <summary>
    /// Success result class.
    /// </summary>
    public class SuccessResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessResult"/> class.
        /// </summary>
        /// <param name="code">Result code.</param>
        /// <param name="message">Result message.</param>
        public SuccessResult(int code, string message) : base(code, message, ResultType.Success)
        {
        }
    }
}
