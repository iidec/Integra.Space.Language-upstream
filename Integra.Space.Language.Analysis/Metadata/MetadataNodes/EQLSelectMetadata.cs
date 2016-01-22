using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.MetadataNodes
{
    class EQLSelectMetadata
    {
        public int Top { get; }

        public EQLSelectColumnMetadata[] Columns { get; }
    }
}
