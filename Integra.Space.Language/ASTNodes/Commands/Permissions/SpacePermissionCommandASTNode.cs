//-----------------------------------------------------------------------
// <copyright file="SpacePermissionCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using CommandContext;
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
        private ListASTNode<SpacePermissionWithObjectGroupASTNode, Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>> permissionList;

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
            this.permissionList = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "PermissionList", ChildrenNodes[1]) as ListASTNode<SpacePermissionWithObjectGroupASTNode, Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>>;
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
            SpaceActionCommandEnum actionAux = (SpaceActionCommandEnum)this.action.Evaluate(thread);
            List<Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>> permissionListAux = (List<Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>>)this.permissionList.Evaluate(thread);
            Tuple<string, SpaceObjectEnum> userOrRoleAux = (Tuple<string, SpaceObjectEnum>)this.userOrRole.Evaluate(thread);
            this.EndEvaluate(thread);

            List<Tuple<string, SpaceObjectEnum, bool?>> spaceObjectList = new List<Tuple<string, SpaceObjectEnum, bool?>>();
            spaceObjectList.Add(Tuple.Create<string, SpaceObjectEnum, bool?>(userOrRoleAux.Item1, userOrRoleAux.Item2, null));
            this.AddObjectsUsedInTheCommand(permissionListAux, spaceObjectList);

            return new PipelineCommandContext(actionAux, spaceObjectList, permissionListAux);
        }

        /// <summary>
        /// Add the objects specified in the permissions to the space object list.
        /// </summary>
        /// <param name="permissionList">Permission list.</param>
        /// <param name="objectList">Space object list.</param>
        protected void AddObjectsUsedInTheCommand(List<Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>> permissionList, List<Tuple<string, SpaceObjectEnum, bool?>> objectList)
        {
            foreach (Tuple<SpacePermissionsEnum, SpaceObjectEnum, string> p in permissionList)
            {
                if (p.Item3 != null)
                {
                    objectList.Add(Tuple.Create<string, SpaceObjectEnum, bool?>(p.Item3, p.Item2, null));
                }
            }
        }
    }
}
