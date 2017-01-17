//-----------------------------------------------------------------------
// <copyright file="SourceColumnsASTNode.cs" company="Integra.Space.Language">
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
    /// Source column AST node.
    /// </summary>
    internal class SourceColumnsASTNode : AstNodeBase
    {
        /// <summary>
        /// Action enumerable type.
        /// </summary>
        private Type columnType;

        /// <summary>
        /// Object identifier.
        /// </summary>
        private string columnIdentifier;

        /// <summary>
        /// Column length.
        /// </summary>
        private uint? columnLength;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            int childCount = ChildrenNodes.Count;
            if (childCount == 2)
            {
                this.columnIdentifier = (string)ChildrenNodes[0].Token.Value;
                this.columnType = (Type)ChildrenNodes[1].Token.Value;
            }
            else if (childCount == 3)
            {
                this.columnIdentifier = (string)ChildrenNodes[0].Token.Value;
                this.columnType = (Type)ChildrenNodes[1].Token.Value;
                this.columnLength = (uint)(int)ChildrenNodes[2].Token.Value;
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
            this.EndEvaluate(thread);

            SourceColumnTypeNode type = new SourceColumnTypeNode(this.columnType, this.columnLength, null);
            SourceColumnNode column = new SourceColumnNode(this.columnIdentifier, type);
            return column;
        }
    }
}
