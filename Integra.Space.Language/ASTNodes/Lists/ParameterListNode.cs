//-----------------------------------------------------------------------
// <copyright file="ParameterListNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
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
    /// ParameterListNode class
    /// </summary>
    internal sealed class ParameterListNode : AstNodeBase
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
        private List<PlanNode> result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 2)
            {
                this.parentNode = AddChild(NodeUseType.Parameter, "ParentNode", ChildrenNodes[0]) as AstNodeBase;
                this.valueNode = AddChild(NodeUseType.Parameter, "ActualNode", ChildrenNodes[1]) as AstNodeBase;
            }
            else
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
            this.result = new List<PlanNode>();
            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 2)
            {
                List<PlanNode> auxList = (List<PlanNode>)this.parentNode.Evaluate(thread);
                PlanNode auxActual = (PlanNode)this.valueNode.Evaluate(thread);
                this.result = auxList;
                this.result.Add(auxActual);
            }
            else
            {
                PlanNode aux = (PlanNode)this.parentNode.Evaluate(thread);
                this.result.Add(aux);
            }

            this.EndEvaluate(thread);

            return this.result;
        }
    }
}
