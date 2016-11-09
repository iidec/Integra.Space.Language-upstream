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
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            
            this.columnIdentifier = (string)ChildrenNodes[0].Token.Value;
            this.columnType = (Type)ChildrenNodes[1].Token.Value;
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

            return Tuple.Create(this.columnIdentifier, this.columnType);
        }
    }
}
