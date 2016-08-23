//-----------------------------------------------------------------------
// <copyright file="AlterObjectNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal abstract class AlterObjectNode : SpaceCrudCommandNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlterObjectNode"/> class.
        /// </summary>
        /// <param name="spaceObjectType">Space object type.</param>
        /// <param name="name">Space object name.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterObjectNode(SystemObjectEnum spaceObjectType, string name, int line, int column, string nodeText) : base(ActionCommandEnum.Alter, spaceObjectType, name, line, column, nodeText)
        {
        }

        /// <inheritdoc />
        public override PermissionsEnum PermissionValue
        {
            get
            {
                if (this.Action == ActionCommandEnum.Alter)
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
