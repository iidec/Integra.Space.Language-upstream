//-----------------------------------------------------------------------
// <copyright file="DateTimeOrTimespanNode.cs" company="CompanyName">
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
    internal sealed class DateTimeOrTimespanNode : AstNodeBase
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

            this.result = new PlanNode();
            this.result.Column = treeNode.Token.Location.Column;
            this.result.Line = treeNode.Token.Location.Line;
            this.result.NodeText = treeNode.Token.Text;
            this.result.NodeType = PlanNodeTypeEnum.Constant;
            this.result.Properties.Add("IsConstant", true);

            DateTime d;
            TimeSpan t;

            if (TimeSpan.TryParse(treeNode.Token.Value.ToString(), out t))
            {
                this.result.Properties.Add("Value", t);
                this.result.Properties.Add("DataType", typeof(TimeSpan).ToString());
            }
            else if (DateTime.TryParse(treeNode.Token.Value.ToString(), out d))
            {
                this.result.Properties.Add("Value", d);
                this.result.Properties.Add("DataType", typeof(DateTime).ToString());
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
