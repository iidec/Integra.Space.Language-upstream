//-----------------------------------------------------------------------
// <copyright file="SecureObjectsTypeNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Permissions
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// SecureObjects class
    /// </summary>
    internal sealed class SecureObjectsTypeNode : AstNodeBase
    {
        /// <summary>
        /// secure object: role, stream, server role
        /// </summary>
        private string secureObject;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 1)
            {
                this.secureObject = (string)ChildrenNodes[0].Token.Value;
            }
            else if (childrenCount == 2)
            {
                this.secureObject = (string)ChildrenNodes[0].Token.Value + " " + (string)ChildrenNodes[1].Token.Value;
            }
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            return this.secureObject;
        }
    }
}
