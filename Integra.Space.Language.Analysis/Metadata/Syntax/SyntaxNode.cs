using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.Metadata.Syntax
{
    public class SyntaxNode
    {
        string _Text;
        List<SyntaxNode> _Childrens;

        public SyntaxNode(string text)
        {
            this._Text = text;
        }

        public SyntaxNode Parent
        {
            get;
            private set;
        }

        public string Text
        {
            get
            {
                return _Text;
            }
        }

        public IEnumerable<SyntaxNode> Childrens
        {
            get
            {
                return GetChildrens().AsEnumerable();
            }
        }

        internal SyntaxNode AddChildren(SyntaxNode node)
        {
            node.Parent = this;
            GetChildrens().Add(node);
            return node;
        }

        private IList<SyntaxNode> GetChildrens()
        {
            if (_Childrens == null)
                _Childrens = new List<SyntaxNode>();

            return _Childrens;
        }
    }
}
