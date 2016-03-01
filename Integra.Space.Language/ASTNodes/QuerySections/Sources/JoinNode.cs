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
                this.result.NodeText = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", joinTypeAux.NodeText, joinAux.NodeText, whereJoinAux.NodeText, withAux.NodeText, whereWithAux.NodeText, onAux.NodeText, timeoutAux.NodeText, eventLifeTimeAux.NodeText);
            }
            else
            {
                this.result.Column = joinAux.Column;
                this.result.Line = joinAux.Line;
                this.result.NodeText = string.Format("{0} {1} {2} {3} {4} {5} {6}", joinAux.NodeText, whereJoinAux.NodeText, withAux.NodeText, whereWithAux.NodeText, onAux.NodeText, timeoutAux.NodeText, eventLifeTimeAux.NodeText);
            }

            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(joinAux.Children[0]);
            this.result.Children.Add(whereJoinAux);
            this.result.Children.Add(withAux.Children[0]);
            this.result.Children.Add(whereWithAux);
            this.result.Children.Add(onAux);
            this.result.Children.Add(timeoutAux);
            this.result.Children.Add(eventLifeTimeAux);

            return this.result;
        }

        /// <summary>
        /// Sets the join type to the result node
        /// </summary>
        /// <param name="joinTypeNode">Join type node</param>
        private void SetResultJoinType(PlanNode joinTypeNode)
        {
            if (PlanNodeTypeEnum.LeftJoin.ToString().Equals(joinTypeNode.NodeText, System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.LeftJoin;
            }
            else if (PlanNodeTypeEnum.RightJoin.ToString().Equals(joinTypeNode.NodeText, System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.RightJoin;
            }
            else if (PlanNodeTypeEnum.CrossJoin.ToString().Equals(joinTypeNode.NodeText, System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.CrossJoin;
            }
            else if (PlanNodeTypeEnum.InnerJoin.ToString().Equals(joinTypeNode.NodeText, System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.InnerJoin;
            }
            else
            {
                this.result.NodeType = PlanNodeTypeEnum.CrossJoin;
            }
        }
    }
}
