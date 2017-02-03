//-----------------------------------------------------------------------
// <copyright file="TruncateNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Diagnostics.Contracts;
    using Common;

    /// <summary>
    /// Command object class.
    /// </summary>
    internal sealed class TruncateNode : DMLCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TruncateNode"/> class.
        /// </summary>
        /// <param name="action">Command action.</param>
        /// <param name="commandObject">Command object.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public TruncateNode(Common.ActionCommandEnum action, CommandObject commandObject, int line, int column, string nodeText) : base(action, commandObject, line, column, nodeText)
        {
        }
    }
}
