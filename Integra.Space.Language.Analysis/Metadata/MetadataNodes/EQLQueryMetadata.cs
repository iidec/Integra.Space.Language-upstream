using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.MetadataNodes
{
    class EQLQueryMetadata
    {
        public string From { get; }

        public EQLSelectMetadata Select { get; }

        public EQLGroupByMetadata GroupBy { get; }

        public EQLOrderByMetadata OrderBy { get; }
    }
}
