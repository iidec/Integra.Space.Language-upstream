//-----------------------------------------------------------------------
// <copyright file="IsNullFunctionASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Values.Functions
{
    using System;
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Math function node class
    /// </summary>
    internal class IsNullFunctionASTNode : AstNodeBase
    {
        /// <summary>
        /// operator of the expression
        /// </summary>
        private string isnullReserverdWord;

        /// <summary>
        /// minuend of the subtract
        /// </summary>
        private AstNodeBase checkExpression;

        /// <summary>
        /// subtrahend of the subtract
        /// </summary>
        private AstNodeBase replacementValue;

        /// <summary>
        /// this.result plan
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

            this.isnullReserverdWord = (string)ChildrenNodes[0].Token.Value;
            this.checkExpression = AddChild(NodeUseType.Parameter, "CheckExpression", ChildrenNodes[1]) as AstNodeBase;
            this.replacementValue = AddChild(NodeUseType.Parameter, "ReplacementValue", ChildrenNodes[2]) as AstNodeBase;

            this.result = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.IsNullFunction, this.NodeText);
            this.result.Children = new List<PlanNode>();
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
            PlanNode auxCheckExpression = (PlanNode)this.checkExpression.Evaluate(thread);
            PlanNode auxReplacementValue = (PlanNode)this.replacementValue.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.NodeText = string.Format("{0}({1}, {2})", this.isnullReserverdWord, auxCheckExpression.NodeText, auxReplacementValue.NodeText);
            this.result.Children.Add(auxCheckExpression);
            this.result.Children.Add(auxReplacementValue);

            bool isConstant = (bool)auxCheckExpression.Properties["IsConstant"] && (bool)auxReplacementValue.Properties["IsConstant"];
            Type returnValueType = null;

            if (auxCheckExpression.Properties["DataType"].Equals(auxReplacementValue.Properties["DataType"]))
            {
                returnValueType = Type.GetType(auxCheckExpression.Properties["DataType"].ToString());
            }
            else
            {
                if (auxCheckExpression.Properties["DataType"].Equals(typeof(object)) && !auxReplacementValue.Properties["DataType"].Equals(typeof(object)))
                {
                    returnValueType = Type.GetType(auxReplacementValue.Properties["DataType"].ToString());
                }
                else if (!auxCheckExpression.Properties["DataType"].Equals(typeof(object)) && auxReplacementValue.Properties["DataType"].Equals(typeof(object)))
                {
                    returnValueType = Type.GetType(auxReplacementValue.Properties["DataType"].ToString());
                }
                else
                {
                    returnValueType = typeof(object);
                }
            }

            this.result.Properties.Add("DataType", returnValueType);
            this.result.Properties.Add("IsConstant", isConstant);

            return this.result;
        }
    }
}
