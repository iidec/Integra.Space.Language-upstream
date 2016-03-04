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

            PlanNode fromAux = (PlanNode)this.from.Evaluate(thread);
            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            int childrenCount = ChildrenNodes.Count;

            if (childrenCount == 3)
            {
                PlanNode secondArgument = (PlanNode)this.where.Evaluate(thread);
                PlanNode projectionAux = (PlanNode)this.select.Evaluate(thread);

                // two and three arguments
                if (secondArgument == null)
                {
                    this.AuxTwoParts(fromAux, projectionAux);
                }
                else
                {
                    this.AuxThreeParts(fromAux, secondArgument, projectionAux);
                }
            }
            else if (childrenCount == 6)
            {
                PlanNode secondArgument = (PlanNode)this.where.Evaluate(thread);
                PlanNode thirdArgument = (PlanNode)this.applyWindow.Evaluate(thread);
                PlanNode fourthArgument = (PlanNode)this.groupBy.Evaluate(thread);
                PlanNode fifthArgument = (PlanNode)this.select.Evaluate(thread);
                PlanNode sixthArgument = (PlanNode)this.sixth.Evaluate(thread);

                // six, five, four, and three arguments
                if (secondArgument != null && fourthArgument != null && sixthArgument != null)
                {
                    this.AuxSixParts(fromAux, secondArgument, thirdArgument, fourthArgument, fifthArgument, sixthArgument);
                }
                else if (secondArgument == null && fourthArgument != null && sixthArgument != null)
                {
                    this.AuxFiveParts(fromAux, thirdArgument, fourthArgument, fifthArgument, sixthArgument);
                }
                else if (secondArgument != null && fourthArgument == null && sixthArgument != null)
                {
                    this.AuxFiveParts(fromAux, secondArgument, thirdArgument, fifthArgument, sixthArgument);
                }
                else if (secondArgument != null && fourthArgument != null && sixthArgument == null)
                {
                    this.AuxFiveParts(fromAux, secondArgument, thirdArgument, fourthArgument, fifthArgument);
                }
                else if (secondArgument == null && fourthArgument == null && sixthArgument != null)
                {
                    this.AuxFourParts(fromAux, thirdArgument, fifthArgument, sixthArgument);
                }
                else if (secondArgument != null && fourthArgument == null && sixthArgument == null)
                {
                    this.AuxFourParts(fromAux, secondArgument, thirdArgument, fifthArgument);
                }
                else if (secondArgument == null && fourthArgument != null && sixthArgument == null)
                {
                    this.AuxFourParts(fromAux, thirdArgument, fourthArgument, fifthArgument);
                }
                else if (secondArgument == null && fourthArgument == null && sixthArgument == null)
                {
                    this.AuxThreeParts(fromAux, thirdArgument, fifthArgument);
                }
            }

            this.EndEvaluate(thread);

            this.result.Column = fromAux.Column;
            this.result.Line = fromAux.Line;
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

            PlanNode finalResult = new PlanNode();
            finalResult.NodeType = PlanNodeTypeEnum.SelectForResult;
            finalResult.Children = new List<PlanNode>();

            finalResult.Children.Add(scopeFinalResult);
            finalResult.Children.Add(lambdaForResult);

            // nodo para crear el subscribe
            PlanNode scopeSubscribe = new PlanNode();
            scopeSubscribe.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSubscribe.Children = new List<PlanNode>();
            scopeSubscribe.Children.Add(finalResult);

            PlanNode subscriptionNode = new PlanNode();
            subscriptionNode.NodeType = PlanNodeTypeEnum.Subscription;
            subscriptionNode.Children = new List<PlanNode>();
            subscriptionNode.Children.Add(scopeSubscribe);

            PlanNode observableCreateNode = new PlanNode();
            observableCreateNode.NodeType = PlanNodeTypeEnum.ObservableCreate;
            observableCreateNode.NodeText = this.result.NodeText;
            observableCreateNode.Children = new List<PlanNode>();
            observableCreateNode.Children.Add(subscriptionNode);

            // agrega algunas cosas a la compilación
            PlanNode executionPlanNode = observableCreateNode;
            TreeTransformations tf = new TreeTransformations(executionPlanNode);
            tf.Transform();

            return observableCreateNode;
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

            this.result.NodeText = string.Format("{0} {1} {2} {3}", fromAux.NodeText, secondArgument.NodeText, thirdArgument.NodeText, fourthArgument.NodeText);
        }

        /// <summary>
        /// Helper method for query with five parts
        /// </summary>
        /// <param name="fromAux">Query source</param>
        /// <param name="secondArgument">Second argument</param>
        /// <param name="thirdArgument">Third argument</param>
        /// <param name="fourthArgument">Fourth argument</param>
        /// <param name="fifthArgument">fifth argument</param>
        private void AuxFiveParts(PlanNode fromAux, PlanNode secondArgument, PlanNode thirdArgument, PlanNode fourthArgument, PlanNode fifthArgument)
        {
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

            this.result.NodeText = string.Format("{0} {1} {2} {3} {4}", fromAux.NodeText, secondArgument.NodeText, thirdArgument.NodeText, fourthArgument.NodeText, fifthArgument.NodeText);
        }

        /// <summary>
        /// Helper method for query with five parts
        /// </summary>
        /// <param name="fromAux">Query source</param>
        /// <param name="secondArgument">Second argument</param>
        /// <param name="thirdArgument">Third argument</param>
        /// <param name="fourthArgument">Fourth argument</param>
        /// <param name="fifthArgument">fifth argument</param>
        /// <param name="sixthArgument">sixth argument</param>
        private void AuxSixParts(PlanNode fromAux, PlanNode secondArgument, PlanNode thirdArgument, PlanNode fourthArgument, PlanNode fifthArgument, PlanNode sixthArgument)
        {
            this.result = this.CreateFromWhereApplyWindowGroupBySelectOrderBy(fromAux, secondArgument, thirdArgument, fourthArgument, fifthArgument, sixthArgument);

            this.result.NodeText = string.Format("{0} {1} {2} {3} {4} {5}", fromAux.NodeText, secondArgument.NodeText, thirdArgument.NodeText, fourthArgument.NodeText, fifthArgument.NodeText, sixthArgument.NodeText);
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
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForSource = new PlanNode();
            scopeSelectForSource.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForSource.Children = new List<PlanNode>();

            scopeSelectForSource.Children.Add(fromAux);

            PlanNode selectForSource = new PlanNode();
            selectForSource.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForSource.Children = new List<PlanNode>();

            selectForSource.Children.Add(scopeSelectForSource);
            /*projectionAux.Properties.Add("DisposeEvents", true);*/
            selectForSource.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(selectForSource);

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
        /// Creates the from where select query execution plan. Contains DisposeEvents = true
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
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForBuffer.Children = new List<PlanNode>();

            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /*projectionAux.Properties.Add("DisposeEvents", true);*/
            selectForBuffer.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(selectForBuffer);

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
        /// Creates the from where select query execution plan. Contains DisposeEvents = true
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="applyWindow">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowSelectWithOnlyFunctionsInProjection(PlanNode fromAux, PlanNode applyWindow, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(fromAux);

            applyWindow.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindow.Children[1] = applyWindow.Children[1].Children[0].Children[1];
            applyWindow.Children[0] = scopeForApplyWindow;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindow);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForBuffer.Children = new List<PlanNode>();
            /* ******************************************************************************************************************************************************** */
            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /*projectionAux.Properties.Add("DisposeEvents", true);*/
            selectForBuffer.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow2 = new PlanNode();
            scopeForApplyWindow2.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow2.Children = new List<PlanNode>();
            scopeForApplyWindow2.Children.Add(selectForBuffer);

            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", applyWindow.Children[1].Properties["Value"]);
            bufferSize.Properties.Add("DataType", applyWindow.Children[1].Properties["DataType"]);
            /* ******************************************************************************************************************************************************** */
            buffer.Children.Add(scopeForApplyWindow2);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */

            return buffer;
        }

        /// <summary>
        /// Creates the from where select query execution plan. Contains DisposeEvents = false
        /// </summary>
        /// <param name="fromAux">From plan node.</param>
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowSelectWithoutOnlyFunctionsInProjection(PlanNode fromAux, PlanNode applyWindowAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(fromAux);

            applyWindowAux.Children[0] = scopeForApplyWindow;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
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
            projectionAux.Properties.Add("DisposeEvents", false);
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
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowSelectWithoutOnlyFunctionsInProjectionOrderBy(PlanNode fromAux, PlanNode applyWindowAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(fromAux);

            applyWindowAux.Children[0] = scopeForApplyWindow;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
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
            projectionAux.Properties.Add("DisposeEvents", false);
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
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromWhereApplyWindowGroupBySelect(PlanNode fromAux, PlanNode whereAux, PlanNode applyWindowAux, PlanNode groupByAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            whereAux.Children.ElementAt(0).Children.Add(fromAux);

            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(whereAux);

            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            applyWindowAux.Children[0] = scopeForApplyWindow;
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

            scopeSelectForBuffer.Children.Add(applyWindowAux);

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
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowGroupBySelect(PlanNode fromAux, PlanNode applyWindowAux, PlanNode groupByAux, PlanNode projectionAux)
        {
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(fromAux);

            applyWindowAux.Children[0] = scopeForApplyWindow;
            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
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

            scopeSelectForBuffer.Children.Add(applyWindowAux);

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
        /// Creates the from where applyWindow select query execution plan. Contains DisposeEvents = true
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

            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(whereAux);

            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            applyWindowAux.Children[0] = scopeForApplyWindow;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
            selectForBuffer.Children = new List<PlanNode>();
            /* ******************************************************************************************************************************************************** */
            selectForBuffer.Children.Add(scopeSelectForBuffer);
            /*projectionAux.Properties.Add("DisposeEvents", true);*/
            selectForBuffer.Children.Add(projectionAux);
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow2 = new PlanNode();
            scopeForApplyWindow2.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow2.Children = new List<PlanNode>();
            scopeForApplyWindow2.Children.Add(selectForBuffer);

            PlanNode buffer = new PlanNode();
            buffer.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            buffer.Children = new List<PlanNode>();

            PlanNode bufferSize = new PlanNode();
            bufferSize.NodeType = PlanNodeTypeEnum.Constant;
            bufferSize.Properties.Add("Value", applyWindowAux.Children[1].Properties["Value"]);
            bufferSize.Properties.Add("DataType", applyWindowAux.Children[1].Properties["DataType"]);

            buffer.Children.Add(scopeForApplyWindow2);
            buffer.Children.Add(bufferSize);
            /* ******************************************************************************************************************************************************** */

            return buffer;
        }

        /// <summary>
        /// Creates the from where applyWindow select query execution plan. Contains DisposeEvents = false
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

            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(whereAux);

            applyWindowAux.Children[0] = scopeForApplyWindow;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
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
            projectionAux.Properties.Add("DisposeEvents", false);
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
        /// Creates the from where applyWindow select query execution plan. Contains DisposeEvents = false
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

            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(whereAux);

            applyWindowAux.Children[0] = scopeForApplyWindow;
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeSelectForBuffer = new PlanNode();
            scopeSelectForBuffer.NodeType = PlanNodeTypeEnum.NewScope;
            scopeSelectForBuffer.Children = new List<PlanNode>();

            scopeSelectForBuffer.Children.Add(applyWindowAux);

            PlanNode selectForBuffer = new PlanNode();
            selectForBuffer.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
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
            projectionAux.Properties.Add("DisposeEvents", false);
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
        /// <param name="applyWindowAux">Apply window plan node.</param>
        /// <param name="groupByAux">Group By plan node.</param>
        /// <param name="projectionAux">Projection plan node (tree).</param>
        /// <param name="orderByAux">Order by plan node.</param>
        /// <returns>Execution plan node</returns>
        private PlanNode CreateFromApplyWindowGroupBySelectOrderBy(PlanNode fromAux, PlanNode applyWindowAux, PlanNode groupByAux, PlanNode projectionAux, PlanNode orderByAux)
        {
            /* ******************************************************************************************************************************************************** */
            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(fromAux);

            applyWindowAux.Children[0] = scopeForApplyWindow;
            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
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

            scopeSelectForBuffer.Children.Add(applyWindowAux);

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

        /// <summary>
        /// Creates the from where applyWindow groupBy select query execution plan. Contains DisposeEvents = true
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

            PlanNode scopeForApplyWindow = new PlanNode();
            scopeForApplyWindow.NodeType = PlanNodeTypeEnum.NewScope;
            scopeForApplyWindow.Children = new List<PlanNode>();
            scopeForApplyWindow.Children.Add(whereAux);

            applyWindowAux.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            applyWindowAux.Children[1] = applyWindowAux.Children[1].Children[0].Children[1];
            applyWindowAux.Children[0] = scopeForApplyWindow;
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

            scopeSelectForBuffer.Children.Add(applyWindowAux);

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
