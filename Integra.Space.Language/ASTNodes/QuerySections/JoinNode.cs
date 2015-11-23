//-----------------------------------------------------------------------
// <copyright file="JoinNode.cs" company="Integra.Space.Language">
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
    /// JoinNode class
    /// </summary>
    internal sealed class JoinNode : AstNodeBase
    {
        /// <summary>
        /// identifier node of the source node
        /// </summary>
        private AstNodeBase joinSource;

        /// <summary>
        /// alias of the source
        /// </summary>
        private AstNodeBase joinAlias;

        /// <summary>
        /// reserved word 'join'
        /// </summary>
        private string join;

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
            this.join = (string)ChildrenNodes[0].Token.Value;
            this.joinSource = AddChild(NodeUseType.Parameter, "joinId", ChildrenNodes[1]) as AstNodeBase;
            this.reservedAs = (string)ChildrenNodes[2].Token.Value;
            this.joinAlias = AddChild(NodeUseType.Parameter, "joinAlias", ChildrenNodes[3]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.Join;
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
            PlanNode joinSourceAux = (PlanNode)this.joinSource.Evaluate(thread);
            PlanNode joinAliasAux = (PlanNode)this.joinAlias.Evaluate(thread);
            this.EndEvaluate(thread);

            joinSourceAux.Properties["DataType"] = typeof(string).ToString();
            joinAliasAux.Properties["DataType"] = typeof(string).ToString();

            this.result.NodeText = this.join + " " + joinSourceAux.NodeText + " " + this.reservedAs + " " + joinAliasAux.NodeText;

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(joinSourceAux);
            this.result.Children.Add(joinAliasAux);
            
            return this.result;
        }
    }
}
