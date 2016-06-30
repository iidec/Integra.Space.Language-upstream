//-----------------------------------------------------------------------
// <copyright file="SpacePermissionASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space object AST node class.
    /// </summary>
    internal class SpacePermissionASTNode : AstNodeBase
    {
        /// <summary>
        /// Space permission.
        /// </summary>
        private string spacePermission;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.spacePermission = (string)ChildrenNodes[0].Token.Value;
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            this.BeginEvaluate(thread);            
            this.EndEvaluate(thread);

            SpacePermissionsEnum permission;
            if (System.Enum.TryParse(this.spacePermission, true, out permission))
            {
                return permission;
            }
            else
            {
                throw new Exceptions.SyntaxException(string.Format("Invalid permission {0}.", this.spacePermission));
            }
        }
    }
}
