//-----------------------------------------------------------------------
// <copyright file="IntoASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using System;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Into AST node class.
    /// </summary>
    internal sealed class IntoASTNode : AstNodeBase
    {
        /// <summary>
        /// Reserved word 'into'.
        /// </summary>
        private string intoWord;

        /// <summary>
        /// Identifier path.
        /// </summary>
        private AstNodeBase identifier;

        /// <summary>
        /// First method called.
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            if (this.ChildrenNodes != null)
            {
                this.intoWord = ChildrenNodes[0].Token.ValueString;
                this.identifier = AddChild(NodeUseType.ValueRead, "sourceIdentifier", ChildrenNodes[1]) as AstNodeBase;
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
            if (this.ChildrenNodes != null)
            {
                this.BeginEvaluate(thread);
                PlanNode sourceIdentifier = (PlanNode)this.identifier.Evaluate(thread);
                this.EndEvaluate(thread);

                string databaseIdentifier = null;
                if (sourceIdentifier.Properties.ContainsKey("DatabaseIdentifier") && sourceIdentifier.Properties["DatabaseIdentifier"] != null)
                {
                    databaseIdentifier = sourceIdentifier.Properties["DatabaseIdentifier"].ToString();
                }

                string schemaIdentifier = null;
                if (sourceIdentifier.Properties.ContainsKey("SchemaIdentifier") && sourceIdentifier.Properties["SchemaIdentifier"] != null)
                {
                    schemaIdentifier = sourceIdentifier.Properties["SchemaIdentifier"].ToString();
                }

                string systemObjectIdentifier = null;
                if (sourceIdentifier.Properties.ContainsKey("Value") && sourceIdentifier.Properties["Value"] != null)
                {
                    systemObjectIdentifier = sourceIdentifier.Properties["Value"].ToString();
                }

                return Tuple.Create<string, string, string>(databaseIdentifier, schemaIdentifier, systemObjectIdentifier);
            }
            else
            {
                return null;
            }
        }
    }
}
