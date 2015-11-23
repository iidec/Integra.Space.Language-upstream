//-----------------------------------------------------------------------
// <copyright file="ApplyWindowNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using System.Collections.Generic;
    using System.Configuration;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ApplyWindowNode class
    /// </summary>
    internal sealed class ApplyWindowNode : AstNodeBase
    {
        /// <summary>
        /// reserved word apply
        /// </summary>
        private string applyWord;

        /// <summary>
        /// reserved word window
        /// </summary>
        private string windowWord;

        /// <summary>
        /// reserved word of
        /// </summary>
        private string reservedWordOf;

        /// <summary>
        /// first window size
        /// </summary>
        private AstNodeBase windowSize1;

        /// <summary>
        /// second window size
        /// </summary>
        private AstNodeBase windowSize2;

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

            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 4)
            {
                this.applyWord = (string)ChildrenNodes[0].Token.Value;
                this.windowWord = (string)ChildrenNodes[1].Token.Value;
                this.reservedWordOf = (string)ChildrenNodes[2].Token.Value;
                this.windowSize1 = AddChild(NodeUseType.Parameter, "windowSize1", ChildrenNodes[3]) as AstNodeBase;
            }
            else if (childrenCount == 8)
            {
                this.applyWord = (string)ChildrenNodes[0].Token.Value;
                this.windowWord = (string)ChildrenNodes[1].Token.Value;
                this.reservedWordOf = (string)ChildrenNodes[2].Token.Value;
                this.windowSize1 = AddChild(NodeUseType.Parameter, "windowSize1", ChildrenNodes[4]) as AstNodeBase;
                this.windowSize2 = AddChild(NodeUseType.Parameter, "windowSize2", ChildrenNodes[6]) as AstNodeBase;
            }

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

            PlanNode fromLambdaForBuffer = new PlanNode();
            fromLambdaForBuffer.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            this.result.Children = new List<PlanNode>();

            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 4)
            {
                PlanNode windowSizeAux = (PlanNode)this.windowSize1.Evaluate(thread);

                this.result.NodeType = PlanNodeTypeEnum.ObservableBufferTimeAndSize;
                this.result.NodeText = this.applyWord + " " + this.windowWord + " " + this.reservedWordOf + " " + windowSizeAux.NodeText;
                this.result.Children.Add(fromLambdaForBuffer);

                PlanNode planProjection = new PlanNode();
                planProjection.NodeType = PlanNodeTypeEnum.ProjectionOfConstants;
                planProjection.Children = new List<PlanNode>();

                PlanNode planTuple1 = new PlanNode();
                planTuple1.NodeType = PlanNodeTypeEnum.TupleProjection;
                planTuple1.Children = new List<PlanNode>();

                PlanNode alias1 = new PlanNode();
                alias1.NodeType = PlanNodeTypeEnum.Constant;
                alias1.Properties.Add("Value", "TimeSpanValue");
                alias1.Properties.Add("DataType", typeof(object).ToString());

                planTuple1.Children.Add(alias1);
                planTuple1.Children.Add(windowSizeAux);

                PlanNode planTuple2 = new PlanNode();
                planTuple2.NodeType = PlanNodeTypeEnum.TupleProjection;
                planTuple2.Children = new List<PlanNode>();

                PlanNode alias2 = new PlanNode();
                alias2.NodeType = PlanNodeTypeEnum.Constant;
                alias2.Properties.Add("Value", "IntegerValue");
                alias2.Properties.Add("DataType", typeof(object).ToString());
                
                PlanNode maxWindowSize = new PlanNode();
                maxWindowSize.NodeType = PlanNodeTypeEnum.Constant;
                maxWindowSize.Properties.Add("Value", int.Parse(ConfigurationManager.AppSettings["MaxWindowSize"]));
                maxWindowSize.Properties.Add("DataType", typeof(int));

                planTuple2.Children.Add(alias2);
                planTuple2.Children.Add(maxWindowSize);

                planProjection.Children.Add(planTuple1);
                planProjection.Children.Add(planTuple2);

                this.result.Children.Add(planProjection);
            }
            else if (childrenCount == 8)
            {
                PlanNode windowSizeAux1 = (PlanNode)this.windowSize1.Evaluate(thread);
                PlanNode windowSizeAux2 = (PlanNode)this.windowSize2.Evaluate(thread);

                this.result.NodeType = PlanNodeTypeEnum.ObservableBufferTimeAndSize;
                this.result.NodeText = this.applyWord + " " + this.windowWord + " " + this.reservedWordOf + " (" + windowSizeAux1.NodeText + "," + windowSizeAux2.NodeText + ")";
                this.result.Children.Add(fromLambdaForBuffer);

                PlanNode planProjection = new PlanNode();
                planProjection.NodeType = PlanNodeTypeEnum.ProjectionOfConstants;
                planProjection.Children = new List<PlanNode>();

                PlanNode planTuple1 = new PlanNode();
                planTuple1.NodeType = PlanNodeTypeEnum.TupleProjection;
                planTuple1.Children = new List<PlanNode>();

                PlanNode alias1 = new PlanNode();
                alias1.NodeType = PlanNodeTypeEnum.Constant;
                alias1.Properties.Add("Value", "TimeSpanValue");
                alias1.Properties.Add("DataType", typeof(object).ToString());

                planTuple1.Children.Add(alias1);
                planTuple1.Children.Add(windowSizeAux1);

                PlanNode planTuple2 = new PlanNode();
                planTuple2.NodeType = PlanNodeTypeEnum.TupleProjection;
                planTuple2.Children = new List<PlanNode>();

                PlanNode alias2 = new PlanNode();
                alias2.NodeType = PlanNodeTypeEnum.Constant;
                alias2.Properties.Add("Value", "IntegerValue");
                alias2.Properties.Add("DataType", typeof(object).ToString());

                planTuple2.Children.Add(alias2);
                planTuple2.Children.Add(windowSizeAux2);

                planProjection.Children.Add(planTuple1);
                planProjection.Children.Add(planTuple2);

                this.result.Children.Add(planProjection);
            }

            this.EndEvaluate(thread);

            return this.result;
        }
    }
}
