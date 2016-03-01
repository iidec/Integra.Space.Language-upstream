//-----------------------------------------------------------------------
// <copyright file="TopNode.cs" company="Integra.Space.Language">
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
    /// Top node class
    /// </summary>
    internal sealed class TopNode : AstNodeBase
    {
        /// <summary>
        /// reserved word "top"
        /// </summary>
        private string top;

        /// <summary>
        /// top value, is a number value
        /// </summary>
        private AstNodeBase topValue;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.top = (string)ChildrenNodes[0].Token.Value;
            this.topValue = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[1]) as AstNodeBase;
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
            PlanNode topValueAux = (PlanNode)this.topValue.Evaluate(thread);
            this.EndEvaluate(thread);

            topValueAux.NodeText = string.Format("{0} {1}", this.top, topValueAux.NodeText);

            return topValueAux;
        }
    }
}
