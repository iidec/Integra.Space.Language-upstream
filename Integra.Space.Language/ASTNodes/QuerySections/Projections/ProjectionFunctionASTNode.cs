//-----------------------------------------------------------------------
// <copyright file="ProjectionFunctionASTNode.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
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
    /// Projection function node.
    /// </summary>
    internal class ProjectionFunctionASTNode : AstNodeBase
    {
        /// <summary>
        /// function name
        /// </summary>
        private string functionName;

        /// <summary>
        /// value node
        /// </summary>
        private AstNodeBase value;

        /// <summary>
        /// result execution plan
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
            if (childrenCount == 1)
            {
                this.functionName = (string)ChildrenNodes[0].Token.Value;                
            }
            else if (childrenCount == 2)
            {
                this.functionName = (string)ChildrenNodes[0].Token.Value;
                this.value = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[1]) as AstNodeBase;                
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

            int childrenCount = ChildrenNodes.Count;
            this.result = new PlanNode(this.Location.Line, this.Location.Column, this.SelectResultNodeType(this.functionName.ToString().ToLower()));
            if (childrenCount == 1)
            {
                this.result.Properties.Add("FunctionName", char.ToUpper(this.functionName[0]) + this.functionName.Substring(1));
                this.result.NodeText = string.Format("{0}()", this.functionName);
                this.result.Children = new List<PlanNode>();
            }
            else if (childrenCount == 2)
            {
                PlanNode valueAux = (PlanNode)this.value.Evaluate(thread);

                this.result.Properties.Add("FunctionName", char.ToUpper(this.functionName[0]) + this.functionName.Substring(1));
                this.result.NodeText = string.Format("{0}({1})", this.functionName, valueAux.NodeText);
                this.result.Children = new List<PlanNode>();

                PlanNode newScope = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.NewScope);
                newScope.Children = new List<PlanNode>();

                this.result.Children.Add(newScope);
                this.result.Children.Add(valueAux);
            }

            this.EndEvaluate(thread);

            return this.result;
        }

        /// <summary>
        /// Select the result node type based on the function name
        /// </summary>
        /// <param name="functionName">Projection function name.</param>
        /// <returns>The result node type.</returns>
        private PlanNodeTypeEnum SelectResultNodeType(string functionName)
        {
            PlanNodeTypeEnum resultType = PlanNodeTypeEnum.None;

            if (string.IsNullOrEmpty(functionName))
            {
                return resultType;
            }

            switch (functionName)
            {
                case "sum":
                    resultType = PlanNodeTypeEnum.EnumerableSum;
                    break;
                case "count":
                    resultType = PlanNodeTypeEnum.EnumerableCount;
                    break;
                case "max":
                    resultType = PlanNodeTypeEnum.EnumerableMax;
                    break;
                case "min":
                    resultType = PlanNodeTypeEnum.EnumerableMin;
                    break;
            }

            return resultType;
        }
    }
}
