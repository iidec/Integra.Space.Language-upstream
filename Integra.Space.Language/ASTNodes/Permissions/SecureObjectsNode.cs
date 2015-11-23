//-----------------------------------------------------------------------
// <copyright file="SecureObjectsNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Permissions
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// SecureObjects class
    /// </summary>
    internal sealed class SecureObjectsNode : AstNodeBase
    {
        /// <summary>
        /// secure object: role, stream, server role
        /// </summary>
        private string secureObject;

        /// <summary>
        /// result planNode
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

            this.secureObject = (string)ChildrenNodes[0].Token.Value;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeText = treeNode.Token.Text;
            this.result.Properties.Add("Value", treeNode.Token.Value);
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            this.result.NodeType = PlanNodeTypeEnum.Constant;
            this.result.Properties.Add("DataType", typeof(object).ToString());

            return this.result;
        }
    }
}
