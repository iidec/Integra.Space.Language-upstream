//-----------------------------------------------------------------------
// <copyright file="EngineActionsNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Engine actions node
    /// </summary>
    internal sealed class EngineActionsNode : AstNodeBase
    {
        /// <summary>
        /// reserved word 'boot'
        /// </summary>
        private string actionWord;

        /// <summary>
        /// reserved word 'engine'
        /// </summary>
        private string engineWord;

        /// <summary>
        /// result plan node
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

            this.actionWord = (string)ChildrenNodes[0].Token.Value;
            this.engineWord = (string)ChildrenNodes[1].Token.Value;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeText = this.actionWord + " " + this.engineWord;
            this.result.NodeType = PlanNodeTypeEnum.BootEngine;
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
            this.EndEvaluate(thread);

            this.result.Properties.Add("Action", this.actionWord);
            this.result.Properties.Add("ObjectType", this.engineWord);

            return this.result;
        }
    }
}
