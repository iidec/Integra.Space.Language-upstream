//-----------------------------------------------------------------------
// <copyright file="ConstantValueNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Constants
{
    using Integra.Space.Language.ASTNodes.Base;

    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ConstantValueNode class
    /// </summary>
    internal sealed class ConstantValueNode : AstNodeBase
    {
        /// <summary>
        /// result of the evaluated constant
        /// </summary>
        private AstNode value;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            int childernCount = ChildrenNodes.Count;

            if (childernCount == 1)
            {
                this.value = AddChild(NodeUseType.Keyword, "ConstantValue", ChildrenNodes[0]) as AstNodeBase;
            }
            else if (childernCount == 3)
            {
                this.value = AddChild(NodeUseType.Keyword, "ConstantValue", ChildrenNodes[1]) as AstNodeBase;
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
            PlanNode result = (PlanNode)this.value.Evaluate(thread);
            this.EndEvaluate(thread);

            int childernCount = ChildrenNodes.Count;
            
            if (childernCount == 3)
            {
                result.NodeText = string.Format("({0})", result.NodeText);
            }

            return result;
        }
    }
}
