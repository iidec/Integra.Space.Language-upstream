//-----------------------------------------------------------------------
// <copyright file="TemporalStreamCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.MetadataQuery
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;
    using Runtime;

    /// <summary>
    /// JoinNode class
    /// </summary>
    internal sealed class TemporalStreamCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// on node
        /// </summary>
        private AstNodeBase metadataQuery;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.metadataQuery = AddChild(NodeUseType.Parameter, "TemporalStream", ChildrenNodes[0]) as AstNodeBase;
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
            TemporalStreamNode command = (TemporalStreamNode)this.metadataQuery.Evaluate(thread);
            this.EndEvaluate(thread);

            return command;
        }
    }
}
