using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.Metadata.MetadataNodes
{
    public class SpaceMetadataTreeNode
    {
        public SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum type)
        {
            this.Type = type;
        }

        public MetadataTreeNodeTypeEnum Type { get; private set; }

        public List<SpaceMetadataTreeNode> ChildNodes { get; set; }

        public object Value { get; set; }

        public Type ValueDataType { get; set; }
    }
}
