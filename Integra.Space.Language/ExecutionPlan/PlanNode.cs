//-----------------------------------------------------------------------
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
    internal sealed class PlanNode
    {
        /// <summary>
        /// Doc go here
        /// </summary>
        private Dictionary<string, object> properties;

        /// <summary>
        /// Gets or sets the plan node type
        /// </summary>
        public PlanNodeTypeEnum NodeType { get; set; }

        /// <summary>
        /// Gets or sets the line of the evaluated sentence
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the evaluated sentence column
        /// </summary>
        public int Column { get; set; }

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
    }
}