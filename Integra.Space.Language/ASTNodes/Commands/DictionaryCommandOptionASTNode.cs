//-----------------------------------------------------------------------
// <copyright file="DictionaryCommandOptionASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Permission list AST node class.
    /// </summary>
    /// <typeparam name="TOptionEnum">Type of the options of the result list.</typeparam>
    internal class DictionaryCommandOptionASTNode<TOptionEnum> : StatementListNode where TOptionEnum : struct, System.IConvertible
    {
        /// <summary>
        /// List of permissions.
        /// </summary>
        private List<CommandOptionASTNode<TOptionEnum>> permissionList;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.permissionList = new List<CommandOptionASTNode<TOptionEnum>>();
            foreach (CommandOptionASTNode<TOptionEnum> child in this.GetChildNodes())
            {
                this.permissionList.Add(child);
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
            thread.CurrentNode = this;

            Dictionary<TOptionEnum, object> resultDictionary = new Dictionary<TOptionEnum, object>();

            foreach (CommandOptionASTNode<TOptionEnum> child in this.permissionList)
            {
                CommandOption<TOptionEnum> option = (CommandOption<TOptionEnum>)child.Evaluate(thread);
                if (resultDictionary.ContainsKey(option.Option))
                {
                    throw new Exceptions.SyntaxException(string.Format("Option '{0}' is specified more than once.", option));
                }
                else
                {
                    resultDictionary.Add(option.Option, option.Value);
                }
            }
            
            thread.CurrentNode = this;

            return resultDictionary;
        }
    }
}
