﻿//-----------------------------------------------------------------------
// <copyright file="AlterUserNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class AlterUserNode : AlterObjectNode
    {
        /// <summary>
        /// Password of the user.
        /// </summary>
        private List<SpaceUserOption> userOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterUserNode"/> class.
        /// </summary>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="userOptions">User command options.</param>
        public AlterUserNode(string identifier, int line, int column, string nodeText, List<SpaceUserOption> userOptions) : base(Common.SpaceObjectEnum.User, identifier, line, column, nodeText)
        {
            this.userOptions = userOptions;
        }
    }
}
