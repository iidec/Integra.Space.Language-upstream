//-----------------------------------------------------------------------
// <copyright file="ReceiveNode.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.Receive
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Receive command node.
    /// </summary>
    internal sealed class ReceiveNode : AstNodeBase
    {
        /// <summary>
        /// Reserved word 'receive'
        /// </summary>
        private string receiveWord;

        /// <summary>
        /// Reserved word 'from'
        /// </summary>
        private string fromWord;

        /// <summary>
        /// Source or stream name
        /// </summary>
        private AstNodeBase sourceOrStreamName;

        /// <summary>
        /// result plan node
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

            this.receiveWord = (string)ChildrenNodes[0].Token.Value;
            this.fromWord = (string)ChildrenNodes[1].Token.Value;
            this.sourceOrStreamName = AddChild(NodeUseType.Parameter, SR.UserDefinedObjectRole, ChildrenNodes[2]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.Receive;
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
            PlanNode auxSourceName = (PlanNode)this.sourceOrStreamName.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.NodeText = string.Format("{0} {1} {2}", this.receiveWord, this.fromWord, auxSourceName.NodeText);

            this.result.Properties.Add("SourceName", auxSourceName.Properties["Value"].ToString());

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(auxSourceName);

            return this.result;
        }
    }
}
