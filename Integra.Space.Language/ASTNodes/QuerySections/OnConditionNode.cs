//-----------------------------------------------------------------------
// <copyright file="OnConditionNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using Integra.Space.Language.ASTNodes.Base;
    
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// OnConditionNode class
    /// </summary>
    internal sealed class OnConditionNode : AstNodeBase
    {
        /// <summary>
        /// condition of the on node
        /// </summary>
        private AstNodeBase condition;

        /// <summary>
        /// reserved word 'on'
        /// </summary>
        private string on;

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
            this.on = (string)ChildrenNodes[0].Token.Value;
            this.condition = AddChild(NodeUseType.Parameter, "condition", ChildrenNodes[1]) as AstNodeBase;
            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.On;
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
            PlanNode conditionOn = (PlanNode)this.condition.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.NodeText = this.on + " " + conditionOn.NodeText;
            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(conditionOn);

            return this.result;
        }
    }
}
