//-----------------------------------------------------------------------
// <copyright file="EventNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects.Event
{
    using Integra.Space.Language.ASTNodes.Base;

    using Integra.Space.Language.General;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// EventNode class
    /// </summary>
    internal sealed class EventNode : AstNodeBase
    {
        /// <summary>
        /// symbol @
        /// </summary>
        private string symbol;

        /// <summary>
        /// reserved word 'event'
        /// </summary>
        private string eventWord;

        /// <summary>
        /// sign dot '.'
        /// </summary>
        private string dot;

        /// <summary>
        /// source id
        /// </summary>
        private AstNodeBase sourceId;

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
            this.result = new PlanNode();

            int count = ChildrenNodes.Count;
            if (count == 2)
            {
                this.symbol = (string)ChildrenNodes[0].Token.Value;
                this.eventWord = (string)ChildrenNodes[1].Token.Value;
                this.result.NodeText = string.Format("{0}{1}", this.symbol, this.eventWord);
            }
            else if (count == 4)
            {
                this.sourceId = AddChild(NodeUseType.Parameter, "sourceId", ChildrenNodes[3]) as AstNodeBase;
                this.dot = (string)ChildrenNodes[1].Token.Value;
                this.symbol = (string)ChildrenNodes[2].Token.Value;
                this.eventWord = (string)ChildrenNodes[3].Token.Value;
                this.result.NodeText = string.Format("{0}{1}{2}", this.dot, this.symbol, this.eventWord);
            }

            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.Event;
            this.result.Properties.Add("Value", this.eventWord);
            this.result.Properties.Add("DataType", typeof(string).ToString());
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            PlanNode sourceIdAux = null;

            int count = ChildrenNodes.Count;

            if (count == 2)
            {
                sourceIdAux = new PlanNode();
                sourceIdAux.NodeText = string.Empty;
                sourceIdAux.NodeType = PlanNodeTypeEnum.Identifier;
                sourceIdAux.Properties.Add("Value", string.Empty);
                sourceIdAux.Properties.Add("DataType", typeof(object).ToString());
            }
            else if (count == 4)
            {
                this.BeginEvaluate(thread);
                sourceIdAux = (PlanNode)this.sourceId.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText += string.Format("{0}{1}", sourceIdAux.NodeText, this.result.NodeText);
            }

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(sourceIdAux);

            return this.result;
        }
    }
}
