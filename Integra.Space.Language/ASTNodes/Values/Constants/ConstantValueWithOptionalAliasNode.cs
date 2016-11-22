﻿//-----------------------------------------------------------------------
// <copyright file="ConstantValueWithOptionalAliasNode.cs" company="Integra.Space.Language">
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
    /// ConstantValueWithAliasNode class
    /// </summary>
    internal sealed class ConstantValueWithOptionalAliasNode : AstNodeBase
    {
        /// <summary>
        /// value node
        /// </summary>
        private AstNodeBase valueNode;

        /// <summary>
        /// reserved word 'as'
        /// </summary>
        private string tAs;

        /// <summary>
        /// alias node of the value
        /// </summary>
        private AstNodeBase aliasNode;

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

            this.result = new PlanNode();
            int childNodesCount = ChildrenNodes.Count;
            if (childNodesCount == 3)
            {
                this.valueNode = AddChild(NodeUseType.Parameter, "ValueNode", ChildrenNodes[0]) as AstNodeBase;
                this.tAs = (string)ChildrenNodes[1].Token.Value;
                this.aliasNode = AddChild(NodeUseType.Parameter, "AliasNode", ChildrenNodes[2]) as AstNodeBase;
                this.result.NodeType = PlanNodeTypeEnum.ValueWithAlias;
            }
            else if (childNodesCount == 1)
            {
                this.valueNode = AddChild(NodeUseType.Parameter, "ValueNode", ChildrenNodes[0]) as AstNodeBase;
                this.result.NodeType = PlanNodeTypeEnum.ValueWithAlias;
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

            this.result.Children = new List<PlanNode>();
            int childNodesCount = ChildrenNodes.Count;
            if (childNodesCount == 3)
            {
                PlanNode v = (PlanNode)this.valueNode.Evaluate(thread);
                PlanNode a = (PlanNode)this.aliasNode.Evaluate(thread);

                this.result.Column = v.Column;
                this.result.Line = v.Line;
                this.result.NodeText = v.NodeText + " " + this.tAs + " " + a.NodeText;
                this.result.Children.Add(v);
                this.result.Children.Add(a);
            }
            else if (childNodesCount == 1)
            {
                PlanNode alias = (PlanNode)this.valueNode.Evaluate(thread);

                PlanNode value = new PlanNode();
                value.NodeText = alias.NodeText;
                value.NodeType = PlanNodeTypeEnum.Identifier;

                foreach (KeyValuePair<string, object> property in alias.Properties)
                {
                    value.Properties.Add(property.Key, property.Value);
                }

                this.result.Column = value.Column;
                this.result.Line = value.Line;
                this.result.NodeText = value.NodeText;
                this.result.Children.Add(value);
                this.result.Children.Add(alias);
            }

            this.EndEvaluate(thread);

            return this.result;
        }
    }
}
