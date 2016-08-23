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
        private List<UserOption> userOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAndAlterUserNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="userOptions">User command options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public CreateAndAlterUserNode(ActionCommandEnum action, string identifier, List<UserOption> userOptions, int line, int column, string nodeText) : base(action, SystemObjectEnum.DatabaseUser, identifier, line, column, nodeText)
        {
            this.userOptions = userOptions;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAndAlterUserNode"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="userOptions">User command options.</param>
        /// <param name="schema">Schema of the object.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public CreateAndAlterUserNode(ActionCommandEnum action, string identifier, List<UserOption> userOptions, string schema, int line, int column, string nodeText) : base(action, SystemObjectEnum.DatabaseUser, identifier, line, column, nodeText)
        {
            this.userOptions = userOptions;
        }

        /// <summary>
        /// Gets the user options defined in the command.
        /// </summary>
        public List<UserOption> UserOptions
        {
            get
            {
                return this.userOptions;
            }
        }

        /// <inheritdoc />
        public override PermissionsEnum PermissionValue
        {
            get
            {
                if (this.Action == ActionCommandEnum.Create)
                {
                    return PermissionsEnum.Create;
                }
                else if (this.Action == ActionCommandEnum.Alter)
                {
                    return PermissionsEnum.Alter;
                }
                else
                {
                    throw new System.Exception("Invalid action for the command.");
                }
            }
        }
    }
}
