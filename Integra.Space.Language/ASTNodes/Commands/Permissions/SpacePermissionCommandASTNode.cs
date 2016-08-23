﻿//-----------------------------------------------------------------------
// <copyright file="SpacePermissionCommandASTNode.cs" company="Integra.Space.Language">
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
    internal class SpacePermissionCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Space action.
        /// </summary>
        private AstNodeBase action;

        /// <summary>
        /// Space permission list.
        /// </summary>
        private ListASTNode<SpacePermissionWithObjectGroupASTNode, PermissionNode> permissionList;

        /// <summary>
        /// Terminal to.
        /// </summary>
        private string terminalTo;

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

            this.action = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "Action", ChildrenNodes[0]) as AstNodeBase;
            this.permissionList = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "PermissionList", ChildrenNodes[1]) as ListASTNode<SpacePermissionWithObjectGroupASTNode, PermissionNode>;
            this.terminalTo = ChildrenNodes[2].Token.Text;
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
            ActionCommandEnum actionAux = (ActionCommandEnum)this.action.Evaluate(thread);
            List<PermissionNode> permissionListAux = (List<PermissionNode>)this.permissionList.Evaluate(thread);
            Tuple<string, SystemObjectEnum> userOrRoleAux = (Tuple<string, SystemObjectEnum>)this.userOrRole.Evaluate(thread);
            this.EndEvaluate(thread);
            
            return new SpacePermissionsCommandNode(actionAux, userOrRoleAux.Item2, userOrRoleAux.Item1, permissionListAux, this.Location.Line, this.Location.Column, this.AsString);
        }
    }
}
