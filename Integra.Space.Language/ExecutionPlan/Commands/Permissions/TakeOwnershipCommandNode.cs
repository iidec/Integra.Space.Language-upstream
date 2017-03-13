//-----------------------------------------------------------------------
// <copyright file="TakeOwnershipCommandNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Integra.Space.Common;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal class TakeOwnershipCommandNode : DDLCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TakeOwnershipCommandNode"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public TakeOwnershipCommandNode(CommandObject commandObject, int line, int column, string nodeText) : base(ActionCommandEnum.TakeOwnership, commandObject, line, column, nodeText)
        {
        }
    }
}
