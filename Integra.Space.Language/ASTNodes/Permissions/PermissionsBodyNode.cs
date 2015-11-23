//-----------------------------------------------------------------------
// <copyright file="PermissionsBodyNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Permissions
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// PermissionsBodyNode class
    /// </summary>
    internal sealed class PermissionsBodyNode : AstNodeBase
    {
        /// <summary>
        /// reserved word 'user' or 'role'
        /// </summary>
        private string userOrRoleWord;

        /// <summary>
        /// user or role name
        /// </summary>
        private AstNodeBase userORoleName;

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
            this.userOrRoleWord = (string)ChildrenNodes[0].Token.Value;
            this.userORoleName = AddChild(NodeUseType.Parameter, SR.IdentifierRole, ChildrenNodes[1]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
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
            PlanNode userOrRoleNameAux = (PlanNode)this.userORoleName.Evaluate(thread);
            this.EndEvaluate(thread);

            if (this.userOrRoleWord.ToLower() == SR.ReservedWordUser)
            {
                this.result.Properties.Add(SR.ToProperty, SR.ReservedWordUser);
            }
            else if (this.userOrRoleWord.ToLower() == SR.ReservedWordRole)
            {
                this.result.Properties.Add(SR.ToProperty, SR.ReservedWordRole);
            }

            this.result.NodeText = this.userOrRoleWord + " " + this.userORoleName;

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(userOrRoleNameAux);

            return this.result;
        }
    }
}
