//-----------------------------------------------------------------------
// <copyright file="GroupKeyValueNode.cs" company="Integra.Space.Language">
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
    /// Group key value class
    /// </summary>
    internal class GroupKeyValueNode : AstNodeBase
    {
        /// <summary>
        /// Result plan node
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// object node
        /// </summary>
        private AstNodeBase groupKey;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.groupKey = AddChild(NodeUseType.Keyword, SR.ObjectRole, ChildrenNodes[0]) as AstNodeBase;
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
            PlanNode groupKeyAux = (PlanNode)this.groupKey.Evaluate(thread);
            this.EndEvaluate(thread);

            if (groupKeyAux.NodeType.Equals(PlanNodeTypeEnum.GroupKey))
            {
                this.result = groupKeyAux;
            }
            else if (groupKeyAux.NodeType.Equals(PlanNodeTypeEnum.Identifier))
            {
                PlanNode key = new PlanNode();
                key.NodeType = PlanNodeTypeEnum.GroupKey;
                key.Column = ChildrenNodes[0].Token.Location.Column;
                key.Line = ChildrenNodes[0].Token.Location.Line;
                key.NodeText = ChildrenNodes[0].Token.Text;
                key.Properties.Add("Value", "Key");

                PlanNode property = new PlanNode();
                property.NodeType = PlanNodeTypeEnum.GroupKeyProperty;
                property.Properties.Add("Value", groupKeyAux.Properties["Value"]);
                property.Children = new List<PlanNode>();
                property.Children.Add(key);

                this.result = new PlanNode();
                this.result.NodeType = PlanNodeTypeEnum.GroupKeyValue;
                this.result.Column = groupKeyAux.Column;
                this.result.Line = groupKeyAux.Line;
                this.result.NodeText = groupKeyAux.NodeText;
                this.result.Children = new List<PlanNode>();
                this.result.Children.Add(property);
            }
            else
            {
                this.result = new PlanNode();
                this.result.NodeType = PlanNodeTypeEnum.GroupKeyValue;
                this.result.Column = groupKeyAux.Column;
                this.result.Line = groupKeyAux.Line;
                this.result.NodeText = groupKeyAux.NodeText;
                this.result.Children = new List<PlanNode>();
                this.result.Children.Add(groupKeyAux);
            }

            return this.result;
        }
    }
}
