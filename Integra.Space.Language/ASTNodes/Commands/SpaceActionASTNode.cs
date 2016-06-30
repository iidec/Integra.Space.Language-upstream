//-----------------------------------------------------------------------
// <copyright file="SpaceActionASTNode.cs" company="Integra.Space.Language">
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
    /// Space action AST node class.
    /// </summary>
    internal class SpaceActionASTNode : AstNodeBase
    {
        /// <summary>
        /// Space object.
        /// </summary>
        private string spaceAction;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.spaceAction = (string)ChildrenNodes[0].Token.Value;
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

            SpaceActionCommandEnum action;
            if (System.Enum.TryParse(this.spaceAction, true, out action))
            {
                return action;
            }
            else
            {
                throw new Exceptions.SyntaxException(string.Format("Invalid action {0}.", this.spaceAction));
            }
        }
    }
}
