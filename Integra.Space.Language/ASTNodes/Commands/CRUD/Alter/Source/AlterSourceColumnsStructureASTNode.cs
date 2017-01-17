//-----------------------------------------------------------------------
// <copyright file="AlterSourceColumnsStructureASTNode.cs" company="Integra.Space.Language">
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
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class AlterSourceColumnsStructureASTNode : AstNodeBase
    {
        /// <summary>
        /// Source columns.
        /// </summary>
        private ListASTNode<SourceColumnsASTNode, SourceColumnNode> columns;
        
        /// <summary>
        /// Add or remove action.
        /// </summary>
        private string action;
        
        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.action = (string)ChildrenNodes[0].Token.Value;
            this.columns = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "SOURCE_COLUMNS", ChildrenNodes[1]) as ListASTNode<SourceColumnsASTNode, SourceColumnNode>;
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
            List<SourceColumnNode> columnsAux = (List<SourceColumnNode>)this.columns.Evaluate(thread);
            this.EndEvaluate(thread);

            return Tuple.Create(this.action, columnsAux);
        }
    }
}
