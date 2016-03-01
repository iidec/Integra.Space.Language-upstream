//-----------------------------------------------------------------------
// <copyright file="ObjectNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects.Object
{
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;
    using Messaging;

    /// <summary>
    /// ObjectNode class
    /// </summary>
    internal sealed class ObjectNode : AstNodeBase
    {
        /// <summary>
        /// Event node
        /// </summary>
        private AstNodeBase evento;

        /// <summary>
        /// Object node
        /// </summary>
        private AstNodeBase objeto;

        /// <summary>
        /// Id node
        /// </summary>
        private AstNodeBase id;

        /// <summary>
        /// Message word
        /// </summary>
        private string message;

        /// <summary>
        /// Result plan
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
            int childrenCount = ChildrenNodes.Count();
            if (childrenCount == 4)
            {
                this.evento = AddChild(NodeUseType.Parameter, SR.EventRole, ChildrenNodes[0]) as AstNodeBase;
                this.message = (string)ChildrenNodes[1].Token.Text;
                this.objeto = AddChild(NodeUseType.Keyword, SR.ObjectRole, ChildrenNodes[2]) as AstNodeBase;
                this.id = AddChild(NodeUseType.Keyword, SR.IdentifierRole, ChildrenNodes[3]) as AstNodeBase;
            }
            else if (childrenCount == 2)
            {
                this.objeto = AddChild(NodeUseType.Keyword, SR.ObjectRole, ChildrenNodes[0]) as AstNodeBase;
                this.id = AddChild(NodeUseType.Keyword, SR.IdentifierRole, ChildrenNodes[1]) as AstNodeBase;
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
            int childrenCount = ChildrenNodes.Count();

            if (childrenCount == 4)
            {
                this.BeginEvaluate(thread);
                PlanNode eventObject = (PlanNode)this.evento.Evaluate(thread);
                PlanNode idPartOrFieldObject = (PlanNode)this.objeto.Evaluate(thread);
                PlanNode idFieldObject = (PlanNode)this.id.Evaluate(thread);
                this.EndEvaluate(thread);

                PlanNode auxMessage = new PlanNode();
                auxMessage.Column = ChildrenNodes[1].Token.Location.Column;
                auxMessage.Properties.Add(SR.DataTypeProperty, typeof(Message));
                auxMessage.Line = ChildrenNodes[1].Token.Location.Line;
                auxMessage.NodeText = string.Format("{0}.{1}", eventObject.NodeText, this.message);
                auxMessage.NodeType = PlanNodeTypeEnum.ObjectMessage;
                auxMessage.Children = new List<PlanNode>();
                auxMessage.Children.Add(eventObject);

                PlanNode auxPart = new PlanNode();
                auxPart.Column = idPartOrFieldObject.Column;
                auxPart.Properties.Add(SR.DataTypeProperty, typeof(MessagePart));
                auxPart.Line = idPartOrFieldObject.Line;
                auxPart.NodeText = auxMessage.NodeText + "." + idPartOrFieldObject.NodeText;
                auxPart.NodeType = PlanNodeTypeEnum.ObjectPart;
                auxPart.Children = new List<PlanNode>();
                auxPart.Children.Add(auxMessage);
                auxPart.Children.Add(idPartOrFieldObject);

                PlanNode auxField = new PlanNode();
                auxField.Column = idFieldObject.Column;
                auxField.Properties.Add(SR.DataTypeProperty, typeof(MessageField));
                auxField.Line = idFieldObject.Line;
                auxField.NodeText = auxPart.NodeText + "." + idFieldObject.NodeText;
                auxField.NodeType = PlanNodeTypeEnum.ObjectField;

                auxField.Children = new List<PlanNode>();
                auxField.Children.Add(auxPart);
                auxField.Children.Add(idFieldObject);

                this.result = auxField;
            }
            else if (childrenCount == 2)
            {
                this.BeginEvaluate(thread);
                PlanNode idPartOrFieldObject = (PlanNode)this.objeto.Evaluate(thread);
                PlanNode idFieldObject = (PlanNode)this.id.Evaluate(thread);
                this.EndEvaluate(thread);

                PlanNode auxField = new PlanNode();
                auxField.Column = idFieldObject.Column;
                auxField.Properties.Add(SR.DataTypeProperty, typeof(MessageField));
                auxField.Line = idFieldObject.Line;
                auxField.NodeText = idPartOrFieldObject.NodeText + "." + idFieldObject.NodeText;
                auxField.NodeType = PlanNodeTypeEnum.ObjectField;
                auxField.Children = new List<PlanNode>();
                auxField.Children.Add(idPartOrFieldObject);
                auxField.Children.Add(idFieldObject);

                this.result = auxField;
            }

            return this.result;
        }
    }
}
