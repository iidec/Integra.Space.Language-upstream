//-----------------------------------------------------------------------
// <copyright file="TimeStatementASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// TimeoutStatementNode class
    /// </summary>
    internal class TimeStatementASTNode : AstNodeBase
    {
        /// <summary>
        /// timeout reserved word
        /// </summary>
        private string timeoutWord;

        /// <summary>
        /// timespan value of the timeout
        /// </summary>
        private AstNodeBase timespan;

        /// <summary>
        /// result plan
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeStatementASTNode"/> class.
        /// </summary>
        /// <param name="nodeType">Node type</param>
        public TimeStatementASTNode(PlanNodeTypeEnum nodeType)
        {
            this.result = new PlanNode(this.Location.Line, this.Location.Column, nodeType, this.NodeText);
        }

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
                this.timeoutWord = (string)ChildrenNodes[0].Token.Value;
                this.timespan = AddChild(NodeUseType.Parameter, "timespan", ChildrenNodes[1]) as AstNodeBase;
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
            if (ChildrenNodes.Count != 0)
            {
                this.BeginEvaluate(thread);
                PlanNode timespanAux = (PlanNode)this.timespan.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText = string.Format("{0} {1}", this.timeoutWord, timespanAux.NodeText);

                this.result.Children = new List<PlanNode>();
                this.result.Children.Add(timespanAux);

                return this.result;
            }

            return null;
        }
    }
}
