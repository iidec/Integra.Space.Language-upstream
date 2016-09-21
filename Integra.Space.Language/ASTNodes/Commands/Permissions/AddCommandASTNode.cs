//-----------------------------------------------------------------------
// <copyright file="AddCommandASTNode.cs" company="Integra.Space.Language">
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
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class AddCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Space permission list.
        /// </summary>
        private StatementListNode users;

        /// <summary>
        /// Terminal to.
        /// </summary>
        private string terminalAdd;

        /// <summary>
        /// Space user or role.
        /// </summary>
        private StatementListNode roles;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.terminalAdd = ChildrenNodes[0].Token.Text;
            this.users = AddChild(NodeUseType.ValueRead, "Users", ChildrenNodes[1]) as StatementListNode;
            this.roles = AddChild(NodeUseType.ValueRead, "Roles", ChildrenNodes[3]) as StatementListNode;
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

            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);

            ActionCommandEnum actionAux;
            Enum.TryParse<ActionCommandEnum>(this.terminalAdd, true, out actionAux);

            HashSet<CommandObject> usersAux = new HashSet<CommandObject>(new CommandObjectComparer());
            foreach (IdentifierNode child in this.users.GetChildNodes())
            {
                if (!usersAux.Add(new CommandObject(SystemObjectEnum.DatabaseUser, child.Symbol, PermissionsEnum.Control, false)))
                {
                    throw new Exceptions.SyntaxException(string.Format("The user '{0}' is specified more than once."));
                }
            }

            HashSet<CommandObject> rolesAux = new HashSet<CommandObject>(new CommandObjectComparer());
            foreach (IdentifierNode child in this.roles.GetChildNodes())
            {
                if (!rolesAux.Add(new CommandObject(SystemObjectEnum.DatabaseRole, child.Symbol, PermissionsEnum.Control, false)))
                {
                    throw new Exceptions.SyntaxException(string.Format("The role '{0}' is specified more than once."));
                }
            }
            
            this.EndEvaluate(thread);

            return new AddCommandNode(actionAux, rolesAux, usersAux, this.Location.Line, this.Location.Column, this.GetNodeText(), null, databaseName);
        }
    }
}
