//-----------------------------------------------------------------------
// <copyright file="SpaceMetadataTreeNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Metadata
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Space metadata tree node
    /// </summary>
    public class SpaceMetadataTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceMetadataTreeNode"/> class.
        /// </summary>
        /// <param name="type">Space metadata three node type.</param>
        public SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Gets the node type.
        /// </summary>
        public SpaceMetadataTreeNodeTypeEnum Type { get; private set; }

        /// <summary>
        /// Gets or sets the child nodes.
        /// </summary>
        public List<SpaceMetadataTreeNode> ChildNodes { get; set; }

        /// <summary>
        /// Gets or sets the value of the nodes.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the data type of the value node.
        /// </summary>
        public Type ValueDataType { get; set; }
    }
}
