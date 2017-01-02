//-----------------------------------------------------------------------
// <copyright file="PassASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;

    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// SourceDefinitionNode class
    /// </summary>
    internal sealed class PassASTNode : AstNodeBase
    {
        /// <summary>
        /// Non terminal node
        /// </summary>
        private AstNodeBase nonTerminal;

        /// <summary>
        /// Terminal node
        /// </summary>
        private string terminal;

        /// <summary>
        /// Resultant plan node.
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            if (ChildrenNodes.Count != 0)
            {
                this.nonTerminal = AddChild(NodeUseType.Parameter, "CopiedNode", ChildrenNodes[0]) as AstNodeBase;

                if (this.nonTerminal == null)
                {
                    this.terminal = (string)ChildrenNodes[0].Token.Value;
                }
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
            if (ChildrenNodes.Count == 0)
            {
                return null;
            }

            if (this.nonTerminal != null)
            {
                this.BeginEvaluate(thread);
                this.result = (PlanNode)this.nonTerminal.Evaluate(thread);
                this.EndEvaluate(thread);
            }
            else
            {
                this.result = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
                this.result.Column = ChildrenNodes[0].Token.Location.Column;
                this.result.Line = ChildrenNodes[0].Token.Location.Line;
                this.result.NodeText = this.terminal;
            }

            return this.result;
        }
    }
}
