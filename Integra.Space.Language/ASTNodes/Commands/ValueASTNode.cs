//-----------------------------------------------------------------------
// <copyright file="ValueASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using Integra.Space.Language.ASTNodes.Base;
    
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// StringNode class
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    internal sealed class ValueASTNode<TValue> : AstNodeBase
    {
        /// <summary>
        /// Node value.
        /// </summary>
        private TValue value;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.value = (TValue)treeNode.Token.Value;
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            return this.value;
        }
    }
}
