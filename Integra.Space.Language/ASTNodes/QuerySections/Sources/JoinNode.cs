//-----------------------------------------------------------------------
// <copyright file="JoinNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using System.Collections.Generic;
    using System.Linq;
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
        /// Get the plan node for the where statement that optimize the buffers send to the observable join.
        /// </summary>
        /// <param name="child">Child for this branch.</param>
        /// <returns>Where branch statement that optimize the buffers send to the observable join.</returns>
        private PlanNode GetWhereForOptimization(PlanNode child)
        {
            PlanNode newScope = new PlanNode();
            newScope.NodeType = PlanNodeTypeEnum.NewScope;
            newScope.Children = new List<PlanNode>();
            newScope.Children.Add(child);

            PlanNode where = new PlanNode();
            where.NodeType = PlanNodeTypeEnum.ObservableWhere;
            where.Children = new List<PlanNode>();

            PlanNode getParam = new PlanNode();
            getParam.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;
            getParam.Properties.Add("SourceName", child.FindNode(PlanNodeTypeEnum.Identifier).Single().Properties["Value"]);
            getParam.Properties.Add("ParameterPosition", 0);

            PlanNode getProperty = new PlanNode();
            getProperty.NodeType = PlanNodeTypeEnum.Property;
            getProperty.Properties.Add("Property", "Count");
            getProperty.Properties.Add("InternalUse", true);
            getProperty.Properties.Add("FromInterface", "ICollection`1");
            getProperty.Properties.Add("DataType", typeof(int).ToString());
            getProperty.Children = new List<PlanNode>();
            getProperty.Children.Add(getParam);

            PlanNode greaterThan = new PlanNode();
            greaterThan.NodeType = PlanNodeTypeEnum.GreaterThan;
            greaterThan.Children = new List<PlanNode>();

            PlanNode constant = new PlanNode();
            constant.NodeType = PlanNodeTypeEnum.Constant;
            constant.Properties.Add("Value", 0);
            constant.Properties.Add("DataType", typeof(int));
            constant.Properties.Add("IsConstant", true);

            greaterThan.Children.Add(getProperty);
            greaterThan.Children.Add(constant);
            
            where.Children.Add(newScope);
            where.Children.Add(greaterThan);

            return where;
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
            publish.Children.Add(this.GetWhereForOptimization(this.GetApplyWindow(child)));

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
            // este solo se coloca porque se hace llama a PopScope en la compilación.
            PlanNode newScope = new PlanNode();
            newScope.NodeType = PlanNodeTypeEnum.NewScope;
            newScope.Children = new List<PlanNode>();
            newScope.Children.Add(child);

            PlanNode result = new PlanNode();
            result.NodeType = PlanNodeTypeEnum.ObservableBuffer;
            result.Properties.Add("internallyGenerated", true);
            result.Children = new List<PlanNode>();
            result.Children.Add(newScope);

            PlanNode bufferSizeForJoin = new PlanNode();
            bufferSizeForJoin.NodeType = PlanNodeTypeEnum.BufferSizeForJoin;
            bufferSizeForJoin.Children = new List<PlanNode>();

            result.Children.Add(bufferSizeForJoin);

            PlanNode windowSize = new PlanNode();
            windowSize.NodeType = PlanNodeTypeEnum.Constant;
            windowSize.Properties.Add("Value", int.Parse(System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]));
            windowSize.Properties.Add("DataType", typeof(int));

            bufferSizeForJoin.Children.Add(windowSize);

            return result;
        }
    }
}
