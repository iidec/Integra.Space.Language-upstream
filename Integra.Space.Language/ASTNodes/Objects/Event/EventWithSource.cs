//-----------------------------------------------------------------------
// <copyright file="EventWithSource.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects.Event
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// EventWithSource class
    /// </summary>
    internal sealed class EventWithSource : AstNodeBase
    {
        /// <summary>
        /// prefix node, name of the object
        /// </summary>
        private AstNodeBase idObject;

        /// <summary>
        /// reserved word "."
        /// </summary>
        private string point;

        /// <summary>
        /// object node
        /// </summary>
        private AstNodeBase eventObject;

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
            this.idObject = AddChild(NodeUseType.Parameter, SR.EventRole, ChildrenNodes[0]) as AstNodeBase;
            this.point = ".";
            this.eventObject = AddChild(NodeUseType.Parameter, SR.EventRole, ChildrenNodes[1]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[1].Token.Location.Column;
            this.result.Line = ChildrenNodes[1].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.ObjectWithPrefix;
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
            PlanNode idObjectAux = (PlanNode)this.idObject.Evaluate(thread);
            PlanNode eventObjectAux = (PlanNode)this.eventObject.Evaluate(thread);
            this.EndEvaluate(thread);

            // se cambia el tipo de nodo del primer hijo
            idObjectAux.NodeType = PlanNodeTypeEnum.ObjectPrefix;

            this.result.NodeText = idObjectAux.NodeText + this.point + eventObjectAux.NodeText;

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(idObjectAux);
            this.result.Children.Add(eventObjectAux);

            return this.result;
        }
    }
}
