//-----------------------------------------------------------------------
// <copyright file="NullValueASTNode.cs" company="Integra.Space.Language">
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
    /// NullValueNode class
    /// </summary>
    internal sealed class NullValueASTNode : AstNodeBase
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
            this.result = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.Constant, "null");
            this.result.Properties.Add("Value", null);
            this.result.Properties.Add("IsConstant", true);
            this.result.Properties.Add("DataType", typeof(object));
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
