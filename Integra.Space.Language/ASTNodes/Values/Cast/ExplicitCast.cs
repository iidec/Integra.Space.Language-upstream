//-----------------------------------------------------------------------
// <copyright file="ExplicitCast.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Cast
{
    using System;
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Explicit cast class
    /// </summary>
    internal class ExplicitCast : AstNodeBase
    {
        /// <summary>
        /// target type for cast
        /// </summary>
        private AstNodeBase targetType;

        /// <summary>
        /// value to cast
        /// </summary>
        private AstNodeBase value;

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
            this.targetType = AddChild(NodeUseType.Parameter, "TargetType", ChildrenNodes[0]) as AstNodeBase;
            this.value = AddChild(NodeUseType.Parameter, "Value", ChildrenNodes[1]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.Cast;
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
            PlanNode targetTypeAux = (PlanNode)this.targetType.Evaluate(thread);
            PlanNode valueAux = (PlanNode)this.value.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.Properties.Add("DataType", (Type)targetTypeAux.Properties["Value"]);
            this.result.NodeText = string.Format("({0}){1}", targetTypeAux.NodeText, valueAux.NodeText);
            this.result.Properties.Add("IsConstant", bool.Parse(valueAux.Properties["IsConstant"].ToString()));
            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(valueAux);

            if (valueAux.Properties.ContainsKey("PropertyName"))
            {
                this.result.Properties.Add("PropertyName", valueAux.Properties["PropertyName"]);
            }

            return this.result;
        }
    }
}
