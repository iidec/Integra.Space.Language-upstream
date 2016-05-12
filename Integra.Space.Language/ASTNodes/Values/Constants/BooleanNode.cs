//-----------------------------------------------------------------------
// <copyright file="BooleanNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Constants
{
    using Integra.Space.Language.ASTNodes.Base;
    
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// BooleanNode class
    /// </summary>
    internal sealed class BooleanNode : AstNodeBase
    {
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

            this.result = new PlanNode();
            this.result.Column = treeNode.Token.Location.Column;
            this.result.Line = treeNode.Token.Location.Line;
            this.result.NodeText = treeNode.Token.Text;
            this.result.NodeType = PlanNodeTypeEnum.Constant;
            this.result.Properties.Add("Value", treeNode.Token.Value);
            this.result.Properties.Add("DataType", typeof(bool));
            this.result.Properties.Add("IsConstant", true);
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            return this.result;
        }
    }
}
