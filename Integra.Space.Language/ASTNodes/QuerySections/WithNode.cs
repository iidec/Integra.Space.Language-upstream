//-----------------------------------------------------------------------
// <copyright file="WithNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// WithNode class
    /// </summary>
    internal sealed class WithNode : AstNodeBase
    {
        /// <summary>
        /// identifier node of the source with node
        /// </summary>
        private AstNodeBase withSource;

        /// <summary>
        /// reserved word 'with'
        /// </summary>
        private string with;

        /// <summary>
        /// alias of the source
        /// </summary>
        private AstNodeBase withAlias;

        /// <summary>
        /// reserved word 'as'
        /// </summary>
        private string reservedAs;

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
            this.with = (string)ChildrenNodes[0].Token.Value;
            this.withSource = AddChild(NodeUseType.Parameter, "withId", ChildrenNodes[1]) as AstNodeBase;
            this.reservedAs = (string)ChildrenNodes[2].Token.Value;
            this.withAlias = AddChild(NodeUseType.Parameter, "withAlias", ChildrenNodes[3]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.With;
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
            PlanNode withSourceAux = (PlanNode)this.withSource.Evaluate(thread);
            PlanNode withAliasAux = (PlanNode)this.withAlias.Evaluate(thread);
            this.EndEvaluate(thread);

            withSourceAux.Properties["DataType"] = typeof(string).ToString();
            withAliasAux.Properties["DataType"] = typeof(string).ToString();

            this.result.NodeText = this.with + " " + withSourceAux.NodeText + " " + this.reservedAs + " " + withAliasAux.NodeText;

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(withSourceAux);
            this.result.Children.Add(withAliasAux);

            return this.result;
        }
    }
}
