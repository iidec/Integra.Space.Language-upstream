//-----------------------------------------------------------------------
// <copyright file="ConstantStringValueNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.General
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// UserStatusNode class
    /// </summary>
    internal sealed class ConstantStringValueNode : AstNodeBase
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
            this.result.Properties.Add(SR.ValueProperty, treeNode.Token.Value);
            this.result.Properties.Add(SR.DataTypeProperty, typeof(string).ToString());
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
