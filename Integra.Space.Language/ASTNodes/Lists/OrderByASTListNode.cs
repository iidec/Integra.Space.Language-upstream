//-----------------------------------------------------------------------
// <copyright file="OrderByASTListNode.cs" company="CompanyName">
//     Copyright (c) CompanyName. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Lists
{
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ListOfValuesWithoutAliasNode class
    /// </summary>
    internal sealed class OrderByASTListNode : AstNodeBase
    {
        /// <summary>
        /// parent node of the list
        /// </summary>
        private AstNodeBase parentNode;

        /// <summary>
        /// node to add to dictionary
        /// </summary>
        private AstNodeBase valueNode;

        /// <summary>
        /// dictionary of plans
        /// </summary>
        private Dictionary<PlanNode, PlanNode> result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 3)
            {
                this.parentNode = AddChild(NodeUseType.Parameter, "ParentNode", ChildrenNodes[0]) as AstNodeBase;
                this.valueNode = AddChild(NodeUseType.Parameter, "ValueNode", ChildrenNodes[2]) as AstNodeBase;
            }
            else if (childrenCount == 1)
            {
                this.parentNode = AddChild(NodeUseType.Parameter, "ParentNode", ChildrenNodes[0]) as AstNodeBase;
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
            if (childrenCount == 3)
            {
                Binding b2 = thread.Bind("ObjectList", BindingRequestFlags.Read);
                this.result = (Dictionary<PlanNode, PlanNode>)b2.GetValueRef(thread);

                this.parentNode.Evaluate(thread);
                PlanNode aux = (PlanNode)this.valueNode.Evaluate(thread);

                PlanNode fromForLambda = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.ObservableFromForLambda);

                PlanNode property = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.Property);
                property.NodeText = aux.Properties["Value"].ToString();
                property.Properties.Add("Property", aux.Properties["Value"]);
                property.Children = new List<PlanNode>();
                property.Children.Add(fromForLambda);

                this.result.Add(aux, property);
            }
            else if (childrenCount == 1)
            {
                Binding b2 = thread.Bind("ObjectList", BindingRequestFlags.Read);
                this.result = (Dictionary<PlanNode, PlanNode>)b2.GetValueRef(thread);

                PlanNode aux = (PlanNode)this.parentNode.Evaluate(thread);

                PlanNode fromForLambda = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.ObservableFromForLambda);

                PlanNode property = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.Property);
                property.NodeText = aux.Properties["Value"].ToString();
                property.Properties.Add("Property", aux.Properties["Value"]);
                property.Children = new List<PlanNode>();
                property.Children.Add(fromForLambda);

                this.result.Add(aux, property);
            }

            this.EndEvaluate(thread);

            return this.result;
        }
    }
}
