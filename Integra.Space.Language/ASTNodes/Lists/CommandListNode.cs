//-----------------------------------------------------------------------
// <copyright file="CommandListNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Lists
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Errors;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// CommandsNode class
    /// </summary>
    internal sealed class CommandListNode : AstNodeBase
    {
        /// <summary>
        /// previous commands
        /// </summary>
        private AstNodeBase parentNode;

        /// <summary>
        /// Reserved word 'next'
        /// </summary>
        private string nextWord;

        /// <summary>
        /// command: create, drop, alter, grant, revoke, deny, start, stop, set
        /// </summary>
        private AstNodeBase command;

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
                this.nextWord = (string)ChildrenNodes[1].Token.Value;
                this.command = AddChild(NodeUseType.Parameter, "Command", ChildrenNodes[2]) as AstNodeBase;
            }
            else
            {
                this.command = AddChild(NodeUseType.Parameter, "Command", ChildrenNodes[0]) as AstNodeBase;
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

            List<ErrorNode> errorList = new List<ErrorNode>();
            Binding b1 = thread.Bind("errorList", BindingRequestFlags.Write | BindingRequestFlags.NewOnly);
            b1.SetValueRef(thread, errorList);

            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 2)
            {
                this.result = (List<PlanNode>)this.parentNode.Evaluate(thread);
                PlanNode auxCommand = (PlanNode)this.command.Evaluate(thread);
                this.result.Add(auxCommand);
            }
            else
            {
                PlanNode auxCommand = (PlanNode)this.command.Evaluate(thread);
                this.result = new List<PlanNode>();
                this.result.Add(auxCommand);
            }

            this.EndEvaluate(thread);

            return this.result;
        }
    }
}
