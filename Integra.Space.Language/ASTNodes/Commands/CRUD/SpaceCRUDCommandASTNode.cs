//-----------------------------------------------------------------------
// <copyright file="SpaceCRUDCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using CommandContext;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class SpaceCRUDCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Context of the command.
        /// </summary>
        private AstNodeBase context;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.context = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "CRUD_COMMAND", ChildrenNodes[0]) as AstNodeBase;
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
            PipelineCommandContext contextAxu = (PipelineCommandContext)this.context.Evaluate(thread);
            this.EndEvaluate(thread);

            return contextAxu;
        }
    }
}
