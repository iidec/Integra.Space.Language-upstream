//-----------------------------------------------------------------------
// <copyright file="StartStopNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.StartStop
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// StartStopNode class
    /// </summary>
    internal sealed class StartStopNode : AstNodeBase
    {
        /// <summary>
        /// reserved word
        /// </summary>
        private string command;

        /// <summary>
        /// user defined object node
        /// </summary>
        private AstNodeBase userDefinedObject;

        /// <summary>
        /// object name to start or stop
        /// </summary>
        private AstNodeBase objectName;

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
            this.command = (string)ChildrenNodes[0].Token.Value;
            this.userDefinedObject = AddChild(NodeUseType.Parameter, SR.ObjectRole, ChildrenNodes[1]) as AstNodeBase;
            this.objectName = AddChild(NodeUseType.Parameter, SR.IdentifierRole, ChildrenNodes[2]) as AstNodeBase;

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
            string userDefinedObjectAux = (string)this.userDefinedObject.Evaluate(thread);
            PlanNode objectNameAux = (PlanNode)this.objectName.Evaluate(thread);
            this.EndEvaluate(thread);

            this.SelectNodeType(userDefinedObjectAux);

            this.result.Properties.Add(SR.UserDefinedObjectProperty, userDefinedObjectAux.ToLower());

            this.result.NodeText = SR.StartStopNodeText(this.command, userDefinedObjectAux, objectNameAux.NodeText);

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(objectNameAux);

            return this.result;
        }

        /// <summary>
        /// select the node type of the planNode
        /// </summary>
        /// <param name="objectToStartOrStop">user defined object to start or stop</param>
        private void SelectNodeType(string objectToStartOrStop)
        {
            string c = this.command.ToLower();
            string o = objectToStartOrStop;

            if (SR.ReservedWordStart == c)
            {
                if (SR.ReservedWordAdapter == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StartAdapter;
                }
                else if (SR.ReservedWordSource == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StartSource;
                }
                else if (SR.ReservedWordStream == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StartStream;
                }
                else if (SR.ReservedWordTrigger == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StartTrigger;
                }
            }
            else if (SR.ReservedWordStop == c)
            {
                if (SR.ReservedWordAdapter == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StopAdapter;
                }
                else if (SR.ReservedWordSource == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StopSource;
                }
                else if (SR.ReservedWordStream == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StopStream;
                }
                else if (SR.ReservedWordTrigger == o)
                {
                    this.result.NodeType = PlanNodeTypeEnum.StopTrigger;
                }
            }
        }
    }
}
