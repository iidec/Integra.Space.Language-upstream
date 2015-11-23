//-----------------------------------------------------------------------
// <copyright file="CreateStreamNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
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
    /// CreateStreamNode
    /// Doc go here ¬¬
    /// </summary>
    internal sealed class CreateStreamNode : AstNodeBase
    {
        /// <summary>
        /// reserved word
        /// </summary>
        private string createOrAlterWord;

        /// <summary>
        /// reserved word
        /// </summary>
        private string streamWord;

        /// <summary>
        /// stream id
        /// </summary>
        private AstNodeBase streamId;

        /// <summary>
        /// reserved word
        /// </summary>
        private string asWord;

        /// <summary>
        /// from node
        /// </summary>
        private AstNodeBase from;

        /// <summary>
        /// join node
        /// </summary>
        private AstNodeBase join;

        /// <summary>
        /// with node
        /// </summary>
        private AstNodeBase with;

        /// <summary>
        /// on node
        /// </summary>
        private AstNodeBase on;

        /// <summary>
        /// apply window node
        /// </summary>
        private AstNodeBase applyWindow;

        /// <summary>
        /// where node
        /// </summary>
        private AstNodeBase where;

        /// <summary>
        /// select node
        /// </summary>
        private AstNodeBase select;

        /// <summary>
        /// result planNode
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

            this.createOrAlterWord = (string)ChildrenNodes[0].Token.Value;
            this.streamWord = (string)ChildrenNodes[1].Token.Value;
            this.streamId = AddChild(NodeUseType.Parameter, "streamId", ChildrenNodes[2]) as AstNodeBase;
            this.asWord = (string)ChildrenNodes[3].Token.Value;

            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 7)
            {
                this.from = AddChild(NodeUseType.Parameter, SR.FromRole, ChildrenNodes[4]) as AstNodeBase;
                this.where = AddChild(NodeUseType.Parameter, SR.WhereRole, ChildrenNodes[5]) as AstNodeBase;
                this.select = AddChild(NodeUseType.Parameter, SR.SelectRole, ChildrenNodes[6]) as AstNodeBase;
            }
            else if (childrenCount == 10)
            {
                this.join = AddChild(NodeUseType.Parameter, SR.JoinRole, ChildrenNodes[4]) as AstNodeBase;
                this.with = AddChild(NodeUseType.Parameter, SR.WithRole, ChildrenNodes[5]) as AstNodeBase;
                this.on = AddChild(NodeUseType.Parameter, SR.OnRole, ChildrenNodes[6]) as AstNodeBase;
                this.applyWindow = AddChild(NodeUseType.Parameter, SR.ApplyWindowRole, ChildrenNodes[7]) as AstNodeBase;
                this.where = AddChild(NodeUseType.Parameter, SR.WhereRole, ChildrenNodes[8]) as AstNodeBase;
                this.select = AddChild(NodeUseType.Parameter, SR.SelectRole, ChildrenNodes[9]) as AstNodeBase;
            }

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;

            if (this.createOrAlterWord.Equals("create", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.CreateStream;
            }
            else if (this.createOrAlterWord.Equals("alter", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.AlterStream;
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

            PlanNode streamIdAux = (PlanNode)this.streamId.Evaluate(thread);

            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 7)
            {
                PlanNode fromAux = (PlanNode)this.from.Evaluate(thread);
                PlanNode whereAux = (PlanNode)this.where.Evaluate(thread);
                PlanNode selectAux = (PlanNode)this.select.Evaluate(thread);

                this.result.NodeText = SR.StreamNodeText1(this.createOrAlterWord, this.streamWord, streamIdAux.NodeText, this.asWord, fromAux.NodeText, whereAux.NodeText, selectAux.NodeText);

                this.result.Children = new System.Collections.Generic.List<PlanNode>();
                this.result.Children.Add(streamIdAux);
                this.result.Children.Add(fromAux);
                this.result.Children.Add(whereAux);
                this.result.Children.Add(selectAux);
            }
            else if (childrenCount == 10)
            {
                PlanNode joinAux = (PlanNode)this.join.Evaluate(thread);
                PlanNode withAux = (PlanNode)this.with.Evaluate(thread);
                PlanNode onAux = (PlanNode)this.on.Evaluate(thread);
                PlanNode applyWindowAux = (PlanNode)this.applyWindow.Evaluate(thread);
                PlanNode whereAux = (PlanNode)this.where.Evaluate(thread);
                PlanNode selectAux = (PlanNode)this.select.Evaluate(thread);

                this.result.NodeText = SR.StreamNodeText2(this.createOrAlterWord, this.streamWord, streamIdAux.NodeText, this.asWord, joinAux.NodeText, withAux.NodeText, onAux.NodeText, applyWindowAux.NodeText, whereAux.NodeText, selectAux.NodeText);

                this.result.Children = new System.Collections.Generic.List<PlanNode>();
                this.result.Children.Add(streamIdAux);
                this.result.Children.Add(joinAux);
                this.result.Children.Add(withAux);
                this.result.Children.Add(onAux);
                this.result.Children.Add(applyWindowAux);
                this.result.Children.Add(whereAux);
                this.result.Children.Add(selectAux);
            }

            this.EndEvaluate(thread);

            return this.result;
        }
    }
}
