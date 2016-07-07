//-----------------------------------------------------------------------
// <copyright file="CreateAndAlterUserNode.cs" company="Integra.Space.Common">
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
    internal sealed class CreateAndAlterUserNode : SpaceCrudCommandNode
    {
        /// <summary>
        /// Password of the user.
        /// </summary>
        private List<SpaceUserOption> userOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAndAlterUserNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="userOptions">User command options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public CreateAndAlterUserNode(SpaceActionCommandEnum action, string identifier, List<SpaceUserOption> userOptions, int line, int column, string nodeText) : base(action, SpaceObjectEnum.User, identifier, line, column, nodeText)
        {
            this.userOptions = userOptions;
        }

        /// <summary>
        /// Gets the user options defined in the command.
        /// </summary>
        public List<SpaceUserOption> UserOptions
        {
            get
            {
                return this.userOptions;
            }
        }
    }
}
