//-----------------------------------------------------------------------
// <copyright file="SelectNode.cs" company="Integra.Space.Language">
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
    /// SelectNode class
    /// </summary>
    internal sealed class SelectNode : AstNodeBase
    {
        /// <summary>
        /// reserved word "select"
        /// </summary>
        private string select;

        /// <summary>
        /// reserved word "top"
        /// </summary>
        private AstNodeBase top;

        /// <summary>
        /// list of values for the projection
        /// </summary>
        private AstNodeBase listOfValues;

        /// <summary>
        /// result of the evaluation
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

            if (childrenCount == 2)
            {
                this.select = (string)ChildrenNodes[0].Token.Value;
                this.listOfValues = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[1]) as AstNodeBase;

                this.result.NodeText = this.select;
            }
            else if (childrenCount == 3)
            {
                this.select = (string)ChildrenNodes[0].Token.Value;
                this.top = AddChild(NodeUseType.Parameter, "topNode", ChildrenNodes[1]) as AstNodeBase;
                this.listOfValues = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[2]) as AstNodeBase;

                this.result.NodeText = this.select;
            }

            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.Projection;
            this.result.Properties.Add("ProjectionType", PlanNodeTypeEnum.ObservableSelect);
            this.result.Properties.Add("OverrideGetHashCodeMethod", false);
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
            if (childrenCount == 3)
            {
                PlanNode topAux = (PlanNode)this.top.Evaluate(thread);

                PlanNode planTop = new PlanNode();
                planTop.NodeType = PlanNodeTypeEnum.EnumerableTake;
                planTop.Children = new List<PlanNode>();

                PlanNode newScope = new PlanNode();
                newScope.NodeType = PlanNodeTypeEnum.NewScope;

                planTop.Children.Add(newScope);
                planTop.Children.Add(topAux);

                this.result.Properties.Add(PlanNodeTypeEnum.EnumerableTake.ToString(), planTop);
            }

            Dictionary<PlanNode, PlanNode> projection = new Dictionary<PlanNode, PlanNode>();
            Binding b1 = thread.Bind("ObjectList", BindingRequestFlags.Write | BindingRequestFlags.ExistingOrNew);
            b1.SetValueRef(thread, projection);

            this.listOfValues.Evaluate(thread);

            Binding b2 = thread.Bind("ObjectList", BindingRequestFlags.Read);

            this.EndEvaluate(thread);

            projection = (Dictionary<PlanNode, PlanNode>)b2.GetValueRef(thread);
            this.result.Children = new List<PlanNode>();

            bool isFirst = true;
            foreach (var tupla in projection)
            {
                PlanNode plan = new PlanNode();
                plan.NodeType = PlanNodeTypeEnum.TupleProjection;
                plan.Children = new List<PlanNode>();
                plan.Children.Add(tupla.Key);
                plan.Children.Add(tupla.Value);

                PlanNode fromLambda = new PlanNode();
                fromLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

                if (tupla.Value.NodeType.Equals(PlanNodeTypeEnum.EnumerableSum) || tupla.Value.NodeType.Equals(PlanNodeTypeEnum.EnumerableMax) || tupla.Value.NodeType.Equals(PlanNodeTypeEnum.EnumerableMin))
                {
                    // se le agrega el from lambda a las extensiones que reciben dos parametros, donde el último de ellos es una funcion
                    tupla.Value.Children.ElementAt(0).Children = new List<PlanNode>();
                    tupla.Value.Children.ElementAt(0).Children.Add(fromLambda);
                }
                else if (tupla.Value.NodeType.Equals(PlanNodeTypeEnum.EnumerableCount))
                {
                    // se le agrega el from lambda a las extensiones que reciben un parametro
                    tupla.Value.Children.Add(fromLambda);
                }

                if (isFirst)
                {
                    isFirst = false;
                    this.result.NodeText += " " + tupla.Value.NodeText + " as " + tupla.Key.NodeText;
                }
                else
                {
                    this.result.NodeText += ", " + tupla.Value.NodeText + " as " + tupla.Key.NodeText;
                }

                this.result.Children.Add(plan);
            }

            return this.result;
        }
    }
}
