//-----------------------------------------------------------------------
// <copyright file="GoASTNode.cs" company="Integra.Space.Language">
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
    internal class GoASTNode : AstNodeBase
    {
        /// <summary>
        /// Reserved world 'use'.
        /// </summary>
        private int counter;

        /// <summary>
        /// Reserved word 'go'.
        /// </summary>
        private string go;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.go = (string)ChildrenNodes[0].Token.Value;
            this.counter = 1;

            if (ChildrenNodes.Count > 1)
            {
                this.counter = (int)ChildrenNodes[1].Token.Value;
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
            this.BeginEvaluate(thread);
            this.EndEvaluate(thread);

            return new GoCommandNode(this.counter, this.Location.Line, this.Location.Column, string.Format("{0} {1}", this.go, this.counter));            
        }
    }
}
