using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.MetadataNodes
{
    class EQLOrderByMetadata
    {
        public bool IsDescendent { get; }

        public EQLOrderByColumnMetadata[] Columns { get; }
    }
}
