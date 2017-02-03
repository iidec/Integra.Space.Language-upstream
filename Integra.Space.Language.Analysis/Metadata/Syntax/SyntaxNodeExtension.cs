using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.Metadata.Syntax
{
    public static class SyntaxNodeExtension
    {
        public static void Dump(this SyntaxNode value)
        {
            Dump(value, 0);
        }

        private static void Dump(SyntaxNode node, int level)
        {
            if (node.Parent != null && false)
                System.Console.WriteLine(string.Format("{0}+{1} ({2})", new string('-', level), node.Text, node.Parent.Text));
            else
                System.Console.WriteLine(string.Format("{0}+{1}", new string('-', level), node.Text));

            foreach (SyntaxNode children in node.Childrens)
            {
                Dump(children, level + 1);
            }
        }
    }
}
