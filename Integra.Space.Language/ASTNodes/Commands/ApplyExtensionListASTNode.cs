//-----------------------------------------------------------------------
// <copyright file="ApplyExtensionListASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Permission list AST node class.
    /// </summary>
    /// <typeparam name="TListItem">Type of the AST node of the items of the list.</typeparam>
    internal class ApplyExtensionListASTNode<TListItem> : StatementListNode where TListItem : AstNodeBase
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

            List<PlanNode> resultList = new List<PlanNode>();

            foreach (TListItem child in this.permissionList)
            {
                PlanNode extension = (PlanNode)child.Evaluate(thread);

                if (resultList.Any(x => x.NodeType == extension.NodeType))
                {
                    thread.App.Parser.Context.AddParserError(Resources.ParseResults.DuplicateApplyExtension((int)ResultCodes.DuplicateApplyExtension, extension.NodeText));
                }

                resultList.Add(extension);
            }

            thread.CurrentNode = this;

            return resultList;
        }
    }
}
