//-----------------------------------------------------------------------
// <copyright file="FromNode.cs" company="Integra.Space.Language">
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
    /// FromNode class
    /// </summary>
    internal sealed class FromNode : AstNodeBase
    {
        /// <summary>
        /// identifier node of the from node
        /// </summary>
        private AstNodeBase idFromNode;

        /// <summary>
        /// reserved word 'from'
        /// </summary>
        private string from;

        /// <summary>
        /// result plan
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
            this.from = (string)ChildrenNodes[0].Token.Value;
            this.idFromNode = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[1]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.ObservableFrom;
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
            PlanNode idFrom = (PlanNode)this.idFromNode.Evaluate(thread);
            this.EndEvaluate(thread);

            idFrom.Properties["DataType"] = typeof(string).ToString();
            this.result.NodeText = this.from + " " + idFrom.NodeText;
            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(idFrom);

            return this.result;
        }
    }
}
