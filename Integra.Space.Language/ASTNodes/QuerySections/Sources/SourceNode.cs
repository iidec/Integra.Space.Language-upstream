//-----------------------------------------------------------------------
// <copyright file="SourceNode.cs" company="Integra.Space.Language">
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
    /// FromNode class
    /// </summary>
    internal class SourceNode : AstNodeBase
    {
        /// <summary>
        /// identifier node of the from node
        /// </summary>
        private AstNodeBase idFromNode;

        /// <summary>
        /// reserved word 'from'
        /// </summary>
        private string from;

        /// <summary>
        /// result plan
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceNode"/> class.
        /// </summary>
        /// <param name="sourcePosition">Position of the source in the query.</param>
        public SourceNode(int sourcePosition)
        {
            this.result = new PlanNode();
            this.result.Properties.Add("SourcePosition", sourcePosition);
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.from = (string)ChildrenNodes[0].Token.Value;
            this.idFromNode = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[1]) as AstNodeBase;

            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.ObservableFrom;
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
            PlanNode idFrom = (PlanNode)this.idFromNode.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.Children = new List<PlanNode>();
            if (idFrom.NodeType.Equals(PlanNodeTypeEnum.ValueWithAlias))
            {
                idFrom.Children[1].Properties["DataType"] = typeof(string).ToString();
                this.result.Children.Add(idFrom.Children[1]);
            }
            else
            {
                idFrom.Properties["DataType"] = typeof(string).ToString();
                this.result.Children.Add(idFrom);
            }

            this.result.NodeText = string.Format("{0} {1}", this.from, idFrom.NodeText);
            
            PlanNode scopeWhereForEventLock = new PlanNode();
            scopeWhereForEventLock.NodeType = PlanNodeTypeEnum.NewScope;
            scopeWhereForEventLock.Children = new List<PlanNode>();

            scopeWhereForEventLock.Children.Add(this.result);

            PlanNode whereForEventLock = new PlanNode();
            whereForEventLock.NodeType = PlanNodeTypeEnum.ObservableWhereForEventLock;
            whereForEventLock.Properties.Add("Source", idFrom.Children.Last().Properties["Value"]);
            whereForEventLock.NodeText = this.result.NodeText;
            whereForEventLock.Children = new List<PlanNode>();
            whereForEventLock.Children.Add(scopeWhereForEventLock);

            return whereForEventLock;
        }
    }
}
