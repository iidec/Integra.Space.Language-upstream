//-----------------------------------------------------------------------
// <copyright file="DictionaryASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Permission list AST node class.
    /// </summary>
    /// <typeparam name="TListItem">Type of the AST node of the items of the list.</typeparam>
    /// <typeparam name="TResultKeyItem">Type of the items of the result dictionary.</typeparam>
    /// <typeparam name="TResultValueItem">Type of the value of the result dictionary.</typeparam>
    internal class DictionaryASTNode<TListItem, TResultKeyItem, TResultValueItem> : StatementListNode where TListItem : AstNodeBase
    {
        /// <summary>
        /// List of permissions.
        /// </summary>
        private List<TListItem> permissionList;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.permissionList = new List<TListItem>();
            foreach (TListItem child in this.GetChildNodes())
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

            Dictionary<TResultKeyItem, TResultValueItem> resultDictionary = new Dictionary<TResultKeyItem, TResultValueItem>();

            foreach (TListItem child in this.permissionList)
            {
                Tuple<TResultKeyItem, TResultValueItem> t = (Tuple<TResultKeyItem, TResultValueItem>)child.Evaluate(thread);

                if (!resultDictionary.ContainsKey(t.Item1))
                {
                    resultDictionary.Add(t.Item1, t.Item2);
                }
                else
                {
                    thread.App.Parser.Context.AddParserError(Resources.ParseResults.DuplicateColumn((int)ResultCodes.DuplicateColumn, t.Item1));
                }
            }

            thread.CurrentNode = this;

            return resultDictionary;
        }
    }
}
