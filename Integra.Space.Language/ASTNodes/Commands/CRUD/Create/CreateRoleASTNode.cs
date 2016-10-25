//-----------------------------------------------------------------------
// <copyright file="CreateRoleASTNode.cs" company="Integra.Space.Language">
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
    internal class CreateRoleASTNode : CreateCommandASTNode<RoleOptionEnum>
    {
        /// <summary>
        /// Options AST node.
        /// </summary>
        private CommandOptionListASTNode<RoleOptionEnum> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateRoleASTNode"/> class.
        /// </summary>
        public CreateRoleASTNode() : base(PermissionsEnum.CreateRole)
        {
            this.NotAllowedOptions.Add(RoleOptionEnum.Name);
            this.NotAllowedOptions.Add(RoleOptionEnum.Remove);
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[3]) as CommandOptionListASTNode<RoleOptionEnum>;
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
            
            this.CheckAllowedOptions(optionsAux);
            
            HashSet<CommandObject> usersAux = new HashSet<CommandObject>(new CommandObjectComparer());
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

                    if (!usersAux.Add(new CommandObject(SystemObjectEnum.DatabaseUser, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, PermissionsEnum.Control, false)))
                    {
                        throw new Exceptions.SyntaxException(string.Format("The user '{0}' is specified more than once."));
                    }
                }

                optionsAux[RoleOptionEnum.Add] = usersAux;
            }

            return new CreateRoleNode(commandObject, optionsAux, this.Location.Line, this.Location.Column, this.GetNodeText());
        }
    }
}
