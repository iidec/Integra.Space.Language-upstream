//-----------------------------------------------------------------------
// <copyright file="UseCommandNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Integra.Space.Common;

    /// <summary>
    /// Command object class.
    /// </summary>
    internal sealed class UseCommandNode : DDLCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UseCommandNode"/> class.
        /// </summary>
        /// <param name="name">Name of the object.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public UseCommandNode(string name, int line, int column, string nodeText) : base(ActionCommandEnum.Use, new CommandObject(SystemObjectEnum.Database, name, PermissionsEnum.Connect, false), line, column, nodeText, null, null)
        {
        }
    }
}
