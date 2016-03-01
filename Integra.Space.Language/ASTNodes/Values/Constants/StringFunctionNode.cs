//-----------------------------------------------------------------------
// <copyright file="StringFunctionNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Constants
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// DateFunctionNode class
    /// </summary>
    internal sealed class StringFunctionNode : AstNodeBase
    {
        /// <summary>
        /// DateTime or Timespan node
        /// </summary>
        private AstNodeBase number;

        /// <summary>
        /// function to execute
        /// </summary>
        private string function;

        /// <summary>
        /// value node
        /// </summary>
        private AstNode value;

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
            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 2)
            {
                this.function = (string)ChildrenNodes[0].Token.Value;
                this.value = AddChild(NodeUseType.Parameter, "ValueNode", ChildrenNodes[1]) as AstNodeBase;
            }
            else if (childrenCount == 3)
            {
                this.function = (string)ChildrenNodes[0].Token.Value;
                this.value = AddChild(NodeUseType.Parameter, "ValueNode", ChildrenNodes[1]) as AstNodeBase;
                this.number = AddChild(NodeUseType.Parameter, "DateTimeNode", ChildrenNodes[2]) as AstNodeBase;
            }

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
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
            int childrenCount = ChildrenNodes.Count;
            PlanNode valueAux = null;
            PlanNode numberAux = null;

            if (childrenCount == 2)
            {
                valueAux = (PlanNode)this.value.Evaluate(thread);

                this.result.NodeText = string.Format("{0}({1})", this.function, valueAux.NodeText);
            }
            else if (childrenCount == 3)
            {
                valueAux = (PlanNode)this.value.Evaluate(thread);
                numberAux = (PlanNode)this.number.Evaluate(thread);

                this.result.NodeText = string.Format("{0}({1},{2})", this.function, valueAux.NodeText, numberAux.NodeText);
                this.result.Properties.Add("Number", numberAux);
            }

            this.EndEvaluate(thread);

            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(valueAux);

            switch (this.function.ToLower())
            {
                case "left":
                    this.result.NodeType = PlanNodeTypeEnum.StringLeftFunction;
                    break;
                case "right":
                    this.result.NodeType = PlanNodeTypeEnum.StringRightFunction;
                    break;
                case "upper":
                    this.result.NodeType = PlanNodeTypeEnum.StringUpperFunction;
                    break;
                case "lower":
                    this.result.NodeType = PlanNodeTypeEnum.StringLowerFunction;
                    break;
            }

            this.result.Properties.Add("IsConstant", bool.Parse(valueAux.Properties["IsConstant"].ToString()));

            return this.result;
        }
    }
}
