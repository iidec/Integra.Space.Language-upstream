using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.LanguageUnitTests.TypeBuilder
{
    public class EventResultABC : EventResult
    {
        private string propX;

        public EventResultABC()
        {
        }

        public string PropX
        {
            get
            {
                return this.propX;
            }

            set
            {
                this.propX = value;
            }
        }

        public override void Serialize(IQueryResultWriter writer)
        {
            base.Serialize(writer);
        }
    }
}
