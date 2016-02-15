using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.LanguageUnitTests.TypeBuilder
{
    public class QueryResultWriter : IQueryResultWriter
    {
        private string jsonResult;
        private bool firstInQuery;
        private bool firstInRow;
        private bool inRowScope;
        private bool hasData;
        private bool firstRow;

        public QueryResultWriter()
        {
            this.jsonResult = string.Empty;
            this.firstInQuery = true;
            this.firstInRow = true;
            this.inRowScope = false;
            this.hasData = false;
            this.firstRow = true;
        }

        public string JsonResult
        {
            get
            {
                return this.jsonResult;
            }
        }

        public void WriteEndQueryResult()
        {
            if (this.hasData)
            {
                this.jsonResult = string.Concat(this.jsonResult, "]");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, "]]");
            }

            this.jsonResult = string.Concat(this.jsonResult, "}");
        }

        public void WriteEndQueryResultRow()
        {
            this.jsonResult = string.Concat(this.jsonResult, "]");
            this.firstInRow = true;
            this.inRowScope = false;
        }

        public void WriteStartQueryResult()
        {
            this.jsonResult = string.Concat(this.jsonResult, "{ \"r\" : [[");
        }

        public void WriteStartQueryResultRow()
        {
            if (this.firstRow)
            {
                this.jsonResult = string.Concat(this.jsonResult, "]");
                this.firstRow = false;
            }

            this.WriteComa();
            this.jsonResult = string.Concat(this.jsonResult, "[");
            this.hasData = true;
            this.inRowScope = true;
        }

        public void WriteValue(Guid value)
        {
            this.jsonResult = string.Concat(this.jsonResult, value.ToString());
        }

        public void WriteValue(bool? value)
        {
            this.WriteComa();
            if (value == true)
            {
                this.jsonResult = string.Concat(this.jsonResult, "true");
            }
            else if (value == false)
            {
                this.jsonResult = string.Concat(this.jsonResult, "false");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
        }

        public void WriteValue(sbyte? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(short? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(int? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(long? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(float? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(decimal? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(string value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, "\"" + value + "\"");
            }
        }

        public void WriteValue(DateTime value)
        {
            this.WriteComa();
            this.jsonResult = string.Concat(this.jsonResult, "\"", value.ToString("yyyy-MM-dd hh:mm:ss"), "\"");
        }

        public void WriteValue(double? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(ulong? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(uint? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(ushort? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(byte? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(char? value)
        {
            this.WriteComa();
            if (value == null)
            {
                this.jsonResult = string.Concat(this.jsonResult, "null");
            }
            else
            {
                this.jsonResult = string.Concat(this.jsonResult, value);
            }
        }

        public void WriteValue(char[] value)
        {
            this.WriteComa();
            string r = string.Empty;

            foreach (char c in value)
            {
                r += c;
            }

            this.jsonResult = string.Concat(this.jsonResult, r);
        }

        public void WriteValue(TimeSpan value)
        {
            this.jsonResult = string.Concat(this.jsonResult, value.ToString());
        }

        public void WriteValue(byte[] value)
        {
            throw new NotImplementedException();
        }

        public void WriteComa()
        {
            if (this.inRowScope)
            {
                if (!this.firstInRow)
                {
                    this.jsonResult += ",";
                }
                else
                {
                    this.firstInRow = false;
                }
            }
            else
            {
                if (!this.firstInQuery)
                {
                    this.jsonResult += ",";
                }
                else
                {
                    this.firstInQuery = false;
                }
            }
        }

        public void WriteValue(object value)
        {
            this.jsonResult += value.ToString();
        }
    }
}
