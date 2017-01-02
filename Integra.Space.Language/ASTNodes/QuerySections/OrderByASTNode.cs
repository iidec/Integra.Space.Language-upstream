//-----------------------------------------------------------------------
// <copyright file="OrderByASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// GroupByNode class
    /// </summary>
    internal class OrderByASTNode : AstNodeBase
    {
        /// <summary>
        /// reserved word group
        /// </summary>
        private string orderWord;

        /// <summary>
        /// reserved word by
        /// </summary>
        private string reservedWordBy;

        /// <summary>
        /// reserved word direction for order by
        /// </summary>
        private string reservedWordDirection;

        /// <summary>
        /// list of values for the projection
        /// </summary>
        private AstNodeBase listOfValues;

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

            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 0)
            {
                return;
            }
            else if (childrenCount == 3)
            {
                this.orderWord = (string)ChildrenNodes[0].Token.Value;
                this.reservedWordBy = (string)ChildrenNodes[1].Token.Value;
                this.listOfValues = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[2]) as AstNodeBase;
            }
            else if (childrenCount == 4)
            {
                this.orderWord = (string)ChildrenNodes[0].Token.Value;
                this.reservedWordBy = (string)ChildrenNodes[1].Token.Value;
                this.reservedWordDirection = (string)ChildrenNodes[2].Token.Value;
                this.listOfValues = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[3]) as AstNodeBase;
            }

            this.result = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
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

            PlanNode planProjection = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
            planProjection.Column = ChildrenNodes[0].Token.Location.Column;
            planProjection.Line = ChildrenNodes[0].Token.Location.Line;
            planProjection.NodeType = PlanNodeTypeEnum.Projection;
            planProjection.Properties.Add("OverrideGetHashCodeMethod", false);
            planProjection.Children = new List<PlanNode>();

            if (this.reservedWordDirection == null)
            {
                this.result.NodeType = PlanNodeTypeEnum.EnumerableOrderBy;
                planProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.EnumerableOrderBy);
            }
            else
            {
                if (this.reservedWordDirection.Equals("asc", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.result.NodeType = PlanNodeTypeEnum.EnumerableOrderBy;
                    planProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.EnumerableOrderBy);
                }
                else if (this.reservedWordDirection.Equals("desc", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    this.result.NodeType = PlanNodeTypeEnum.EnumerableOrderByDesc;
                    planProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.EnumerableOrderByDesc);
                }
            }

            Dictionary<PlanNode, PlanNode> projection = new Dictionary<PlanNode, PlanNode>();
            Binding b1 = thread.Bind("ObjectList", BindingRequestFlags.Write | BindingRequestFlags.ExistingOrNew);
            b1.SetValueRef(thread, projection);

            this.listOfValues.Evaluate(thread);

            Binding b2 = thread.Bind("ObjectList", BindingRequestFlags.Read);

            this.result.NodeText = this.orderWord + " " + this.reservedWordBy;

            projection = (Dictionary<PlanNode, PlanNode>)b2.GetValueRef(thread);
            this.result.Children = new List<PlanNode>();

            bool isFirst = true;
            foreach (var tupla in projection)
            {
                PlanNode plan = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
                plan.NodeType = PlanNodeTypeEnum.TupleProjection;
                plan.Children = new List<PlanNode>();
                plan.Children.Add(tupla.Key);
                plan.Children.Add(tupla.Value);

                if (isFirst)
                {
                    isFirst = false;
                    this.result.NodeText += string.Format(" {0} {1}", this.reservedWordDirection, tupla.Value.NodeText);
                }
                else
                {
                    this.result.NodeText += string.Format(", {0}", tupla.Value.NodeText);
                }

                planProjection.Children.Add(plan);
            }

            PlanNode newScope1 = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
            newScope1.NodeType = PlanNodeTypeEnum.NewScope;
            newScope1.Children = new List<PlanNode>();

            this.result.Children.Add(newScope1);
            this.result.Children.Add(planProjection);

            return this.result;
        }

        /// <summary>
        /// Set the from for lambda node for a value node in the tuple projection.
        /// </summary>
        /// <param name="plan">Value plan node.</param>
        /// <param name="fromLambda">From for lambda node to add.</param>
        private void SetFromForLambda(PlanNode plan, PlanNode fromLambda)
        {
            if (plan.Children == null)
            {
                plan.Children = new List<PlanNode>();
                plan.Children.Add(fromLambda);
            }
            else
            {
                this.SetFromForLambda(plan.Children.First(), fromLambda);
            }
        }
    }
}
