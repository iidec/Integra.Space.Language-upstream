//-----------------------------------------------------------------------
// <copyright file="QueryForMetadataASTNode.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.MetadataQuery
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
    internal sealed class QueryForMetadataASTNode : AstNodeBase
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
        /// Query select section.
        /// </summary>
        private AstNodeBase select;

        /// <summary>
        /// Sixth section of the query.
        /// </summary>
        private AstNodeBase orderBy;

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
            this.from = AddChild(NodeUseType.Keyword, SR.FromRole, ChildrenNodes[0]) as AstNodeBase;
            this.where = AddChild(NodeUseType.Keyword, SR.WhereRole, ChildrenNodes[1]) as AstNodeBase;
            this.select = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[2]) as AstNodeBase;
            this.orderBy = AddChild(NodeUseType.Keyword, SR.SelectRole, ChildrenNodes[3]) as AstNodeBase;

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

            PlanNode source = (PlanNode)this.from.Evaluate(thread);
            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            int childrenCount = ChildrenNodes.Count;
            
            PlanNode whereAux = (PlanNode)this.where.Evaluate(thread);
            PlanNode selectAux = (PlanNode)this.select.Evaluate(thread);
            PlanNode orderByAux = (PlanNode)this.orderBy.Evaluate(thread);

            // six, five, four, and three arguments
            if (whereAux != null && orderByAux != null)
            {
                this.AuxFourParts(source, whereAux, selectAux, orderByAux);
            }
            else if (whereAux == null && orderByAux != null)
            {
                this.AuxThreeParts(source, selectAux, orderByAux);
            }
            else if (whereAux != null && orderByAux == null)
            {
                this.AuxThreeParts(source, whereAux, selectAux);
            }
            else if (whereAux == null && orderByAux == null)
            {
                this.AuxTwoParts(source, selectAux);
            }

            this.EndEvaluate(thread);

            this.result.Column = source.Column;
            this.result.Line = source.Line;
            /* ******************************************************************************************************************************************************** */

            // nodos para crear el objeto QueryResult resultante
            PlanNode scopeFinalResult = new PlanNode();
            scopeFinalResult.NodeType = PlanNodeTypeEnum.NewScope;
            scopeFinalResult.Children = new List<PlanNode>();

            scopeFinalResult.Children.Add(this.result);

            PlanNode fromForLambda = new PlanNode();
            fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode lambdaForResult = new PlanNode();
            lambdaForResult.NodeType = PlanNodeTypeEnum.SelectForResultProjection;
            lambdaForResult.Children = new List<PlanNode>();

            lambdaForResult.Children.Add(fromForLambda);

            PlanNode finalSelect = new PlanNode();
            finalSelect.NodeType = PlanNodeTypeEnum.SelectForResult;
            finalSelect.NodeText = this.result.NodeText;
            finalSelect.Children = new List<PlanNode>();

            finalSelect.Children.Add(scopeFinalResult);
            finalSelect.Children.Add(lambdaForResult);
            
            return finalSelect;
        }

        /// <summary>
        /// Adds the nodes to compile Subscribe and Create observable extensions.
        /// </summary>
        /// <param name="child">Child node.</param>
        /// <returns>Execution plan node with subscribe and create nodes</returns>
        private PlanNode AddSubscribeAndCreate(PlanNode child)
        {
            // nodo para crear el subscribe
            PlanNode scopeSubscribe = new PlanNode();
            scopeSubscribe.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSubscribe.Children = new List<PlanNode>();
            scopeSubscribe.Children.Add(child);

            PlanNode subscriptionNode = new PlanNode();
            subscriptionNode.NodeType = PlanNodeTypeEnum.Subscription;
            subscriptionNode.Children = new List<PlanNode>();
            subscriptionNode.Children.Add(scopeSubscribe);

            PlanNode observableCreateNode = new PlanNode();
            observableCreateNode.NodeType = PlanNodeTypeEnum.ObservableCreate;
            observableCreateNode.NodeText = child.NodeText;
            observableCreateNode.Children = new List<PlanNode>();
            observableCreateNode.Children.Add(subscriptionNode);

            return observableCreateNode;
        }
        
        #region auxiliar

        /// <summary>
        /// Helper method for query with two parts
        /// </summary>
        /// <param name="fromAux">Query source</param>
        /// <param name="projectionAux">Projection to return</param>
        private void AuxTwoParts(PlanNode fromAux, PlanNode projectionAux)
        {
            this.result = this.CreateFromSelect(fromAux, projectionAux);
            this.result.NodeText = string.Format("{0} {1}", fromAux.NodeText, projectionAux.NodeText);
        }

        /// <summary>
        /// Helper method for query with three parts
        /// </summary>
        /// <param name="fromAux">Query source</param>
        /// <param name="secondArgument">Second argument</param>
        /// <param name="projectionAux">Projection to return</param>
        private void AuxThreeParts(PlanNode fromAux, PlanNode secondArgument, PlanNode projectionAux)
        {
            if (secondArgument.NodeType.IsWhere() && projectionAux.NodeType.IsProjectionOfSelect())
            {
                this.result = this.CreateFromWhereSelect(fromAux, secondArgument, projectionAux);
            }
            else if (secondArgument.NodeType.IsGroupBy() && projectionAux.NodeType.IsProjectionOfSelect())
            {
                this.result = this.CreateFromGroupBySelect(fromAux, secondArgument, projectionAux);
            }
            else if (secondArgument.NodeType.IsProjectionOfSelect() && projectionAux.NodeType.IsOrderBy())
            {
                this.result = this.CreateFromSelectWithoutOnlyFunctionsInProjectionOrderBy(fromAux, secondArgument, projectionAux);
            }

            this.result.NodeText = string.Format("{0} {1} {2}", fromAux.NodeText, secondArgument.NodeText, projectionAux.NodeText);
        }

        /// <summary>
        /// Helper method for query with four parts
        /// </summary>
        /// <param name="fromAux">Query source</param>
        /// <param name="secondArgument">Second argument</param>
        /// <param name="thirdArgument">Third argument</param>
        /// <param name="fourthArgument">Fourth argument</param>
        private void AuxFourParts(PlanNode fromAux, PlanNode secondArgument, PlanNode thirdArgument, PlanNode fourthArgument)
        {
            if (secondArgument.NodeType.IsWhere() && thirdArgument.NodeType.IsGroupBy() && fourthArgument.NodeType.IsProjectionOfSelect())
            {
                this.result = this.CreateFromWhereGroupBySelect(fromAux, secondArgument, thirdArgument, fourthArgument);
            }
            else if (secondArgument.NodeType.IsWhere() && thirdArgument.NodeType.IsProjectionOfSelect() && fourthArgument.NodeType.IsOrderBy())
            {
                this.result = this.CreateFromWhereSelectWithoutOnlyFunctionsInProjectionOrderBy(fromAux, secondArgument, thirdArgument, fourthArgument);
            }
            else if (secondArgument.NodeType.IsGroupBy() && thirdArgument.NodeType.IsProjectionOfSelect() && fourthArgument.NodeType.IsOrderBy())
            {
                this.result = this.CreateFromGroupBySelectOrderBy(fromAux, secondArgument, thirdArgument, fourthArgument);
            }
            
            this.result.NodeText = string.Format("{0} {1} {2} {3}", fromAux.NodeText, secondArgument.NodeText, thirdArgument.NodeText, fourthArgument.NodeText);
        }
        
        #endregion auxiliar

        #region CompletarArbolesDeEjecucion

        /// <summary>
        /// Creates the from select query execution plan. Contains DisposeEvents = true
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromSelect(PlanNode fromAux, PlanNode projectionAux)
        {
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(fromAux);

            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", int.Parse(ConfigurationManager.AppSettings["DefaultWindowSize"]));
            bufferSize.Properties.Add("DataType", typeof(int));

            buffer.Children.Add(scopeForApplyWindow);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(buffer);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForBuffer.Children = new List<PlanNode>();
            selectForBuffer.Properties.Add("ForMetadata", true);
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
            PlanNode toList = new PlanNode();
            toList.NodeType = PlanNodeTypeEnum.EnumerableToList;
            toList.Children = new List<PlanNode>();
            toList.Children.Add(selectForEnumerable);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = toList;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(toList);
            }

            /* ******************************************************************************************************************************************************** */
            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from where select query execution plan. Contains DisposeEvents = true
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereSelect(PlanNode fromAux, PlanNode whereAux, PlanNode projectionAux)
        {
            whereAux.Children.ElementAt(0).Children.Add(fromAux);

            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(whereAux);

            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", int.Parse(ConfigurationManager.AppSettings["DefaultWindowSize"]));
            bufferSize.Properties.Add("DataType", typeof(int));

            buffer.Children.Add(scopeForApplyWindow);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(buffer);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForBuffer.Children = new List<PlanNode>();
            selectForBuffer.Properties.Add("ForMetadata", true);
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
            /*projectionAux.Properties.Add("DisposeEvents", false);*/
            selectForEnumerable.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode toList = new PlanNode();
            toList.NodeType = PlanNodeTypeEnum.EnumerableToList;
            toList.Children = new List<PlanNode>();
            toList.Children.Add(selectForEnumerable);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = toList;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(toList);
            }

            /* ******************************************************************************************************************************************************** */
            return selectForBuffer;
        }
        
        /// <summary>
        /// Creates the from where select query execution plan. Contains DisposeEvents = false
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromSelectWithoutOnlyFunctionsInProjectionOrderBy(PlanNode fromAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(fromAux);

            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", int.Parse(ConfigurationManager.AppSettings["DefaultWindowSize"]));
            bufferSize.Properties.Add("DataType", typeof(int));

            buffer.Children.Add(scopeForApplyWindow);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(buffer);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForBuffer.Children = new List<PlanNode>();
            selectForBuffer.Properties.Add("ForMetadata", true);
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
            /*projectionAux.Properties.Add("DisposeEvents", false);*/
            selectForEnumerable.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            orderByAux.Children[0].Children = new List<PlanNode>();
            orderByAux.Children[0].Children.Add(selectForEnumerable);
            /* ******************************************************************************************************************************************************** */
            PlanNode toList = new PlanNode();
            toList.NodeType = PlanNodeTypeEnum.EnumerableToList;
            toList.Children = new List<PlanNode>();
            toList.Children.Add(orderByAux);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = toList;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(toList);
            }

            /* ******************************************************************************************************************************************************** */
            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from where applyWindow groupBy select query execution plan. Contains DisposeEvents = true
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereGroupBySelect(PlanNode fromAux, PlanNode whereAux, PlanNode groupByAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);
            /* ******************************************************************************************************************************************************** */
            List<PlanNode> lpn = projectionAux.FindNode(PlanNodeTypeEnum.TupleProjection);
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

            scopeSelectForBuffer.Children.Add(whereAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
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
            /*projectionAux.Properties.Add("DisposeEvents", true);*/
            selectForGroupBy.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode toList = new PlanNode();
            toList.NodeType = PlanNodeTypeEnum.EnumerableToList;
            toList.Children = new List<PlanNode>();

            toList.Children.Add(selectForGroupBy);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = toList;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(toList);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from applyWindow groupBy select query execution plan. Contains DisposeEvents = true
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromGroupBySelect(PlanNode fromAux, PlanNode groupByAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            List<PlanNode> lpn = projectionAux.FindNode(PlanNodeTypeEnum.TupleProjection);
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

            scopeSelectForBuffer.Children.Add(fromAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
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
            /*projectionAux.Properties.Add("DisposeEvents", true);*/
            selectForGroupBy.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode toList = new PlanNode();
            toList.NodeType = PlanNodeTypeEnum.EnumerableToList;
            toList.Children = new List<PlanNode>();

            toList.Children.Add(selectForGroupBy);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = toList;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(toList);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }
        
        /// <summary>
        /// Creates the from where applyWindow select query execution plan. Contains DisposeEvents = false
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="whereAux">Where plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereSelectWithoutOnlyFunctionsInProjectionOrderBy(PlanNode fromAux, PlanNode whereAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);
            /* ******************************************************************************************************************************************************** */

            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(whereAux);

            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", int.Parse(ConfigurationManager.AppSettings["DefaultWindowSize"]));
            bufferSize.Properties.Add("DataType", typeof(int));

            buffer.Children.Add(scopeForApplyWindow);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(buffer);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForBuffer.Children = new List<PlanNode>();
            selectForBuffer.Properties.Add("ForMetadata", true);
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
            /*projectionAux.Properties.Add("DisposeEvents", false);*/
            selectForEnumerable.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            orderByAux.Children[0].Children = new List<PlanNode>();
            orderByAux.Children[0].Children.Add(selectForEnumerable);
            /* ******************************************************************************************************************************************************** */
            PlanNode toList = new PlanNode();
            toList.NodeType = PlanNodeTypeEnum.EnumerableToList;
            toList.Children = new List<PlanNode>();
            toList.Children.Add(orderByAux);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = toList;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(toList);
            }

            /* ******************************************************************************************************************************************************** */
            return selectForBuffer;
        }

        /// <summary>
        /// Creates the from applyWindow groupBy select query execution plan. Contains DisposeEvents = true
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromGroupBySelectOrderBy(PlanNode fromAux, PlanNode groupByAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            /* ******************************************************************************************************************************************************** */
            List<PlanNode> lpn = projectionAux.FindNode(PlanNodeTypeEnum.TupleProjection);
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

            scopeSelectForBuffer.Children.Add(fromAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
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
            /*projectionAux.Properties.Add("DisposeEvents", true);*/
            selectForGroupBy.Children.Add(projectionAux);

            orderByAux.Children[0].Children = new List<PlanNode>();
            orderByAux.Children[0].Children.Add(selectForGroupBy);
            /* ******************************************************************************************************************************************************** */
            PlanNode toList = new PlanNode();
            toList.NodeType = PlanNodeTypeEnum.EnumerableToList;
            toList.Children = new List<PlanNode>();

            toList.Children.Add(orderByAux);
            /* ******************************************************************************************************************************************************** */
            if (projectionAux.Properties.ContainsKey(PlanNodeTypeEnum.EnumerableTake.ToString()))
            {
                PlanNode planTop = (PlanNode)projectionAux.Properties[PlanNodeTypeEnum.EnumerableTake.ToString()];
                planTop.Children[0] = toList;
                selectForBuffer.Children.Add(planTop);
            }
            else
            {
                selectForBuffer.Children.Add(toList);
            }

            /* ******************************************************************************************************************************************************** */

            return selectForBuffer;
        }
        
        #endregion
    }
}
