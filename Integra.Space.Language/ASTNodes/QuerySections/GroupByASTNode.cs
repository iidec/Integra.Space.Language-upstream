//-----------------------------------------------------------------------
// <copyright file="GroupByASTNode.cs" company="Integra.Space.Language">
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
    internal class GroupByASTNode : AstNodeBase
    {
        /// <summary>
        /// reserved word group
        /// </summary>
        private string groupWord;

        /// <summary>
        /// reserved word by
        /// </summary>
        private string reservedWordBy;

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
            this.groupWord = (string)ChildrenNodes[0].Token.Value;
            this.reservedWordBy = (string)ChildrenNodes[1].Token.Value;
            this.listOfValues = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[2]) as AstNodeBase;

            this.result = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.EnumerableGroupBy;
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

            PlanNode planProjection = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
            planProjection.Column = ChildrenNodes[0].Token.Location.Column;
            planProjection.Line = ChildrenNodes[0].Token.Location.Line;
            planProjection.NodeType = PlanNodeTypeEnum.Projection;
            planProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.EnumerableGroupBy);
            planProjection.Properties.Add("OverrideGetHashCodeMethod", false);
            planProjection.Children = new List<PlanNode>();

            Dictionary<PlanNode, PlanNode> projection = new Dictionary<PlanNode, PlanNode>();
            Binding b1 = thread.Bind("ObjectList", BindingRequestFlags.Write | BindingRequestFlags.ExistingOrNew);
            b1.SetValueRef(thread, projection);

            this.listOfValues.Evaluate(thread);

            Binding b2 = thread.Bind("ObjectList", BindingRequestFlags.Read);

            this.result.NodeText = this.groupWord + " " + this.reservedWordBy;

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

                /*PlanNode fromForLambda = new PlanNode();
                fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

                List<PlanNode> last = tupla.Value.Children;
                List<PlanNode> tuplaActual = new List<PlanNode>();

                while (last != null)
                {
                    tuplaActual = last;
                    last = tuplaActual.First().Children;

                    if (tuplaActual.First().Children == null)
                    {
                        tuplaActual.First().Children = new List<PlanNode>();
                        tuplaActual.First().Children.Add(fromLambda);
                    }
                }

                // se agrega el nodo from for lambda al nodo valor del nodo tupla
                this.SetFromForLambda(tupla.Value, fromForLambda);*/

                if (isFirst)
                {
                    isFirst = false;
                    this.result.NodeText += " " + tupla.Value.NodeText + " as " + tupla.Key.NodeText;
                }
                else
                {
                    this.result.NodeText += ", " + tupla.Value.NodeText + " as " + tupla.Key.NodeText;
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
