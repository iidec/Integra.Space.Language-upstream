using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.LanguageUnitTests.TypeBuilder
{
    public class QueryResultABC : QueryResult<EventResultABC>
    {
        public QueryResultABC() : base("", DateTime.Now, new EventResultABC[] { })
        {

        }

        public override void Serialize(IQueryResultWriter writer)
        {
            base.Serialize(writer);
        }
    }
}
