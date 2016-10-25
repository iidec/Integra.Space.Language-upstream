//-----------------------------------------------------------------------
// <copyright file="SourceForMetadataASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.MetadataQuery
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Integra.Space.Language.ASTNodes.Base;    
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// FromNode class
    /// </summary>
    internal class SourceForMetadataASTNode : AstNodeBase
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
        /// Initializes a new instance of the <see cref="SourceForMetadataASTNode"/> class.
        /// </summary>
        /// <param name="sourcePosition">Position of the source in the query.</param>
        public SourceForMetadataASTNode(int sourcePosition)
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
            this.idFromNode = AddChild(NodeUseType.Parameter, "listOfValues", ChildrenNodes[3]) as AstNodeBase;

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
                idFrom.Children[1].Properties["DataType"] = typeof(string);
                this.result.Children.Add(idFrom.Children[1]);
            }
            else
            {
                idFrom.Properties["DataType"] = typeof(string);
                this.result.Children.Add(idFrom);
            }

            if (idFrom.Children[0].Properties["Value"].ToString().Equals("servers", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.Server>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("endpoints", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.Endpoint>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("logins", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.Login>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("serverroles", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.ServerRole>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("databases", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.Database>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("users", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.DatabaseUser>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("databaseroles", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.DatabaseRole>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("schemas", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.Schema>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("sources", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.Source>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("streams", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.Stream>));
            }
            else if (idFrom.Children[0].Properties["Value"].ToString().Equals("views", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.Properties.Add("SourceType", typeof(System.IObservable<Database.View>));
            }
            else
            {
                throw new Exceptions.SyntaxException("Invalid source name. Valid sources are: 'Servers'.");
            }

            this.result.Properties.Add("SourceName", idFrom.Children[0].Properties["Value"]);
            this.result.NodeText = this.GetNodeText();
            
            return this.result;
        }
    }
}
