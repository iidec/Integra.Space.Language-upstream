//-----------------------------------------------------------------------
// <copyright file="GroupKeyNode.cs" company="Integra.Space.Language">
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
    /// Group key class
    /// </summary>
    internal class GroupKeyNode : AstNodeBase
    {
        /// <summary>
        /// reserved word key
        /// </summary>
        private string reservedWordKey;

        /// <summary>
        /// Predecessor node
        /// </summary>
        private AstNodeBase predecessor;

        /// <summary>
        /// Key property
        /// </summary>
        private string property;

        /// <summary>
        /// Result plan node
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
            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 1)
            {
                this.reservedWordKey = (string)ChildrenNodes[0].Token.Value;
                
                this.result.NodeType = PlanNodeTypeEnum.GroupKey;
                this.result.Column = ChildrenNodes[0].Token.Location.Column;
                this.result.Line = ChildrenNodes[0].Token.Location.Line;
                this.result.NodeText = ChildrenNodes[0].Token.Text;
                this.result.Properties.Add("Value", "Key");
                this.result.Children = new List<PlanNode>();
            }
            else if (childrenCount == 2)
            {
                this.predecessor = AddChild(NodeUseType.Parameter, SR.PrpoertyProperty, ChildrenNodes[0]) as AstNodeBase;
                this.property = (string)ChildrenNodes[1].Token.Value;
                                
                this.result.NodeType = PlanNodeTypeEnum.GroupKeyProperty;
                this.result.Properties.Add("Value", this.property);
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
            if (childrenCount == 2)
            {
                PlanNode auxPredecessor = (PlanNode)this.predecessor.Evaluate(thread);

                this.result.Column = auxPredecessor.Column;
                this.result.Line = auxPredecessor.Line;
                this.result.NodeText = string.Format("{0}.{1}", auxPredecessor.NodeText, this.property);
                this.result.Children = new List<PlanNode>();
                this.result.Children.Add(auxPredecessor);
            }

            this.EndEvaluate(thread);

            return this.result;
        }
    }
}
