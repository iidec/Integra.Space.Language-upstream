//-----------------------------------------------------------------------
// <copyright file="WarningResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    /// <summary>
    /// Success result class.
    /// </summary>
    public class WarningResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WarningResult"/> class.
        /// </summary>
        /// <param name="code">Result code.</param>
        /// <param name="message">Result message.</param>
        public WarningResult(int code, string message) : base(code, message, ResultType.Warning)
        {
        }
    }
}
