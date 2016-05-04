//-----------------------------------------------------------------------
// <copyright file="NonConstantValueNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects
{
    using Integra.Space.Language.ASTNodes.Values;

    /// <summary>
    /// ConstantValueNode class
    /// </summary>
    internal sealed class NonConstantValueNode : ValueNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonConstantValueNode"/> class.
        /// </summary>
        public NonConstantValueNode() : base(false)
        {
        }
    }
}
