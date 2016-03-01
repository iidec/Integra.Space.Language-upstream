//-----------------------------------------------------------------------
// <copyright file="ConditionNode.cs" company="Integra.Space.Language">
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
    /// ConditionNode class
    /// </summary>
    internal class ConditionNode : AstNodeBase
    {
        /// <summary>
        /// conditions of the where expression
        /// </summary>
        private AstNodeBase condition;

        /// <summary>
        /// reserved word 'where'
        /// </summary>
        private string where;

        /// <summary>
        /// result plan
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionNode"/> class
        /// </summary>
        /// <param name="nodeType">Condition node type</param>
        public ConditionNode(PlanNodeTypeEnum nodeType)
        {
            this.result = new PlanNode();
            this.result.NodeType = nodeType;
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            if (ChildrenNodes.Count == 0)
            {
                return;
            }

            this.where = (string)ChildrenNodes[0].Token.Value;
            this.condition = AddChild(NodeUseType.Parameter, "whereConditions", ChildrenNodes[1]) as AstNodeBase;

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
            if (ChildrenNodes.Count == 0)
            {
                return null;
            }

            this.BeginEvaluate(thread);
            PlanNode conditionAux = (PlanNode)this.condition.Evaluate(thread);
            this.EndEvaluate(thread);

            PlanNode newScope = new PlanNode();
            newScope.NodeType = PlanNodeTypeEnum.NewScope;
            newScope.Children = new List<PlanNode>();

            this.result.NodeText = this.where + " " + conditionAux.NodeText;

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(newScope);
            this.result.Children.Add(conditionAux);

            return this.result;
        }
    }
}
