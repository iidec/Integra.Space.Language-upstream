﻿//-----------------------------------------------------------------------
// <copyright file="PlanNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// PlanNode class 
    /// Execution plan tree node
    /// </summary>
    [Serializable]
    internal sealed class PlanNode : ISpaceASTNode
    {
        /// <summary>
        /// Doc go here
        /// </summary>
        private Dictionary<string, object> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanNode"/> class.
        /// </summary>
        /// <param name="line">Node line.</param>
        /// <param name="column">Node column.</param>
        /// <param name="nodeType">Node type.</param>
        public PlanNode(int line, int column, PlanNodeTypeEnum nodeType)
        {
            this.Line = line;
            this.Column = column;
            this.NodeType = nodeType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanNode"/> class.
        /// </summary>
        /// <param name="line">Node line.</param>
        /// <param name="column">Node column.</param>
        /// <param name="nodeType">Node type.</param>
        /// <param name="nodeText">Node text.</param>
        public PlanNode(int line, int column, PlanNodeTypeEnum nodeType, string nodeText)
        {
            this.Line = line;
            this.Column = column;
            this.NodeText = nodeText;
            this.NodeType = nodeType;
        }

        /// <summary>
        /// Gets or sets the plan node type
        /// </summary>
        public PlanNodeTypeEnum NodeType { get; set; }

        /// <summary>
        /// Gets the line of the evaluated sentence
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// Gets the evaluated sentence column
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Gets or sets the text of the actual node
        /// </summary>
        public string NodeText { get; set; }

        /// <summary>
        /// Gets the actual node properties
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = new Dictionary<string, object>();
                }

                return this.properties;
            }
        }

        /// <summary>
        /// Gets or sets the actual node Children
        /// </summary>
        public List<PlanNode> Children { get; set; }

        /// <summary>
        /// Clone the actual node with his children.
        /// </summary>
        /// <returns>The node cloned.</returns>
        public PlanNode Clone()
        {
            PlanNode planCloned = new PlanNode(this.Line, this.Column, this.NodeType);
            planCloned.NodeText = this.NodeText;

            foreach (KeyValuePair<string, object> kvp in this.Properties)
            {
                planCloned.Properties.Add(kvp.Key, kvp.Value);
            }

            if (this.Children != null)
            {
                planCloned.Children = new List<PlanNode>();
                foreach (PlanNode child in this.Children)
                {
                    PlanNode aux = child.Clone();
                    planCloned.Children.Add(aux);
                }
            }

            return planCloned;
        }
    }
}