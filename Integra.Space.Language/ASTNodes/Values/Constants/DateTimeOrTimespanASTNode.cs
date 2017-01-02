//-----------------------------------------------------------------------
// <copyright file="DateTimeOrTimespanASTNode.cs" company="CompanyName">
//     Copyright (c) CompanyName. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Constants
{
    using System;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// DateTimeOrTimespanNode class
    /// </summary>
    internal sealed class DateTimeOrTimespanASTNode : AstNodeBase
    {
        /// <summary>
        /// result plan
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

            this.result = new PlanNode(this.Location.Line, this.Location.Column, this.NodeText);
            this.result.Column = treeNode.Token.Location.Column;
            this.result.Line = treeNode.Token.Location.Line;
            this.result.NodeText = treeNode.Token.Text;
            this.result.NodeType = PlanNodeTypeEnum.Constant;
            this.result.Properties.Add("IsConstant", true);

            DateTime d;
            TimeSpan t;
            string cadena = treeNode.Token.Value.ToString();

            if (TimeSpan.TryParse(cadena, out t))
            {
                this.result.Properties.Add("Value", cadena);
                this.result.Properties.Add("DataType", typeof(TimeSpan));
            }
            else if (DateTime.TryParse(cadena, out d))
            {
                this.result.Properties.Add("Value", cadena);
                this.result.Properties.Add("DataType", typeof(DateTime));
            }
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            return this.result;
        }
    }
}
