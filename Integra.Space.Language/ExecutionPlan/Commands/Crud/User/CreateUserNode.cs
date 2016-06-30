//-----------------------------------------------------------------------
// <copyright file="CreateUserNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Action over object node class.
    /// </summary>
    internal sealed class CreateUserNode : CreateObjectNode
    {
        /// <summary>
        /// Password of the user.
        /// </summary>
        private string password;

        /// <summary>
        /// Status of the user.
        /// </summary>
        private bool enable;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserNode"/> class.
        /// </summary>
        /// <param name="identifier">Space object identifier.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="password">Password of the user.</param>
        /// <param name="enable">Status of the user.</param>
        public CreateUserNode(string identifier, int line, int column, string nodeText, string password, bool enable) : base(identifier, line, column, nodeText)
        {
            this.password = password;
            this.enable = enable;
        }
    }
}
