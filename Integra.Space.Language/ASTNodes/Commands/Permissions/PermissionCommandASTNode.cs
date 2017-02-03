//-----------------------------------------------------------------------
// <copyright file="PermissionCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class PermissionCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Space action.
        /// </summary>
        private string action;

        /// <summary>
        /// Space permission list.
        /// </summary>
        private ListASTNode<PermissionASTNode, PermissionNode> permissionList;

        /// <summary>
        /// Terminal to.
        /// </summary>
        private string terminalTo;

        /// <summary>
        /// Space user or role.
        /// </summary>
        private ListASTNode<SpaceObjectWithIdASTNode, CommandObject> principals;

        /// <summary>
        /// Permission option: "with grant option"
        /// </summary>
        private AstNodeBase permissionOption;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.action = ChildrenNodes[0].Token.Text;
            this.permissionList = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "PermissionList", ChildrenNodes[1]) as ListASTNode<PermissionASTNode, PermissionNode>;
            this.terminalTo = ChildrenNodes[2].Token.Text;
            this.principals = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "UserOrRole", ChildrenNodes[3]) as ListASTNode<SpaceObjectWithIdASTNode, CommandObject>;

            if (ChildrenNodes.Count == 5)
            {
                this.permissionOption = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "PermissionOption", ChildrenNodes[4]) as PermissionOptionASTNode;
            }
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
            List<PermissionNode> permissionListAux = (List<PermissionNode>)this.permissionList.Evaluate(thread);
            IEnumerable<CommandObject> principalsAux = (IEnumerable<CommandObject>)this.principals.Evaluate(thread);
            
            bool permissionOptionAux = false;

            if (ChildrenNodes.Count == 5)
            {
                permissionOptionAux = (bool)this.permissionOption.Evaluate(thread);
            }

            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            ActionCommandEnum actionAux;
            if (!System.Enum.TryParse(this.action, true, out actionAux))
            {
                throw new Exceptions.SyntaxException(string.Format("Invalid action {0}.", this.action));
            }

            HashSet<CommandObject> principalsHS = new HashSet<CommandObject>(new CommandObjectComparer());
            foreach (CommandObject co in principalsAux)
            {
                co.GranularPermission = PermissionsEnum.None;
                co.IsNew = false;
                principalsHS.Add(co);
            }

            List<PermissionsCommandNode> listOfPermissionCommands = new List<PermissionsCommandNode>();
            foreach (PermissionNode permission in permissionListAux)
            {
                listOfPermissionCommands.Add(new PermissionsCommandNode(actionAux, principalsHS, databaseName, permission, permissionOptionAux, this.Location.Line, this.Location.Column, this.NodeText));
            }

            return listOfPermissionCommands.ToArray();
        }
    }
}
