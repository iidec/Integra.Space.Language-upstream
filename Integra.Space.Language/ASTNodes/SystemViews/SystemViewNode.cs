//-----------------------------------------------------------------------
// <copyright file="SystemViewNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.SystemViews
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// SystemViewNode class
    /// </summary>
    internal sealed class SystemViewNode : AstNodeBase
    {
        /// <summary>
        /// reserved word 'from'
        /// </summary>
        private string fromWord;

        /// <summary>
        /// from statement
        /// </summary>
        private string fromText;

        /// <summary>
        /// reserved word 'where'
        /// </summary>
        private string whereWord;

        /// <summary>
        /// where statement
        /// </summary>
        private string whereText;

        /// <summary>
        /// reserved word 'select'
        /// </summary>
        private string selectWord;

        /// <summary>
        /// select statement
        /// </summary>
        private string selectText;

        /// <summary>
        /// result plan node
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 4)
            {
                this.fromWord = (string)ChildrenNodes[2].Token.Value;
                this.fromText = (string)ChildrenNodes[3].Token.Value;
                this.selectWord = (string)ChildrenNodes[0].Token.Value;
                this.selectText = (string)ChildrenNodes[1].Token.Value;
            }
            else
            {
                this.fromWord = (string)ChildrenNodes[2].Token.Value;
                this.fromText = (string)ChildrenNodes[3].Token.Value;
                this.whereWord = (string)ChildrenNodes[4].Token.Value;
                this.whereText = (string)ChildrenNodes[5].Token.Value;
                this.selectWord = (string)ChildrenNodes[0].Token.Value;
                this.selectText = (string)ChildrenNodes[1].Token.Value;
            }

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.SystemQuery;
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            this.result.NodeText = this.selectWord + " " + this.selectText + "\n" + this.fromWord + " " + this.fromText + "\n" + this.whereWord + " " + this.whereText;
            
            this.result.Properties.Add("From", this.fromText);
            this.result.Properties.Add("Where", this.whereText);
            this.result.Properties.Add("Select", this.selectText);

            return this.result;
        }
    }
}
