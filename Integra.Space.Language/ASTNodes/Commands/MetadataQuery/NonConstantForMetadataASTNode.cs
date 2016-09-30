//-----------------------------------------------------------------------
// <copyright file="NonConstantForMetadataASTNode.cs" company="Integra.Space.Language">
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

    /// <summary>
    /// IdentifierNode class
    /// </summary>
    internal sealed class NonConstantForMetadataASTNode : AstNodeBase
    {
        /// <summary>
        /// Identifier AST node.
        /// </summary>
        private AstNodeBase identifier;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.identifier = AddChild(NodeUseType.Parameter, "Identifier", ChildrenNodes[0]) as AstNodeBase;
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
            PlanNode identifierNode = (PlanNode)this.identifier.Evaluate(thread);
            this.EndEvaluate(thread);

            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode property = new PlanNode();
            property.NodeType = PlanNodeTypeEnum.Property;
            property.NodeText = identifierNode.Properties["Value"].ToString();
            property.Properties.Add("Property", identifierNode.Properties["Value"]);
            property.Properties.Add("IsConstant", false);
            property.Children = new List<PlanNode>();
            property.Children.Add(fromForLambda);

            return property;
        }
    }
}
