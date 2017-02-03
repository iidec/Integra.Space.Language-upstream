//-----------------------------------------------------------------------
// <copyright file="FourthLevelIdentifierPlanNodeASTNode.cs" company="Integra.Space.Language">
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
    internal sealed class FourthLevelIdentifierPlanNodeASTNode : AstNodeBase
    {
        /// <summary>
        /// Object identifier.
        /// </summary>
        private AstNodeBase identifier;

        /// <summary>
        /// Schema identifier.
        /// </summary>
        private AstNodeBase schemaName;

        /// <summary>
        /// Database name.
        /// </summary>
        private AstNodeBase databaseName;

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
                this.identifier = AddChild(NodeUseType.ValueRead, "identifier", ChildrenNodes[0]) as AstNodeBase;
            }
            else if (ChildrenNodes.Count == 2)
            {
                this.schemaName = AddChild(NodeUseType.ValueRead, "schemaIdentifier", ChildrenNodes[0]) as AstNodeBase;
                this.identifier = AddChild(NodeUseType.ValueRead, "identifier", ChildrenNodes[1]) as AstNodeBase;
            }
            else if (ChildrenNodes.Count == 3)
            {
                this.databaseName = AddChild(NodeUseType.ValueRead, "databaseIdentifier", ChildrenNodes[0]) as AstNodeBase;
                this.schemaName = AddChild(NodeUseType.ValueRead, "schemaIdentifier", ChildrenNodes[1]) as AstNodeBase;
                this.identifier = AddChild(NodeUseType.ValueRead, "identifier", ChildrenNodes[2]) as AstNodeBase;
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
            PlanNode identifierAux = (PlanNode)this.identifier.Evaluate(thread);
            identifierAux.Properties.Add("SchemaIdentifier", null);
            identifierAux.Properties.Add("DatabaseIdentifier", null);
            if (this.schemaName != null)
            {
                PlanNode schemaNameAux = (PlanNode)this.schemaName.Evaluate(thread);
                identifierAux.Properties["SchemaIdentifier"] = schemaNameAux.Properties["Value"];
            }

            if (this.databaseName != null)
            {
                PlanNode databaseNameAux = (PlanNode)this.databaseName.Evaluate(thread);
                identifierAux.Properties["DatabaseIdentifier"] = databaseNameAux.Properties["Value"];
            }

            this.EndEvaluate(thread);

            return identifierAux;
        }
    }
}
