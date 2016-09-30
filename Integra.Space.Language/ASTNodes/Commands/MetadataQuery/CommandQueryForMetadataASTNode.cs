//-----------------------------------------------------------------------
// <copyright file="CommandQueryForMetadataASTNode.cs" company="Integra.Space.Language">
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
    internal sealed class CommandQueryForMetadataASTNode : AstNodeBase
    {
        /// <summary>
        /// on node
        /// </summary>
        private AstNodeBase metadataQuery;

        /// <summary>
        /// result plan
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.metadataQuery = AddChild(NodeUseType.Parameter, "MetadataQuery", ChildrenNodes[0]) as AstNodeBase;

            this.result = new PlanNode();
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
            PlanNode query = (PlanNode)this.metadataQuery.Evaluate(thread);
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            return new QueryCommandForMetadataNode(Common.ActionCommandEnum.ViewDefinition, query, this.Location.Line, this.Location.Column, this.GetNodeText(), null, databaseName);
        }
    }
}
