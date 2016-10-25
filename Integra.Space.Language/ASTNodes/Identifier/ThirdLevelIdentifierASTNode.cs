//-----------------------------------------------------------------------
// <copyright file="ThirdLevelIdentifierASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Identifier
{
    using System;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// IdentifierNode class
    /// </summary>
    internal sealed class ThirdLevelIdentifierASTNode : AstNodeBase
    {
        /// <summary>
        /// Object identifier.
        /// </summary>
        private string identifier;

        /// <summary>
        /// Database name.
        /// </summary>
        private string databaseName;

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
                this.identifier = (string)ChildrenNodes[0].Token.Value;
            }
            else if (ChildrenNodes.Count == 2)
            {
                this.databaseName = (string)ChildrenNodes[0].Token.Value;
                this.identifier = (string)ChildrenNodes[1].Token.Value;
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
            return Tuple.Create<string, string, string>(this.databaseName, null, this.identifier);
        }
    }
}
