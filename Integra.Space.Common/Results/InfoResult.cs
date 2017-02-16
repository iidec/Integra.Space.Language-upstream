//-----------------------------------------------------------------------
// <copyright file="InfoResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    /// <summary>
    /// Infomative result class.
    /// </summary>
    public class InfoResult : ResultBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoResult"/> class.
        /// </summary>
        /// <param name="code">Result code.</param>
        /// <param name="message">Result message.</param>
        public InfoResult(int code, string message) : base(code, message, ResultType.Info)
        {
        }
    }
}
