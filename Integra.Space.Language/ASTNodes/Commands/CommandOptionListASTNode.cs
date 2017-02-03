//-----------------------------------------------------------------------
// <copyright file="CommandOptionListASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System.Collections.Generic;
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space object AST node class.
    /// </summary>
    /// <typeparam name="TOptionEnum">Type of the options enumerable.</typeparam>
    internal class CommandOptionListASTNode<TOptionEnum> : AstNodeBase where TOptionEnum : struct, System.IConvertible
    {
        /// <summary>
        /// Terminal initiator.
        /// </summary>
        private string iniciador;

        /// <summary>
        /// Dictionary of options.
        /// </summary>
        private DictionaryCommandOptionASTNode<TOptionEnum> options;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            if (ChildrenNodes.Count == 1)
            {
                this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[0]) as DictionaryCommandOptionASTNode<TOptionEnum>;
            }
            else if (ChildrenNodes.Count == 2)
            {
                this.iniciador = (string)ChildrenNodes[0].Token.Value;
                this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[1]) as DictionaryCommandOptionASTNode<TOptionEnum>;
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
            Dictionary<TOptionEnum, object> optionsAux = new Dictionary<TOptionEnum, object>();

            this.BeginEvaluate(thread);
            if (ChildrenNodes.Count != 0)
            {
                optionsAux = (Dictionary<TOptionEnum, object>)this.options.Evaluate(thread);
            }

            this.EndEvaluate(thread);

            return optionsAux;
        }
    }
}
