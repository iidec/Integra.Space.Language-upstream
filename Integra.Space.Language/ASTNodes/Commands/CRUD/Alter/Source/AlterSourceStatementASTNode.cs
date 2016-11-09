//-----------------------------------------------------------------------
// <copyright file="AlterSourceStatementASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class AlterSourceStatementASTNode : AstNodeBase
    {
        /// <summary>
        /// Source columns.
        /// </summary>
        private AstNodeBase columns;

        /// <summary>
        /// Options AST node.
        /// </summary>
        private DictionaryCommandOptionASTNode<SourceOptionEnum> options;
        
        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            int childrenCount = this.ChildrenNodes.Count;

            if (childrenCount == 1)
            {
                this.columns = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "SOURCE_COLUMNS", ChildrenNodes[0]) as AstNodeBase;
            }
            else if (childrenCount == 2)
            {
                this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[1]) as DictionaryCommandOptionASTNode<SourceOptionEnum>;
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
            int childrenCount = this.ChildrenNodes.Count;
            this.BeginEvaluate(thread);

            if (childrenCount == 1)
            {
                Tuple<string, Dictionary<string, Type>> columnsAux = (Tuple<string, Dictionary<string, Type>>)this.columns.Evaluate(thread);
                this.EndEvaluate(thread);
                return columnsAux;
            }
            else if (childrenCount == 2)
            {
                Dictionary<SourceOptionEnum, object> optionsAux = new Dictionary<SourceOptionEnum, object>();
                if (this.options != null)
                {
                    optionsAux = (Dictionary<SourceOptionEnum, object>)this.options.Evaluate(thread);
                }

                this.EndEvaluate(thread);
                return optionsAux;
            }
            else
            {
                throw new Exception("Invalid alter source statement.");
            }
        }
    }
}
