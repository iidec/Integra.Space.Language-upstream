//-----------------------------------------------------------------------
// <copyright file="AddSourceNode.cs" company="ARITEC">
//     Integra Vision. All rights reserved.
// </copyright>
// <author>Oscar Canek</author>
//-----------------------------------------------------------------------

namespace Integra.Space.Language.ASTNodes.Commands.Create
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// CreateSourceNode
    /// Doc go here
    /// </summary>
    internal sealed class AddSourceNode : AstNodeBase
    {
        /// <summary>
        /// reserved word 'add'
        /// </summary>
        private string terminalAdd;

        /// <summary>
        /// reserved word 'source'
        /// </summary>
        private string sourceWord;

        /// <summary>
        /// name of the source
        /// </summary>
        private AstNodeBase idSource;
                
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
            this.terminalAdd = (string)ChildrenNodes[0].Token.Value;
            this.sourceWord = (string)ChildrenNodes[1].Token.Value;
            this.idSource = AddChild(NodeUseType.Parameter, SR.UserDefinedObjectRole, ChildrenNodes[2]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[1].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.CreateSource;
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
            PlanNode idSourceAux = (PlanNode)this.idSource.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.NodeText = SR.SourceNodeText(this.terminalAdd, this.sourceWord, idSourceAux.NodeText);

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(idSourceAux);

            return this.result;
        }
    }
}
