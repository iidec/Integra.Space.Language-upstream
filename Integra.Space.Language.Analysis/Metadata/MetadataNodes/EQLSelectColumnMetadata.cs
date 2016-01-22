using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.MetadataNodes
{
    class EQLSelectColumnMetadata
    {
        public string Alias { get; }
        public EQLAgregationFunction AgregationFunction { get; } //Podria indicar si la columna es una función de agregación
    }
}
