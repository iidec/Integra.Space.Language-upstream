using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integra.Space.Language.Analysis.Metadata.Syntax
{
    public class EQLSyntaxTree
    {
        bool iterateOverAll = false;

        public static SyntaxNode Parse(string text)
        {
            EQLSyntaxTree syntaxTree = new EQLSyntaxTree();
            QueryParser parser = new QueryParser(text);
            PlanNode plan = parser.Evaluate().Payload.Item1;
            SyntaxNode root = new SyntaxNode("Query");
            syntaxTree.Parse(plan, root);
            return root;
        }

        internal void Parse(PlanNode pNode, SyntaxNode parent)
        {
            if (pNode == null)
                return;

            SyntaxNode previousNode = null;

            if (pNode.Children != null)
            {
                foreach (PlanNode pnChild in pNode.Children)
                {
                    if (pnChild.NodeType == PlanNodeTypeEnum.ObservableWhere)
                    {
                        previousNode = parent;
                        var newNode = parent.AddChildren(new SyntaxNode("Where"));
                        parent = newNode;
                    }

                    if (pnChild.NodeType == PlanNodeTypeEnum.Projection)
                    {
                        previousNode = parent;
                        var newNode = GetRoot(parent).AddChildren(new SyntaxNode("Select"));
                        parent = newNode;
                    }

                    if (pnChild.NodeType == PlanNodeTypeEnum.And || pnChild.NodeType == PlanNodeTypeEnum.Or)
                    {
                        previousNode = parent;
                        var newNode = parent.AddChildren(new SyntaxNode("Predicate"));
                        parent = newNode;
                    }

                    if (pnChild.NodeType == PlanNodeTypeEnum.And || pnChild.NodeType == PlanNodeTypeEnum.Or)
                    {
                        previousNode = parent;
                        var newNode = parent.AddChildren(new SyntaxNode(pnChild.NodeType.ToString()));
                        parent = newNode;
                    }

                    if (pnChild.NodeType == PlanNodeTypeEnum.ObservableFrom)
                    {
                        GetRoot(parent).AddChildren(new SyntaxNode("From " + pnChild.Children[0].NodeText));
                    }

                    if (pnChild.NodeType == PlanNodeTypeEnum.TupleProjection || pnChild.NodeType == PlanNodeTypeEnum.Equal || pnChild.NodeType == PlanNodeTypeEnum.GreaterThanOrEqual || pnChild.NodeType == PlanNodeTypeEnum.GreaterThan || pnChild.NodeType == PlanNodeTypeEnum.LessThanOrEqual || pnChild.NodeType == PlanNodeTypeEnum.LessThan)
                    {
                        iterateOverAll = true;
                    }
                    
                    if (iterateOverAll)
                    {
                        previousNode = parent;

                        if (pnChild.NodeType == PlanNodeTypeEnum.Equal || pnChild.NodeType == PlanNodeTypeEnum.GreaterThanOrEqual || pnChild.NodeType == PlanNodeTypeEnum.GreaterThan || pnChild.NodeType == PlanNodeTypeEnum.LessThanOrEqual || pnChild.NodeType == PlanNodeTypeEnum.LessThan)
                        {
                            var newNode = parent.AddChildren(new SyntaxNode("Comparision predicate " + pnChild.NodeText));
                            parent = newNode;
                        }
                        else
                        {

                            var newNode = parent.AddChildren(new SyntaxNode(pnChild.NodeType.ToString() + " " + pnChild.NodeText));
                            parent = newNode;
                        }
                    }

                    if (pnChild.NodeType == PlanNodeTypeEnum.And || pnChild.NodeType == PlanNodeTypeEnum.Or)
                    {
                        //System.Console.WriteLine(string.Format("{0}{1}", new string(' ', level), pnChild.NodeType));
                    }

                    Parse(pnChild, parent);

                    if (pnChild.NodeType == PlanNodeTypeEnum.TupleProjection || pnChild.NodeType == PlanNodeTypeEnum.Equal || pnChild.NodeType == PlanNodeTypeEnum.GreaterThanOrEqual || pnChild.NodeType == PlanNodeTypeEnum.GreaterThan || pnChild.NodeType == PlanNodeTypeEnum.LessThanOrEqual || pnChild.NodeType == PlanNodeTypeEnum.LessThan)
                    {
                        iterateOverAll = false;
                    }

                    if (previousNode != null)
                        parent = previousNode;

                    if (pnChild.NodeType == PlanNodeTypeEnum.And || pnChild.NodeType == PlanNodeTypeEnum.Or)
                    {
                        parent = parent.Parent;
                    }
                }
            }
        }

        private SyntaxNode GetRoot(SyntaxNode node)
        {
            if (node.Parent == null)
                return node;

            return GetRoot(node.Parent);
        }
    }
}
