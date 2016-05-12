//-----------------------------------------------------------------------
// <copyright file="MathFunctionNode.cs" company="Integra.Space.Language">
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
    /// Math function node class
    /// </summary>
    internal sealed class MathFunctionNode : AstNodeBase
    {
        /// <summary>
        /// DateTime or Timespan node
        /// </summary>
        private AstNodeBase numericExpression;

        /// <summary>
        /// function to execute
        /// </summary>
        private string function;

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
            this.function = (string)ChildrenNodes[0].Token.Value;
            this.numericExpression = AddChild(NodeUseType.Parameter, "MathFunctionNode", ChildrenNodes[1]) as AstNodeBase;

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
            PlanNode auxValue = (PlanNode)this.numericExpression.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(auxValue);
            this.result.NodeText = string.Format("{0}({1})", this.function, auxValue.NodeText);
            this.result.Properties.Add("DataType", auxValue.Properties["DataType"]);
            this.result.NodeType = PlanNodeTypeEnum.MathFunctionWithOneParameter;
            this.result.Properties.Add("IsConstant", bool.Parse(auxValue.Properties["IsConstant"].ToString()));

            switch (this.function.ToLower())
            {
                case "abs":
                    this.result.Properties.Add("Function", "Abs");
                    break;                
            }

            return this.result;
        }
    }
}
