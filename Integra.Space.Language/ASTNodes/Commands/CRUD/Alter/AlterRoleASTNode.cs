//-----------------------------------------------------------------------
// <copyright file="AlterRoleASTNode.cs" company="Integra.Space.Language">
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
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class AlterRoleASTNode : AlterCommandASTNode<RoleOptionEnum>
    {
        /// <summary>
        /// Options AST node.
        /// </summary>
        private DictionaryCommandOptionASTNode<RoleOptionEnum> options;

        /// <summary>
        /// Reserved word with.
        /// </summary>
        private string with;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterRoleASTNode"/> class.
        /// </summary>
        public AlterRoleASTNode() : base(PermissionsEnum.Alter)
        {
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.with = (string)ChildrenNodes[3].Token.Value;
            this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[4]) as DictionaryCommandOptionASTNode<RoleOptionEnum>;
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            CommandObject commandObject = (CommandObject)base.DoEvaluate(thread);
            
            this.BeginEvaluate(thread);
            Dictionary<RoleOptionEnum, object> optionsAux = new Dictionary<RoleOptionEnum, object>();
            if (this.options != null)
            {
                optionsAux = (Dictionary<RoleOptionEnum, object>)this.options.Evaluate(thread);
            }

            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            HashSet<CommandObject> usersToAdd = new HashSet<CommandObject>(new CommandObjectComparer());
            if (optionsAux.ContainsKey(RoleOptionEnum.Add))
            {
                IEnumerable<AstNode> identifiersWithPath = (IEnumerable<AstNode>)optionsAux[RoleOptionEnum.Add];
                foreach (AstNode child in identifiersWithPath)
                {
                    System.Tuple<string, string, string> identifierWithPath = (System.Tuple<string, string, string>)child.Evaluate(thread);
                    if (!string.IsNullOrWhiteSpace(identifierWithPath.Item1))
                    {
                        databaseName = identifierWithPath.Item1;
                    }

                    if (!usersToAdd.Add(new CommandObject(SystemObjectEnum.DatabaseUser, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, PermissionsEnum.None, false)))
                    {
                        throw new Exceptions.SyntaxException(string.Format("The user '{0}' is specified more than once."));
                    }
                }

                optionsAux[RoleOptionEnum.Add] = usersToAdd;
            }
            
            HashSet<CommandObject> usersToRemove = new HashSet<CommandObject>(new CommandObjectComparer());
            if (optionsAux.ContainsKey(RoleOptionEnum.Remove))
            {
                IEnumerable<AstNode> identifiersWithPath = (IEnumerable<AstNode>)optionsAux[RoleOptionEnum.Remove];
                foreach (AstNode child in identifiersWithPath)
                {
                    System.Tuple<string, string, string> identifierWithPath = (System.Tuple<string, string, string>)child.Evaluate(thread);
                    if (!string.IsNullOrWhiteSpace(identifierWithPath.Item1))
                    {
                        databaseName = identifierWithPath.Item1;
                    }

                    if (!usersToRemove.Add(new CommandObject(SystemObjectEnum.DatabaseUser, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, PermissionsEnum.None, false)))
                    {
                        throw new Exceptions.SyntaxException(string.Format("The user '{0}' is specified more than once."));
                    }
                }

                optionsAux[RoleOptionEnum.Remove] = usersToRemove;
            }
            
            return new AlterRoleNode(commandObject, optionsAux, this.Location.Line, this.Location.Column, this.GetNodeText());
        }
    }
}
