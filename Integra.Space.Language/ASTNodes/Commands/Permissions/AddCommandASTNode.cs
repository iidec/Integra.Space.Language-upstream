//-----------------------------------------------------------------------
// <copyright file="AddCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class AddCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Space permission list.
        /// </summary>
        private ListASTNode<SpaceObjectWithIdASTNode, Tuple<string, SystemObjectEnum>> listOfUsersOrRoles;

        /// <summary>
        /// Terminal to.
        /// </summary>
        private string terminalAdd;

        /// <summary>
        /// Space user or role.
        /// </summary>
        private AstNodeBase userOrRole;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.terminalAdd = ChildrenNodes[0].Token.Text;
            this.listOfUsersOrRoles = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "PermissionList", ChildrenNodes[1]) as ListASTNode<SpaceObjectWithIdASTNode, Tuple<string, SystemObjectEnum>>;
            this.userOrRole = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "UserOrRole", ChildrenNodes[3]) as AstNodeBase;
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
            ActionCommandEnum actionAux;
            Enum.TryParse<ActionCommandEnum>(this.terminalAdd, true, out actionAux);
            List<Tuple<string, SystemObjectEnum>> userOrRolesListAux = (List<Tuple<string, SystemObjectEnum>>)this.listOfUsersOrRoles.Evaluate(thread);
            Tuple<string, SystemObjectEnum> userOrRoleAux = (Tuple<string, SystemObjectEnum>)this.userOrRole.Evaluate(thread);
            this.EndEvaluate(thread);
            
            return new AddCommandNode(actionAux, userOrRoleAux.Item2, userOrRoleAux.Item1, userOrRolesListAux, this.Location.Line, this.Location.Column, this.AsString);
        }
    }
}
