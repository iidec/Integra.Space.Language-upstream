//-----------------------------------------------------------------------
// <copyright file="NonConstantValueASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects
{
    using System.Collections.Generic;
    using System.Linq;
    using Base;
    using Commands;
    using Identifier;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// ConstantValueNode class
    /// </summary>
    internal sealed class NonConstantValueASTNode : AstNodeBase
    {
        /// <summary>
        /// List of identifiers.
        /// </summary>
        private ListASTNode<IdentifierASTNode, PlanNode> idList;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.idList = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[0]) as ListASTNode<IdentifierASTNode, PlanNode>;
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
            List<PlanNode> planNodeList = (List<PlanNode>)this.idList.Evaluate(thread);
            this.EndEvaluate(thread);

            PlanNode fromForLambda = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode child = fromForLambda;

            if (planNodeList.Count > 1)
            {
                PlanNode idSource = planNodeList.First();
                fromForLambda.Properties.Add("SourceName", idSource.Properties["Value"]);
                planNodeList.Remove(idSource);
            }

            foreach (PlanNode planNode in planNodeList)
            {
                planNode.NodeType = PlanNodeTypeEnum.Property;
                planNode.Properties.Add("Property", planNode.Properties["Value"]);
                planNode.Properties.Add("IsConstant", false);
                if (child.Properties.ContainsKey("SourceName"))
                {
                    planNode.Properties.Add("SourceName", child.Properties["SourceName"]);
                }
                else
                {
                    planNode.Properties.Add("SourceName", null);
                }

                planNode.Children = new List<PlanNode>();
                planNode.Children.Add(child);
                child = planNode;
            }

            return child;
        }
    }
}
