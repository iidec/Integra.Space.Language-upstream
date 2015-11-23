//-----------------------------------------------------------------------
// <copyright file="CreateRole.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.Create
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// CreateRole class
    /// </summary>
    internal sealed class CreateRole : AstNodeBase
    {
        /// <summary>
        /// reserved word
        /// </summary>
        private string createWord;

        /// <summary>
        /// reserved word
        /// </summary>
        private string roleWord;

        /// <summary>
        /// new role name
        /// </summary>
        private AstNodeBase roleName;

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
            this.createWord = (string)ChildrenNodes[0].Token.Value;
            this.roleWord = (string)ChildrenNodes[1].Token.Value;
            this.roleName = AddChild(NodeUseType.Parameter, SR.UserDefinedObjectRole, ChildrenNodes[2]) as AstNodeBase;

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
            this.result.NodeType = PlanNodeTypeEnum.CreateRole;
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
            PlanNode roleNameAux = (PlanNode)this.roleName.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.NodeText = SR.RoleNodeText(this.createWord, this.roleWord, roleNameAux.NodeText);

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(roleNameAux);

            return this.result;
        }
    }
}
