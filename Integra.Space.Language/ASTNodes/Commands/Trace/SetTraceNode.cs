//-----------------------------------------------------------------------
// <copyright file="SetTraceNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.Trace
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// SetTraceNode class
    /// </summary>
    internal sealed class SetTraceNode : AstNodeBase
    {
        /// <summary>
        /// reserved word 'set'
        /// </summary>
        private string setWord;

        /// <summary>
        /// reserved word 'trace'
        /// </summary>
        private string traceWord;

        /// <summary>
        /// reserved word 'level'
        /// </summary>
        private string levelWord;

        /// <summary>
        /// level to trace
        /// </summary>
        private AstNodeBase level;

        /// <summary>
        /// reserved word 'to'
        /// </summary>
        private string toWord;

        /// <summary>
        /// object to trace
        /// </summary>
        private AstNodeBase objectToTrace;

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
            this.setWord = (string)ChildrenNodes[0].Token.Value;
            this.traceWord = (string)ChildrenNodes[1].Token.Value;
            this.levelWord = (string)ChildrenNodes[2].Token.Value;
            this.level = AddChild(NodeUseType.Parameter, SR.ObjectRole, ChildrenNodes[3]) as AstNodeBase;
            this.toWord = (string)ChildrenNodes[4].Token.Value;
            this.objectToTrace = AddChild(NodeUseType.Parameter, SR.IdentifierRole, ChildrenNodes[5]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
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
            PlanNode levelAux = (PlanNode)this.level.Evaluate(thread);
            string objectToTraceAux = (string)this.objectToTrace.Evaluate(thread);
            this.EndEvaluate(thread);

            this.SelectNodeType(objectToTraceAux);
            this.result.Properties.Add("ObjectToTrace", objectToTraceAux);

            this.result.NodeText = this.setWord + " " + this.traceWord + " " + levelAux.NodeText + " " + this.toWord + " " + objectToTraceAux;

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(levelAux);

            return this.result;
        }

        /// <summary>
        /// select the node type of the planNode
        /// </summary>
        /// <param name="objectToSetTrace">user defined object to trace</param>
        private void SelectNodeType(string objectToSetTrace)
        {
            string o = objectToSetTrace.ToLower();

            if (SR.ReservedWordAdapter == o)
            {
                this.result.NodeType = PlanNodeTypeEnum.SetTraceAdapter;
            }
            else if (SR.ReservedWordSource == o)
            {
                this.result.NodeType = PlanNodeTypeEnum.SetTraceSource;
            }
            else if (SR.ReservedWordStream == o)
            {
                this.result.NodeType = PlanNodeTypeEnum.SetTraceStream;
            }
            else if (SR.ReservedWordTrigger == o)
            {
                this.result.NodeType = PlanNodeTypeEnum.SetTraceTrigger;
            }
            else if (SR.ReservedWordEngine == o)
            {
                this.result.NodeType = PlanNodeTypeEnum.SetTraceEngine;
            }
            else
            {
                this.result.NodeType = PlanNodeTypeEnum.SetTraceObject;
            }
        }
    }
}
