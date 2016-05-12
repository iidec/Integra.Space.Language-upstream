//-----------------------------------------------------------------------
// <copyright file="DateFunctionNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Constants
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;

    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// DateFunctionNode class
    /// </summary>
    internal sealed class DateFunctionNode : AstNodeBase
    {
        /// <summary>
        /// DateTime or Timespan node
        /// </summary>
        private AstNodeBase date;

        /// <summary>
        /// function to execute
        /// </summary>
        private string function;

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
            this.function = (string)ChildrenNodes[0].Token.Value;
            this.date = AddChild(NodeUseType.Parameter, "DateTimeNode", ChildrenNodes[1]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            this.BeginEvaluate(thread);
            PlanNode auxDate = (PlanNode)this.date.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(auxDate);
            this.result.NodeText = string.Format("{0}({1})", this.function, auxDate.NodeText);
            this.result.Properties.Add("DataType", typeof(int?));
            this.result.NodeType = PlanNodeTypeEnum.DateTimeFunction;
            this.result.Properties.Add("IsConstant", bool.Parse(auxDate.Properties["IsConstant"].ToString()));

            switch (this.function.ToLower())
            {
                case "year":
                    this.result.Properties.Add("Property", "Year");
                    break;
                case "month":
                    this.result.Properties.Add("Property", "Month");
                    break;
                case "day":
                    this.result.Properties.Add("Property", "Day");
                    break;
                case "hour":
                    this.result.Properties.Add("Property", "Hour");
                    break;
                case "minute":
                    this.result.Properties.Add("Property", "Minute");
                    break;
                case "second":
                    this.result.Properties.Add("Property", "Second");
                    break;
                case "millisecond":
                    this.result.Properties.Add("Property", "Millisecond");
                    break;
            }

            return this.result;
        }
    }
}
