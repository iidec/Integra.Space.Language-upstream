//-----------------------------------------------------------------------
// <copyright file="UserQueryNode.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.UserQuery
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;
    using Runtime;

    /// <summary>
    /// User query node.
    /// </summary>
    internal sealed class UserQueryNode : AstNodeBase
    {
        /// <summary>
        /// Query from section.
        /// </summary>
        private AstNodeBase from;

        /// <summary>
        /// Query where section.
        /// </summary>
        private AstNodeBase where;

        /// <summary>
        /// Query apply window section
        /// </summary>
        private AstNode applyWindow;

        /// <summary>
        /// Query group by section
        /// </summary>
        private AstNode groupBy;

        /// <summary>
        /// Query select section.
        /// </summary>
        private AstNodeBase select;

        /// <summary>
        /// Sixth section of the query.
        /// </summary>
        private AstNodeBase sixth;

        /// <summary>
        /// Result plan node
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
            if (childrenCount == 3)
            {
                this.from = AddChild(NodeUseType.Keyword, SR.FromRole, ChildrenNodes[0]) as AstNodeBase;
                this.where = AddChild(NodeUseType.Keyword, SR.WhereRole, ChildrenNodes[1]) as AstNodeBase;
                this.select = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[2]) as AstNodeBase;
            }
            else if (childrenCount == 2)
            {
                this.from = AddChild(NodeUseType.Keyword, SR.FromRole, ChildrenNodes[0]) as AstNodeBase;
                this.select = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[1]) as AstNodeBase;
            }
            else if (childrenCount == 4)
            {
                this.from = AddChild(NodeUseType.Keyword, SR.FromRole, ChildrenNodes[0]) as AstNodeBase;
                this.applyWindow = AddChild(NodeUseType.Keyword, SR.ApplyWindowRole, ChildrenNodes[1]) as AstNodeBase;
                this.groupBy = AddChild(NodeUseType.Keyword, SR.GroupByRole, ChildrenNodes[2]) as AstNodeBase;
                this.select = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[3]) as AstNodeBase;
            }
            else if (childrenCount == 5)
            {
                this.from = AddChild(NodeUseType.Keyword, SR.FromRole, ChildrenNodes[0]) as AstNodeBase;
                this.where = AddChild(NodeUseType.Keyword, SR.WhereRole, ChildrenNodes[1]) as AstNodeBase;
                this.applyWindow = AddChild(NodeUseType.Keyword, SR.ApplyWindowRole, ChildrenNodes[2]) as AstNodeBase;
                this.groupBy = AddChild(NodeUseType.Keyword, SR.GroupByRole, ChildrenNodes[3]) as AstNodeBase;
                this.select = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[4]) as AstNodeBase;
            }
            else if (childrenCount == 6)
            {
                this.from = AddChild(NodeUseType.Keyword, SR.FromRole, ChildrenNodes[0]) as AstNodeBase;
                this.where = AddChild(NodeUseType.Keyword, SR.WhereRole, ChildrenNodes[1]) as AstNodeBase;
                this.applyWindow = AddChild(NodeUseType.Keyword, SR.ApplyWindowRole, ChildrenNodes[2]) as AstNodeBase;
                this.groupBy = AddChild(NodeUseType.Keyword, SR.GroupByRole, ChildrenNodes[3]) as AstNodeBase;
                this.select = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[4]) as AstNodeBase;
                this.sixth = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[5]) as AstNodeBase;
            }

            this.result = new PlanNode();
            this.result.NodeType = PlanNodeTypeEnum.UserQuery;
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

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 2)
            {
                PlanNode fromAux = (PlanNode)this.from.Evaluate(thread);
                PlanNode projectionAux = (PlanNode)this.select.Evaluate(thread);

                this.result = this.CreateFromSelect(fromAux, projectionAux);
                this.result.Column = fromAux.Column;
                this.result.Line = fromAux.Line;
                this.result.NodeText = string.Format("{0} {1}", fromAux.NodeText, projectionAux.NodeText);
            }
            else if (childrenCount == 3)
            {
                PlanNode fromAux = (PlanNode)this.from.Evaluate(thread);
                PlanNode secondArgument = (PlanNode)this.where.Evaluate(thread);
                PlanNode projectionAux = (PlanNode)this.select.Evaluate(thread);

                if (secondArgument.NodeType.IsWhere())
                {
                    this.result = this.CreateFromWhereSelect(fromAux, secondArgument, projectionAux);
                }
                else if (secondArgument.NodeType.IsBuffer())
                {
                    if (this.CheckProjectionIfAllAreFunctions(projectionAux))
                    {
                        this.result = this.CreateFromApplyWindowSelectWithOnlyFunctionsInProjection(fromAux, secondArgument, projectionAux);
                    }
                    else
                    {
                        this.result = this.CreateFromApplyWindowSelectWithoutOnlyFunctionsInProjection(fromAux, secondArgument, projectionAux);
                    }
                }

                this.result.Column = fromAux.Column;
                this.result.Line = fromAux.Line;
                this.result.NodeText = string.Format("{0} {1} {2}", fromAux.NodeText, secondArgument.NodeText, projectionAux.NodeText);
            }
            else if (childrenCount == 4)
            {
                PlanNode fromAux = (PlanNode)this.from.Evaluate(thread);
                PlanNode secondArgument = (PlanNode)this.applyWindow.Evaluate(thread);
                PlanNode thirdArgument = (PlanNode)this.groupBy.Evaluate(thread);
                PlanNode fourthArgument = (PlanNode)this.select.Evaluate(thread);

                if ((secondArgument.NodeType.IsBuffer() || thirdArgument.NodeType.IsProjectionOfSelect()) && fourthArgument.NodeType.IsOrderBy())
                {
                    if (this.CheckProjectionIfAllAreFunctions(thirdArgument))
                    {
                        // no es necesario hacer el order by teniendo unicamente funciones de agregación en la proyección
                        this.result = this.CreateFromApplyWindowSelectWithOnlyFunctionsInProjection(fromAux, secondArgument, thirdArgument);
                    }
                    else
                    {
                        this.result = this.CreateFromApplyWindowSelectWithoutOnlyFunctionsInProjectionOrderBy(fromAux, secondArgument, thirdArgument, fourthArgument);
                    }
                }
                else if (secondArgument.NodeType.IsBuffer() && thirdArgument.NodeType.IsGroupBy() && fourthArgument.NodeType.IsProjectionOfSelect())
                {
                    this.result = this.CreateFromApplyWindowGroupBySelect(fromAux, secondArgument, thirdArgument, fourthArgument);
                }
                else if (secondArgument.NodeType.IsWhere() && thirdArgument.NodeType.IsBuffer() && fourthArgument.NodeType.IsProjectionOfSelect())
                {
                    if (this.CheckProjectionIfAllAreFunctions(fourthArgument))
                    {
                        this.result = this.CreateFromWhereApplyWindowSelectWithOnlyFunctionsInProjection(fromAux, secondArgument, thirdArgument, fourthArgument);
                    }
                    else
                    {
                        this.result = this.CreateFromWhereApplyWindowSelectWithoutOnlyFunctionsInProjection(fromAux, secondArgument, thirdArgument, fourthArgument);
                    }
                }

                this.result.Column = fromAux.Column;
                this.result.Line = fromAux.Line;
                this.result.NodeText = string.Format("{0} {1} {2} {3}", fromAux.NodeText, secondArgument.NodeText, thirdArgument.NodeText, fourthArgument.NodeText);
            }
            else if (childrenCount == 5)
            {
                PlanNode fromAux = (PlanNode)this.from.Evaluate(thread);
                PlanNode secondArgument = (PlanNode)this.where.Evaluate(thread);
                PlanNode thirdArgument = (PlanNode)this.applyWindow.Evaluate(thread);
                PlanNode fourthArgument = (PlanNode)this.groupBy.Evaluate(thread);
                PlanNode fifthArgument = (PlanNode)this.select.Evaluate(thread);

                if (secondArgument.NodeType.IsWhere() && thirdArgument.NodeType.IsBuffer() && fourthArgument.NodeType.IsGroupBy() && fifthArgument.NodeType.IsProjectionOfSelect())
                {
                    this.result = this.CreateFromWhereApplyWindowGroupBySelect(fromAux, secondArgument, thirdArgument, fourthArgument, fifthArgument);
                }
                else if (secondArgument.NodeType.IsWhere() && thirdArgument.NodeType.IsBuffer() && fourthArgument.NodeType.IsProjectionOfSelect() && fifthArgument.NodeType.IsOrderBy())
                {
                    if (this.CheckProjectionIfAllAreFunctions(fourthArgument))
                    {
                        this.result = this.CreateFromWhereApplyWindowSelectWithOnlyFunctionsInProjection(fromAux, secondArgument, thirdArgument, fourthArgument);
                    }
                    else
                    {
                        this.result = this.CreateFromWhereApplyWindowSelectWithoutOnlyFunctionsInProjectionOrderBy(fromAux, secondArgument, thirdArgument, fourthArgument, fifthArgument);
                    }
                }
                else if (secondArgument.NodeType.IsBuffer() && thirdArgument.NodeType.IsGroupBy() && fourthArgument.NodeType.IsProjectionOfSelect() && fifthArgument.NodeType.IsOrderBy())
                {
                    this.result = this.CreateFromApplyWindowGroupBySelectOrderBy(fromAux, secondArgument, thirdArgument, fourthArgument, fifthArgument);
                }

                this.result.Column = fromAux.Column;
                this.result.Line = fromAux.Line;
                this.result.NodeText = string.Format("{0} {1} {2} {3} {4}", fromAux.NodeText, secondArgument.NodeText, thirdArgument.NodeText, fourthArgument.NodeText, fifthArgument.NodeText);
            }
            else if (childrenCount == 6)
            {
                PlanNode fromAux = (PlanNode)this.from.Evaluate(thread);
                PlanNode secondArgument = (PlanNode)this.where.Evaluate(thread);
                PlanNode thirdArgument = (PlanNode)this.applyWindow.Evaluate(thread);
                PlanNode fourthArgument = (PlanNode)this.groupBy.Evaluate(thread);
                PlanNode fifthArgument = (PlanNode)this.select.Evaluate(thread);
                PlanNode sixthArgument = (PlanNode)this.sixth.Evaluate(thread);

                this.result = this.CreateFromWhereApplyWindowGroupBySelectOrderBy(fromAux, secondArgument, thirdArgument, fourthArgument, fifthArgument, sixthArgument);

                this.result.Column = fromAux.Column;
                this.result.Line = fromAux.Line;
                this.result.NodeText = string.Format("{0} {1} {2} {3} {4} {5}", fromAux.NodeText, secondArgument.NodeText, thirdArgument.NodeText, fourthArgument.NodeText, fifthArgument.NodeText, sixthArgument.NodeText);
            }

            this.EndEvaluate(thread);

            return this.result;
        }

        /// <summary>
        /// Check if all projection columns are functions.
        /// </summary>
        /// <param name="projection">Projection plan.</param>
        /// <returns>True or false.</returns>
        private bool CheckProjectionIfAllAreFunctions(PlanNode projection)
        {
            return projection.Children.All(x => PlanNodeTypeEnum.EnumerableMin.Equals(x.Children[1].NodeType) || PlanNodeTypeEnum.EnumerableMax.Equals(x.Children[1].NodeType) || PlanNodeTypeEnum.EnumerableCount.Equals(x.Children[1].NodeType) || PlanNodeTypeEnum.EnumerableSum.Equals(x.Children[1].NodeType));
        }

        /// <summary>
        /// Check if all projection columns are values.
        /// </summary>
        /// <param name="projection">Projection plan.</param>
        /// <returns>True or false.</returns>
        private bool CheckProjectionIfAllAreValues(PlanNode projection)
        {
            return projection.Children.All(x => PlanNodeTypeEnum.Event.Equals(x.Children[1].NodeType) || PlanNodeTypeEnum.Constant.Equals(x.Children[1].NodeType) || PlanNodeTypeEnum.Identifier.Equals(x.Children[1].NodeType) || PlanNodeTypeEnum.GroupKey.Equals(x.Children[1].NodeType));
        }

        /// <summary>
        /// Check if the projection columns are values and functions.
        /// </summary>
        /// <param name="projection">Projection plan.</param>
        /// <returns>True or false.</returns>
        private bool CheckProjectionIfAreValuesAndFunctions(PlanNode projection)
        {
            if (!this.CheckProjectionIfAllAreFunctions(projection) && !this.CheckProjectionIfAllAreValues(projection))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates the from select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromSelect(PlanNode fromAux, PlanNode projectionAux)
        {
            PlanNode scopeSelectForSource = new PlanNode();
            scopeSelectForSource.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForSource.Children = new List<PlanNode>();

            scopeSelectForSource.Children.Add(fromAux);

            PlanNode selectForSource = new PlanNode();
            selectForSource.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForSource.Children = new List<PlanNode>();

            selectForSource.Children.Add(scopeSelectForSource);
            selectForSource.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", int.Parse(ConfigurationManager.AppSettings["DefaultWindowSize"]));
            bufferSize.Properties.Add("DataType", typeof(int));

            buffer.Children.Add(selectForSource);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.NodeType = PlanNodeTypeEnum.ObservableTake;
                planTop.Children[0] = buffer;

                return planTop;
            }

            return buffer;
        }

        /// <summary>
        /// Creates the from where select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereSelect(PlanNode fromAux, PlanNode whereAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(whereAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            selectForBuffer.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", int.Parse(ConfigurationManager.AppSettings["DefaultWindowSize"]));
            bufferSize.Properties.Add("DataType", typeof(int));

            buffer.Children.Add(selectForBuffer);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.NodeType = PlanNodeTypeEnum.ObservableTake;
                planTop.Children[0] = buffer;

                return planTop;
            }

            /* ******************************************************************************************************************************************************** */

            return buffer;
        }

        /// <summary>
        /// Creates the from where select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="applyWindow">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowSelectWithOnlyFunctionsInProjection(PlanNode fromAux, PlanNode applyWindow, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            applyWindow.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindow.Children[1] = applyWindow.Children[1].Children[0].Children[1];
            applyWindow.Children[0] = fromAux;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindow);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();
            /* ******************************************************************************************************************************************************** */
            selectForBuffer.Children.Add(scopeSelectForBuffer);
            selectForBuffer.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", applyWindow.Children[1].Properties["Value"]);
            bufferSize.Properties.Add("DataType", applyWindow.Children[1].Properties["DataType"]);
            /* ******************************************************************************************************************************************************** */
            buffer.Children.Add(selectForBuffer);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */

            return buffer;
        }

        /// <summary>
        /// Creates the from where select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowSelectWithoutOnlyFunctionsInProjection(PlanNode fromAux, PlanNode applyWindowAux, PlanNode projectionAux)
        {
            applyWindowAux.Children[0] = fromAux;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeSelectForEnumerable = new PlanNode();
            scopeSelectForEnumerable.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForEnumerable.Children = new List<PlanNode>();

            scopeSelectForEnumerable.Children.Add(fromForLambda);

            PlanNode selectForEnumerable = new PlanNode();
            selectForEnumerable.NodeType = PlanNodeTypeEnum.EnumerableSelectForEnumerable;
            selectForEnumerable.Children = new List<PlanNode>();

            selectForEnumerable.Children.Add(scopeSelectForEnumerable);
            selectForEnumerable.Children.Add(projectionAux);                        
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = selectForEnumerable;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(selectForEnumerable);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from where select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowSelectWithoutOnlyFunctionsInProjectionOrderBy(PlanNode fromAux, PlanNode applyWindowAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            applyWindowAux.Children[0] = fromAux;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeSelectForEnumerable = new PlanNode();
            scopeSelectForEnumerable.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForEnumerable.Children = new List<PlanNode>();

            scopeSelectForEnumerable.Children.Add(fromForLambda);

            PlanNode selectForEnumerable = new PlanNode();
            selectForEnumerable.NodeType = PlanNodeTypeEnum.EnumerableSelectForEnumerable;
            selectForEnumerable.Children = new List<PlanNode>();

            selectForEnumerable.Children.Add(scopeSelectForEnumerable);
            selectForEnumerable.Children.Add(projectionAux);

            orderByAux.Children[0].Children = new List<PlanNode>();
            orderByAux.Children[0].Children.Add(selectForEnumerable);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = orderByAux;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(orderByAux);
            }

            /* ******************************************************************************************************************************************************** */
            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from where applyWindow groupBy select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereApplyWindowGroupBySelect(PlanNode fromAux, PlanNode whereAux, PlanNode applyWindowAux, PlanNode groupByAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);
            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            applyWindowAux.Children[0] = whereAux;
            /* ******************************************************************************************************************************************************** */
            NodesFinder nf = new NodesFinder();
            List<PlanNode> lpn = nf.FindNode(projectionAux, PlanNodeTypeEnum.TupleProjection);
            foreach (PlanNode tuple in lpn)
            {
                PlanNode tupleValue = tuple.Children[1];
                if (tupleValue.NodeType.Equals(PlanNodeTypeEnum.Identifier))
                {
                    PlanNode groupKey = new PlanNode();
                    groupKey.NodeType = PlanNodeTypeEnum.GroupKey;
                    groupKey.Column = tupleValue.Column;
                    groupKey.Line = tupleValue.Line;
                    groupKey.NodeText = tupleValue.NodeText;
                    groupKey.Properties.Add("Value", "Key");

                    tupleValue.NodeType = PlanNodeTypeEnum.GroupKeyProperty;
                    tupleValue.Children = new List<PlanNode>();
                    tupleValue.Children.Add(groupKey);
                }
            }

            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeKeySelectorForGroupBy = new PlanNode();
            scopeKeySelectorForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeKeySelectorForGroupBy.Children = new List<PlanNode>();

            scopeKeySelectorForGroupBy.Children.Add(fromForLambda);
            groupByAux.Children[0].Children.Add(fromForLambda);

            PlanNode scopeSelectForGroupBy = new PlanNode();
            scopeSelectForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForGroupBy.Children = new List<PlanNode>();

            scopeSelectForGroupBy.Children.Add(groupByAux);

            PlanNode selectForGroupBy = new PlanNode();
            selectForGroupBy.NodeType = PlanNodeTypeEnum.EnumerableSelectForGroupBy;
            selectForGroupBy.Children = new List<PlanNode>();

            selectForGroupBy.Children.Add(scopeSelectForGroupBy);
            selectForGroupBy.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = selectForGroupBy;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(selectForGroupBy);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from applyWindow groupBy select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowGroupBySelect(PlanNode fromAux, PlanNode applyWindowAux, PlanNode groupByAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            applyWindowAux.Children[0] = fromAux;
            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            /* ******************************************************************************************************************************************************** */
            NodesFinder nf = new NodesFinder();
            List<PlanNode> lpn = nf.FindNode(projectionAux, PlanNodeTypeEnum.TupleProjection);
            foreach (PlanNode tuple in lpn)
            {
                PlanNode tupleValue = tuple.Children[1];
                if (tupleValue.NodeType.Equals(PlanNodeTypeEnum.Identifier))
                {
                    PlanNode groupKey = new PlanNode();
                    groupKey.NodeType = PlanNodeTypeEnum.GroupKey;
                    groupKey.Column = tupleValue.Column;
                    groupKey.Line = tupleValue.Line;
                    groupKey.NodeText = tupleValue.NodeText;
                    groupKey.Properties.Add("Value", "Key");

                    tupleValue.NodeType = PlanNodeTypeEnum.GroupKeyProperty;
                    tupleValue.Children = new List<PlanNode>();
                    tupleValue.Children.Add(groupKey);
                }
            }

            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeKeySelectorForGroupBy = new PlanNode();
            scopeKeySelectorForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeKeySelectorForGroupBy.Children = new List<PlanNode>();

            scopeKeySelectorForGroupBy.Children.Add(fromForLambda);
            groupByAux.Children[0].Children.Add(fromForLambda);

            PlanNode scopeSelectForGroupBy = new PlanNode();
            scopeSelectForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForGroupBy.Children = new List<PlanNode>();

            scopeSelectForGroupBy.Children.Add(groupByAux);

            PlanNode selectForGroupBy = new PlanNode();
            selectForGroupBy.NodeType = PlanNodeTypeEnum.EnumerableSelectForGroupBy;
            selectForGroupBy.Children = new List<PlanNode>();

            selectForGroupBy.Children.Add(scopeSelectForGroupBy);
            selectForGroupBy.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = selectForGroupBy;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(selectForGroupBy);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from where applyWindow select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereApplyWindowSelectWithOnlyFunctionsInProjection(PlanNode fromAux, PlanNode whereAux, PlanNode applyWindowAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);

            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            applyWindowAux.Children[0] = whereAux;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();
            /* ******************************************************************************************************************************************************** */
            selectForBuffer.Children.Add(scopeSelectForBuffer);
            selectForBuffer.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", applyWindowAux.Children[1].Properties["Value"]);
            bufferSize.Properties.Add("DataType", applyWindowAux.Children[1].Properties["DataType"]);

            buffer.Children.Add(selectForBuffer);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */

            return buffer;
        }

        /// <summary>
        /// Creates the from where applyWindow select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereApplyWindowSelectWithoutOnlyFunctionsInProjection(PlanNode fromAux, PlanNode whereAux, PlanNode applyWindowAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);
            applyWindowAux.Children[0] = whereAux;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeSelectForEnumerable = new PlanNode();
            scopeSelectForEnumerable.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForEnumerable.Children = new List<PlanNode>();

            scopeSelectForEnumerable.Children.Add(fromForLambda);

            PlanNode selectForEnumerable = new PlanNode();
            selectForEnumerable.NodeType = PlanNodeTypeEnum.EnumerableSelectForEnumerable;
            selectForEnumerable.Children = new List<PlanNode>();

            selectForEnumerable.Children.Add(scopeSelectForEnumerable);
            selectForEnumerable.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = selectForEnumerable;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(selectForEnumerable);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from where applyWindow select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereApplyWindowSelectWithoutOnlyFunctionsInProjectionOrderBy(PlanNode fromAux, PlanNode whereAux, PlanNode applyWindowAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);
            applyWindowAux.Children[0] = whereAux;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeSelectForEnumerable = new PlanNode();
            scopeSelectForEnumerable.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForEnumerable.Children = new List<PlanNode>();

            scopeSelectForEnumerable.Children.Add(fromForLambda);

            PlanNode selectForEnumerable = new PlanNode();
            selectForEnumerable.NodeType = PlanNodeTypeEnum.EnumerableSelectForEnumerable;
            selectForEnumerable.Children = new List<PlanNode>();

            selectForEnumerable.Children.Add(scopeSelectForEnumerable);
            selectForEnumerable.Children.Add(projectionAux);

            orderByAux.Children[0].Children = new List<PlanNode>();
            orderByAux.Children[0].Children.Add(selectForEnumerable);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = orderByAux;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(orderByAux);
            }
            
            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from applyWindow groupBy select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowGroupBySelectOrderBy(PlanNode fromAux, PlanNode applyWindowAux, PlanNode groupByAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            /* ******************************************************************************************************************************************************** */
            applyWindowAux.Children[0] = fromAux;
            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            /* ******************************************************************************************************************************************************** */
            NodesFinder nf = new NodesFinder();
            List<PlanNode> lpn = nf.FindNode(projectionAux, PlanNodeTypeEnum.TupleProjection);
            foreach (PlanNode tuple in lpn)
            {
                PlanNode tupleValue = tuple.Children[1];
                if (tupleValue.NodeType.Equals(PlanNodeTypeEnum.Identifier))
                {
                    PlanNode groupKey = new PlanNode();
                    groupKey.NodeType = PlanNodeTypeEnum.GroupKey;
                    groupKey.Column = tupleValue.Column;
                    groupKey.Line = tupleValue.Line;
                    groupKey.NodeText = tupleValue.NodeText;
                    groupKey.Properties.Add("Value", "Key");

                    tupleValue.NodeType = PlanNodeTypeEnum.GroupKeyProperty;
                    tupleValue.Children = new List<PlanNode>();
                    tupleValue.Children.Add(groupKey);
                }
            }

            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeKeySelectorForGroupBy = new PlanNode();
            scopeKeySelectorForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeKeySelectorForGroupBy.Children = new List<PlanNode>();

            scopeKeySelectorForGroupBy.Children.Add(fromForLambda);
            groupByAux.Children[0].Children.Add(fromForLambda);

            PlanNode scopeSelectForGroupBy = new PlanNode();
            scopeSelectForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForGroupBy.Children = new List<PlanNode>();

            scopeSelectForGroupBy.Children.Add(groupByAux);

            PlanNode selectForGroupBy = new PlanNode();
            selectForGroupBy.NodeType = PlanNodeTypeEnum.EnumerableSelectForGroupBy;
            selectForGroupBy.Children = new List<PlanNode>();

            selectForGroupBy.Children.Add(scopeSelectForGroupBy);
            selectForGroupBy.Children.Add(projectionAux);

            orderByAux.Children[0].Children = new List<PlanNode>();
            orderByAux.Children[0].Children.Add(selectForGroupBy);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = orderByAux;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(orderByAux);
            }
            
            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from where applyWindow groupBy select query execution plan.
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereApplyWindowGroupBySelectOrderBy(PlanNode fromAux, PlanNode whereAux, PlanNode applyWindowAux, PlanNode groupByAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);
            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            applyWindowAux.Children[0] = whereAux;
            /* ******************************************************************************************************************************************************** */
            NodesFinder nf = new NodesFinder();
            List<PlanNode> lpn = nf.FindNode(projectionAux, PlanNodeTypeEnum.TupleProjection);
            foreach (PlanNode tuple in lpn)
            {
                PlanNode tupleValue = tuple.Children[1];
                if (tupleValue.NodeType.Equals(PlanNodeTypeEnum.Identifier))
                {
                    PlanNode groupKey = new PlanNode();
                    groupKey.NodeType = PlanNodeTypeEnum.GroupKey;
                    groupKey.Column = tupleValue.Column;
                    groupKey.Line = tupleValue.Line;
                    groupKey.NodeText = tupleValue.NodeText;
                    groupKey.Properties.Add("Value", "Key");

                    tupleValue.NodeType = PlanNodeTypeEnum.GroupKeyProperty;
                    tupleValue.Children = new List<PlanNode>();
                    tupleValue.Children.Add(groupKey);
                }
            }

            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForBuffer;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /* ******************************************************************************************************************************************************** */
            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode scopeKeySelectorForGroupBy = new PlanNode();
            scopeKeySelectorForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeKeySelectorForGroupBy.Children = new List<PlanNode>();

            scopeKeySelectorForGroupBy.Children.Add(fromForLambda);
            groupByAux.Children[0].Children.Add(fromForLambda);

            PlanNode scopeSelectForGroupBy = new PlanNode();
            scopeSelectForGroupBy.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForGroupBy.Children = new List<PlanNode>();

            scopeSelectForGroupBy.Children.Add(groupByAux);

            PlanNode selectForGroupBy = new PlanNode();
            selectForGroupBy.NodeType = PlanNodeTypeEnum.EnumerableSelectForGroupBy;
            selectForGroupBy.Children = new List<PlanNode>();

            selectForGroupBy.Children.Add(scopeSelectForGroupBy);
            selectForGroupBy.Children.Add(projectionAux);

            orderByAux.Children[0].Children = new List<PlanNode>();
            orderByAux.Children[0].Children.Add(selectForGroupBy);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = orderByAux;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(orderByAux);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }
    }
}
