﻿//-----------------------------------------------------------------------
// <copyright file="EventPropertiesNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects.Event
{
    using System;
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;
    using Messaging;

    /// <summary>
    /// EventPropertiesNode class
    /// </summary>
    internal sealed class EventPropertiesNode : AstNodeBase
    {
        /// <summary>
        /// event node
        /// </summary>
        private AstNodeBase ev;

        /// <summary>
        /// property of the event
        /// </summary>
        private string property;

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

            this.ev = AddChild(NodeUseType.Parameter, SR.EventRole, ChildrenNodes[0]) as AstNodeBase;
            this.property = ChildrenNodes[1].Token.Text;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[1].Token.Location.Column;
            this.result.Line = ChildrenNodes[1].Token.Location.Line;
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
            PlanNode auxEvent = (PlanNode)this.ev.Evaluate(thread);
            this.EndEvaluate(thread);

            if (auxEvent.Properties.ContainsKey("PropertyName"))
            {
                string propertyName = string.Format("{0}_{1}", auxEvent.Properties["PropertyName"], ChildrenNodes[1].Token.Value.ToString());
                this.result.Properties.Add("PropertyName", propertyName);
            }
            else
            {
                this.result.Properties.Add("PropertyName", "_" + ChildrenNodes[1].Token.Value.ToString());
            }

            this.result.NodeType = PlanNodeTypeEnum.EventProperty;
            this.result.Properties.Add("IncidenciasEnOn", 0);
            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(auxEvent);

            if (this.property.ToLower() == SR.SystemTimestamp)
            {
                this.result.Properties.Add(SR.PrpoertyProperty, EventPropertiesEnum.SystemTimestamp.ToString());
                this.result.Properties.Add(SR.DataTypeProperty, typeof(DateTime));
                this.result.NodeText = auxEvent.NodeText + "." + this.property;
            }
            else if (this.property.ToLower() == SR.SourceTimestamp)
            {
                this.result.Properties.Add(SR.PrpoertyProperty, EventPropertiesEnum.SourceTimestamp.ToString());
                this.result.Properties.Add(SR.DataTypeProperty, typeof(DateTime));
                this.result.NodeText = auxEvent.NodeText + "." + this.property;
            }
            else if (this.property.ToLower() == SR.NameProperty)
            {
                this.result.Properties.Add(SR.PrpoertyProperty, EventPropertiesEnum.Name.ToString());
                this.result.Properties.Add(SR.DataTypeProperty, typeof(string));
                this.result.NodeText = auxEvent.NodeText + "." + this.property;
            }
            else if (this.property.ToLower() == SR.AdapterProperty)
            {
                this.result.Properties.Add(SR.PrpoertyProperty, EventPropertiesEnum.Adapter.ToString());
                this.result.Properties.Add(SR.DataTypeProperty, typeof(EventAdapter));
                this.result.NodeText = auxEvent.NodeText + "." + this.property;
            }
            else if (this.property.ToLower() == SR.AgentProperty)
            {
                this.result.Properties.Add(SR.PrpoertyProperty, EventPropertiesEnum.Agent.ToString());
                this.result.Properties.Add(SR.DataTypeProperty, typeof(EventAgent));
                this.result.NodeText = auxEvent.NodeText + "." + this.property;
            }
            else if (this.property.ToLower() == SR.MessageProperty)
            {
                this.result.Properties.Add(SR.PrpoertyProperty, EventPropertiesEnum.Message.ToString());
                this.result.Properties.Add(SR.DataTypeProperty, typeof(Message));
                this.result.NodeText = auxEvent.NodeText + "." + this.property;
                this.result.NodeType = PlanNodeTypeEnum.ObjectMessage;
            }
            else
            {
                this.result.Properties.Add(SR.PrpoertyProperty, this.property);
                this.result.Properties.Add(SR.DataTypeProperty, typeof(string));
                this.result.NodeText = auxEvent.NodeText + "." + this.property;
            }

            return this.result;
        }
    }
}