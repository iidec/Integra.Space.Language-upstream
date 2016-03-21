//-----------------------------------------------------------------------
// <copyright file="JoinNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;

    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// JoinNode class
    /// </summary>
    internal sealed class JoinNode : AstNodeBase
    {
        /// <summary>
        /// identifier node of the from node
        /// </summary>
        private AstNodeBase joinType;

        /// <summary>
        /// with node
        /// </summary>
        private AstNodeBase with;

        /// <summary>
        /// join source
        /// </summary>
        private AstNodeBase join;

        /// <summary>
        /// where condition of the 'join' statement
        /// </summary>
        private AstNodeBase whereJoin;

        /// <summary>
        /// where condition of the 'with' statement
        /// </summary>
        private AstNodeBase whereWith;

        /// <summary>
        /// on node
        /// </summary>
        private AstNodeBase on;

        /// <summary>
        /// timeout node
        /// </summary>
        private AstNodeBase timeout;

        /// <summary>
        /// event life time node
        /// </summary>
        private AstNodeBase eventLifeTime;

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
            this.joinType = AddChild(NodeUseType.Parameter, "joinType", ChildrenNodes[0]) as AstNodeBase;
            this.join = AddChild(NodeUseType.Parameter, "join", ChildrenNodes[1]) as AstNodeBase;
            this.whereJoin = AddChild(NodeUseType.Parameter, "whereJoin", ChildrenNodes[2]) as AstNodeBase;
            this.with = AddChild(NodeUseType.Parameter, "withStatement", ChildrenNodes[3]) as AstNodeBase;
            this.whereWith = AddChild(NodeUseType.Parameter, "whereWith", ChildrenNodes[4]) as AstNodeBase;
            this.on = AddChild(NodeUseType.Parameter, "on", ChildrenNodes[5]) as AstNodeBase;
            this.timeout = AddChild(NodeUseType.Parameter, "timeout", ChildrenNodes[6]) as AstNodeBase;
            this.eventLifeTime = AddChild(NodeUseType.Parameter, "eventLifeTime", ChildrenNodes[7]) as AstNodeBase;

            this.result = new PlanNode();
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
            PlanNode joinTypeAux = (PlanNode)this.joinType.Evaluate(thread);
            PlanNode joinAux = (PlanNode)this.join.Evaluate(thread);
            PlanNode whereJoinAux = (PlanNode)this.whereJoin.Evaluate(thread);
            PlanNode withAux = (PlanNode)this.with.Evaluate(thread);
            PlanNode whereWithAux = (PlanNode)this.whereWith.Evaluate(thread);
            PlanNode onAux = (PlanNode)this.on.Evaluate(thread);
            PlanNode timeoutAux = (PlanNode)this.timeout.Evaluate(thread);
            PlanNode eventLifeTimeAux = (PlanNode)this.eventLifeTime.Evaluate(thread);
            this.EndEvaluate(thread);

            if (joinTypeAux != null)
            {
                this.SetResultJoinType(joinTypeAux);
                this.result.Column = joinTypeAux.Column;
                this.result.Line = joinTypeAux.Line;
                this.result.NodeText = string.Format("{0} ", joinTypeAux.NodeText);
            }
            else
            {
                this.result.Column = joinAux.Column;
                this.result.Line = joinAux.Line;
                this.result.NodeText = string.Empty;
            }

            PlanNode refCountLeft = null;

            this.result.Children = new List<PlanNode>();
            if (whereJoinAux != null)
            {
                whereJoinAux.Children[0].Children.Add(joinAux);
                refCountLeft = this.GetPublishAndRefCount(whereJoinAux);
                this.result.NodeText = string.Format("{0} {1} {2} ", this.result.NodeText, joinAux.NodeText, whereJoinAux.NodeText);
            }
            else
            {
                refCountLeft = this.GetPublishAndRefCount(joinAux);
                this.result.NodeText = string.Format("{0} {1}", this.result.NodeText, joinAux.NodeText);
            }

            PlanNode refCountRight = null;

            if (whereWithAux != null)
            {
                whereWithAux.Children[0].Children.Add(withAux);
                refCountRight = this.GetPublishAndRefCount(whereWithAux);
                this.result.NodeText = string.Format("{0} {1} {2} ", this.result.NodeText, withAux.NodeText, whereWithAux.NodeText);
            }
            else
            {
                refCountRight = this.GetPublishAndRefCount(withAux);
                this.result.NodeText = string.Format("{0} {1} ", this.result.NodeText, withAux.NodeText);
            }
            
            this.result.NodeText = string.Format("{0} {1} ", this.result.NodeText, timeoutAux.NodeText);

            PlanNode sourcesNewScope = new PlanNode();
            sourcesNewScope.NodeType = PlanNodeTypeEnum.NewScope;
            sourcesNewScope.Properties.Add("ScopeParameters", new ScopeParameter[] { new ScopeParameter(0, null), new ScopeParameter(1, null) });
            sourcesNewScope.Children = new List<PlanNode>();
            sourcesNewScope.Children.Add(refCountLeft);
            sourcesNewScope.Children.Add(refCountRight);

            this.result.Children.Add(sourcesNewScope);
            onAux.NodeType = PlanNodeTypeEnum.On;
            this.result.Children.Add(onAux);
            this.result.Children.Add(timeoutAux);

            if (eventLifeTimeAux != null)
            {
                this.result.Children.Add(eventLifeTimeAux);
                this.result.NodeText = string.Format("{0} {1} ", this.result.NodeText, eventLifeTimeAux.NodeText);
            }

            return this.result;
        }

        /// <summary>
        /// Sets the join type to the result node
        /// </summary>
        /// <param name="joinTypeNode">Join type node</param>
        private void SetResultJoinType(PlanNode joinTypeNode)
        {
            if (PlanNodeTypeEnum.LeftJoin.ToString().Equals(joinTypeNode.NodeText + "join", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.LeftJoin;
            }
            else if (PlanNodeTypeEnum.RightJoin.ToString().Equals(joinTypeNode.NodeText + "join", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.RightJoin;
            }
            else if (PlanNodeTypeEnum.CrossJoin.ToString().Equals(joinTypeNode.NodeText + "join", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.CrossJoin;
            }
            else if (PlanNodeTypeEnum.InnerJoin.ToString().Equals(joinTypeNode.NodeText + "join", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.InnerJoin;
            }
            else
            {
                this.result.NodeType = PlanNodeTypeEnum.CrossJoin;
            }
        }

        /// <summary>
        /// Creates the nods for publish and ref-count.
        /// </summary>
        /// <param name="child">Child for this branch.</param>
        /// <returns>Publish and ref-count execution plan.</returns>
        private PlanNode GetPublishAndRefCount(PlanNode child)
        {
            PlanNode publish = new PlanNode();
            publish.NodeType = PlanNodeTypeEnum.ObservablePublish;
            publish.Children = new List<PlanNode>();
            publish.Children.Add(this.GetApplyWindow(child));

            PlanNode refCount = new PlanNode();
            refCount.NodeType = PlanNodeTypeEnum.ObservableRefCount;
            refCount.Children = new List<PlanNode>();
            refCount.Children.Add(publish);

            return refCount;
        }

        /// <summary>
        /// Creates the nodes for apply window of 500 milliseconds.
        /// </summary>
        /// <param name="child">Child for this branch.</param>
        /// <returns>Apply window execution plan.</returns>
        private PlanNode GetApplyWindow(PlanNode child)
        {
            PlanNode result = new PlanNode();
            result.NodeType = PlanNodeTypeEnum.ObservableBufferTimeAndSize;
            result.Properties.Add("internallyGenerated", true);
            result.Children = new List<PlanNode>();
            result.Children.Add(child);

            PlanNode planProjection = new PlanNode();
            planProjection.NodeType = PlanNodeTypeEnum.ProjectionOfConstants;
            planProjection.Properties.Add("OverrideGetHashCodeMethod", false);
            planProjection.Children = new List<PlanNode>();

            PlanNode planTuple1 = new PlanNode();
            planTuple1.NodeType = PlanNodeTypeEnum.TupleProjection;
            planTuple1.Children = new List<PlanNode>();

            PlanNode alias1 = new PlanNode();
            alias1.NodeType = PlanNodeTypeEnum.Constant;
            alias1.Properties.Add("Value", "TimeSpanValue");
            alias1.Properties.Add("DataType", typeof(object).ToString());

            PlanNode windowSize = new PlanNode();
            windowSize.NodeType = PlanNodeTypeEnum.Constant;
            windowSize.Properties.Add("Value", System.TimeSpan.FromMilliseconds(500));
            windowSize.Properties.Add("DataType", typeof(System.TimeSpan).ToString());

            planTuple1.Children.Add(alias1);
            planTuple1.Children.Add(windowSize);

            PlanNode planTuple2 = new PlanNode();
            planTuple2.NodeType = PlanNodeTypeEnum.TupleProjection;
            planTuple2.Children = new List<PlanNode>();

            PlanNode alias2 = new PlanNode();
            alias2.NodeType = PlanNodeTypeEnum.Constant;
            alias2.Properties.Add("Value", "IntegerValue");
            alias2.Properties.Add("DataType", typeof(object).ToString());

            PlanNode maxWindowSize = new PlanNode();
            maxWindowSize.NodeType = PlanNodeTypeEnum.Constant;
            maxWindowSize.Properties.Add("Value", int.Parse(System.Configuration.ConfigurationManager.AppSettings["MaxWindowSize"]));
            maxWindowSize.Properties.Add("DataType", typeof(int));

            planTuple2.Children.Add(alias2);
            planTuple2.Children.Add(maxWindowSize);

            planProjection.Children.Add(planTuple1);
            planProjection.Children.Add(planTuple2);
            
            result.Children.Add(planProjection);

            return result;
        }
    }
}
