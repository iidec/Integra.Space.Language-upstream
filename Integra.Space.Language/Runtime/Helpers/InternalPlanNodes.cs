//-----------------------------------------------------------------------
// <copyright file="InternalPlanNodes.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Internal plan nodes class
    /// </summary>
    internal sealed class InternalPlanNodes
    {
        /// <summary>
        /// Gets the execution plan for the duration of each event at the Observable.Join with sending the result timeout to the observer.
        /// </summary>
        /// <param name="timespanValue">Timespan value to timeout.</param>
        /// <param name="parameterPosition">Name of the incoming parameter of the duration section.</param>
        /// <param name="projectionType">Projection type.</param>
        /// <returns>Duration with sending results plan.</returns>
        public PlanNode DurationWithSendingResults(PlanNode timespanValue, int parameterPosition, PlanNodeTypeEnum projectionType)
        {
            /* INICIA NODOS NECESARIOS LEFT DURATION */
            /******************************************************************************************************************************************/
            PlanNode never = new PlanNode();
            never.NodeType = PlanNodeTypeEnum.ObservableNever;
            never.Children = new List<PlanNode>();

            PlanNode timeout = new PlanNode();
            timeout.NodeType = PlanNodeTypeEnum.ObservableTimeout;
            timeout.Properties.Add("ReturnObservable", false);
            timeout.Children = new System.Collections.Generic.List<PlanNode>();
            timeout.Children.Add(never);
            timeout.Children.Add(timespanValue);
            /******************************************************************************************************************************************/
            PlanNode fromForLambdaLeftWhere = new PlanNode();
            fromForLambdaLeftWhere.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;
            fromForLambdaLeftWhere.Properties.Add("ParameterPosition", parameterPosition);

            PlanNode getMatchedProperty = new PlanNode();
            getMatchedProperty.NodeType = PlanNodeTypeEnum.Property;
            getMatchedProperty.Properties.Add("Property", "Matched");
            getMatchedProperty.Children = new List<PlanNode>();
            getMatchedProperty.Children.Add(fromForLambdaLeftWhere);

            PlanNode negate = new PlanNode();
            negate.NodeType = PlanNodeTypeEnum.Not;
            negate.Children = new List<PlanNode>();
            negate.Children.Add(getMatchedProperty);
            /******************************************************************************************************************************************/
            PlanNode fromForLambdaLeft1 = new PlanNode();
            fromForLambdaLeft1.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;
            fromForLambdaLeft1.Properties.Add("ParameterGenericType", typeof(IList<>));
            if (projectionType.Equals(PlanNodeTypeEnum.JoinLeftDuration))
            {
                fromForLambdaLeft1.Properties.Add("index", 0);
            }
            else if (projectionType.Equals(PlanNodeTypeEnum.JoinRightDuration))
            {
                fromForLambdaLeft1.Properties.Add("index", 1);
            }

            PlanNode newScopeLeftWhere = new PlanNode();
            newScopeLeftWhere.NodeType = PlanNodeTypeEnum.NewScope;
            newScopeLeftWhere.Properties.Add("ScopeParameters", new ScopeParameter[] { new ScopeParameter(parameterPosition, null) });
            newScopeLeftWhere.Children = new List<PlanNode>();
            newScopeLeftWhere.Children.Add(fromForLambdaLeft1);

            PlanNode leftWhere = new PlanNode();
            leftWhere.NodeType = PlanNodeTypeEnum.EnumerableWhere;
            leftWhere.Children = new List<PlanNode>();
            leftWhere.Children.Add(newScopeLeftWhere);
            leftWhere.Children.Add(negate);
            /******************************************************************************************************************************************/
            PlanNode newScopenumerableLeftSelect = new PlanNode();
            newScopenumerableLeftSelect.NodeType = PlanNodeTypeEnum.NewScope;
            newScopenumerableLeftSelect.Properties.Add("ScopeParameters", new ScopeParameter[] { new ScopeParameter(parameterPosition, null) });
            newScopenumerableLeftSelect.Children = new List<PlanNode>();
            newScopenumerableLeftSelect.Children.Add(leftWhere);

            PlanNode fromForLambdaLeftSelect = new PlanNode();
            fromForLambdaLeftSelect.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;
            fromForLambdaLeftSelect.Properties.Add("ParameterPosition", parameterPosition);

            PlanNode leftDurationProjection = new PlanNode();
            leftDurationProjection.NodeType = PlanNodeTypeEnum.JoinProjection;
            leftDurationProjection.Properties.Add("ProjectionType", projectionType);
            leftDurationProjection.Children = new List<PlanNode>();
            leftDurationProjection.Children.Add(fromForLambdaLeftSelect);
            /******************************************************************************************************************************************/
            PlanNode leftSelect = new PlanNode();
            leftSelect.NodeType = PlanNodeTypeEnum.EnumerableSelectForEnumerable;
            leftSelect.Children = new List<PlanNode>();
            leftSelect.Children.Add(newScopenumerableLeftSelect);
            leftSelect.Children.Add(leftDurationProjection);

            PlanNode toArray = new PlanNode();
            toArray.NodeType = PlanNodeTypeEnum.EnumerableToArray;
            toArray.Children = new List<PlanNode>();
            toArray.Children.Add(leftSelect);

            PlanNode toObservable = new PlanNode();
            toObservable.NodeType = PlanNodeTypeEnum.EnumerableToObservable;
            toObservable.Children = new List<PlanNode>();
            toObservable.Children.Add(toArray);
            /******************************************************************************************************************************************/
            PlanNode newScope1 = new PlanNode();
            newScope1.NodeType = PlanNodeTypeEnum.NewScope;
            newScope1.Properties.Add("ScopeParameters", new ScopeParameter[] { new ScopeParameter(2, typeof(TimeoutException)) });
            newScope1.Children = new List<PlanNode>();
            newScope1.Children.Add(timeout);

            PlanNode catchNode = new PlanNode();
            catchNode.NodeType = PlanNodeTypeEnum.ObservableCatch;
            catchNode.Children = new System.Collections.Generic.List<PlanNode>();
            catchNode.Children.Add(newScope1);
            catchNode.Children.Add(toObservable);
            /******************************************************************************************************************************************/
            /* TERMINA NODOS NECESARIOS LEFT DURATION */

            return catchNode;
        }

        /// <summary>
        /// Gets the execution plan for the duration of each event at the Observable.Join without sending the result timeout to the observer.
        /// </summary>
        /// <param name="timespanValue">Timespan value to timeout.</param>
        /// <returns>Duration with sending results plan.</returns>
        public PlanNode DurationWithoutSendingResults(PlanNode timespanValue)
        {
            PlanNode never = new PlanNode();
            never.NodeType = PlanNodeTypeEnum.ObservableNever;

            PlanNode timeout = new PlanNode();
            timeout.NodeType = PlanNodeTypeEnum.ObservableTimeout;
            timeout.Properties.Add("ReturnObservable", true);
            timeout.Children = new System.Collections.Generic.List<PlanNode>();
            timeout.Children.Add(never);
            timeout.Children.Add(timespanValue);

            return timeout;
        }
    }
}
