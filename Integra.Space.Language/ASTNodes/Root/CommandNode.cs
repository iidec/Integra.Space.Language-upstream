//-----------------------------------------------------------------------
// <copyright file="CommandNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Root
{
    using CommandContext;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Command node class
    /// </summary>
    internal sealed class CommandNode : AstNodeBase
    {
        /// <summary>
        /// result of the evaluated constant
        /// </summary>
        private AstNodeBase command;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.command = AddChild(NodeUseType.ValueRead, "COMMAND", ChildrenNodes[0]) as AstNodeBase;
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
            PipelineCommandContext context = (PipelineCommandContext)this.command.Evaluate(thread);
            this.EndEvaluate(thread);
            
            return context;
        }
    }
}
