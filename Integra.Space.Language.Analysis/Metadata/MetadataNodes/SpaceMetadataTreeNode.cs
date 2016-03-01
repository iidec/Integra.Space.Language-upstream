//-----------------------------------------------------------------------
// <copyright file="SpaceMetadataTreeNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Analysis.Metadata.MetadataNodes
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Space metadata tree node
    /// </summary>
    public class SpaceMetadataTreeNode
    {
        public SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum type)
        {
            this.Type = type;
        }

        public SpaceMetadataTreeNodeTypeEnum Type { get; private set; }

        public List<SpaceMetadataTreeNode> ChildNodes { get; set; }

        public object Value { get; set; }

        public Type ValueDataType { get; set; }
    }
}
