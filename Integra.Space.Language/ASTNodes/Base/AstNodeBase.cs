//-----------------------------------------------------------------------
// <copyright file="AstNodeBase.cs" company="ARITEC">
//     Integra Vision. All rights reserved.
// </copyright>
// <author>Oscar Canek</author>
//-----------------------------------------------------------------------

namespace Integra.Space.Language.ASTNodes.Base
{
    using System.Collections.Generic;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ASTNodeBase base class of AST nodes
    /// </summary>
    internal class AstNodeBase : AstNode
    {
        /// <summary>
        /// childrenNodes
        /// List of children
        /// </summary>
        private IList<ParseTreeNode> childrenNodes;

        /// <summary>
        /// Node text.
        /// </summary>
        private string nodeText;

        /// <summary>
        /// Gets the node text.
        /// </summary>
        public string NodeText
        {
            get
            {
                if (string.IsNullOrEmpty(this.nodeText))
                {
                    this.nodeText = this.GetNodeText(this.childrenNodes);
                }

                return this.nodeText;
            }
        }

        /// <summary>
        /// Gets the list of children
        /// </summary>
        protected virtual IList<ParseTreeNode> ChildrenNodes
        {
            get
            {
                return this.childrenNodes;
            }
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);            
            this.childrenNodes = treeNode.GetMappedChildNodes();
        }

        /// <summary>
        /// BeginEvaluate
        /// Called at the start of the analysis
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        protected virtual void BeginEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
        }

        /// <summary>
        /// EndEvaluate
        /// Called at the end of the analysis
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        protected virtual void EndEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this.Parent;
        }

        /// <summary>
        /// Gets the results from an internal parse, the goal of this method is to get the results of a stream query parse.
        /// </summary>
        /// <param name="thread">Thread of the actual grammar</param>
        /// <param name="results">Results of the stream query parse.</param>
        protected void GetQueryParseResults(ScriptThread thread, IEnumerable<Common.ResultBase> results)
        {
            foreach (var result in results)
            {
                if (result is ParseErrorResult)
                {
                    ParseErrorResult r = (ParseErrorResult)result;
                    thread.App.LastScript.ParserMessages.Add(new Irony.LogMessage(Irony.ErrorLevel.Error, new SourceLocation(this.Position, r.Line, r.Column), r.Message, null));
                }
                else if (result is ParseWarningResult)
                {
                    ParseWarningResult r = (ParseWarningResult)result;
                    thread.App.LastScript.ParserMessages.Add(new Irony.LogMessage(Irony.ErrorLevel.Error, new SourceLocation(this.Position, r.Line, r.Column), r.Message, null));
                }
                else if (result is ParseInfoResult)
                {
                    ParseInfoResult r = (ParseInfoResult)result;
                    thread.App.LastScript.ParserMessages.Add(new Irony.LogMessage(Irony.ErrorLevel.Error, new SourceLocation(this.Position, r.Line, r.Column), r.Message, null));
                }
            }
        }

        /// <summary>
        /// Gets the test of the non terminal children.
        /// </summary>
        /// <param name="childrenNodes">Non terminal children nodes.</param>
        /// <returns>Non terminal text value.</returns>
        private string GetNodeText(System.Collections.IEnumerable childrenNodes)
        {
            string nodeText = string.Empty;

            if (childrenNodes != null)
            {
                foreach (ParseTreeNode node in childrenNodes)
                {
                    if (node.IsPunctuationOrEmptyTransient())
                    {
                        return nodeText;
                    }

                    if (node.ChildNodes.Count > 0)
                    {
                        nodeText = string.Concat(nodeText, " ", this.GetNodeText(node.ChildNodes));
                    }
                    else
                    {
                        if (node.Token != null)
                        {
                            nodeText = string.Concat(nodeText, " ", node.Token.Text);
                        }
                    }
                }
            }

            return nodeText.Trim();
        }
    }
}
