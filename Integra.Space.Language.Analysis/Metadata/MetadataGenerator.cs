using Integra.Space.Language.Analysis.Metadata.MetadataNodes;
using Integra.Space.Language.Exceptions;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integra.Space.Language.Analysis
{
    internal class MetadataGenerator
    {
        public SpaceParseTreeNode ConvertIronyParseTree(ParseTreeNode ptNode)
        {
            if (ptNode == null)
            {
                throw new Exception("Parse tree node cannot be null.");
            }

            SpaceParseTreeNode root = this.SelectSpaceParseTreeNode(ptNode);

            if (ptNode.ChildNodes != null)
            {
                root.ChildNodes = new List<SpaceParseTreeNode>();

                foreach (ParseTreeNode node in ptNode.ChildNodes)
                {
                    root.ChildNodes.Add(this.ConvertIronyParseTree(node));
                }
            }

            return root;
        }

        private SpaceParseTreeNode SelectSpaceParseTreeNode(ParseTreeNode ptNode)
        {
            SpaceParseTreeNode result = null;
            string nodeType = ptNode.Term.Name;

            if (Enum.IsDefined(typeof(SpaceParseTreeNodeTypeEnum), nodeType))
            {
                result = new SpaceParseTreeNode((SpaceParseTreeNodeTypeEnum)Enum.Parse(typeof(SpaceParseTreeNodeTypeEnum), nodeType));
                if (ptNode.Token != null)
                {
                    result.Token = ptNode.Token.Text;
                    result.TokenValueDataType = ptNode.Token.Value.GetType();
                    result.TokenValue = ptNode.Token.ValueString;
                }
            }
            else
            {
                throw new Exception(string.Format("{0} is an invalid node type, space parse tree node not found.", nodeType));
            }

            return result;
        }
    }
}
