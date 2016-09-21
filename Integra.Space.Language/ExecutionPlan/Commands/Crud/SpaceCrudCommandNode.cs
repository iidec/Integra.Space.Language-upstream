﻿//-----------------------------------------------------------------------
// <copyright file="SpaceCrudCommandNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Common;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal abstract class SpaceCrudCommandNode : CompiledCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceCrudCommandNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="commandObject">Command object.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public SpaceCrudCommandNode(ActionCommandEnum action, CommandObject commandObject, int line, int column, string nodeText, string schemaName, string databaseName) : base(action, commandObject, line, column, nodeText, schemaName, databaseName)
        {
        }
    }
}
