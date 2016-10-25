//-----------------------------------------------------------------------
// <copyright file="CreateUserASTNode.cs" company="Integra.Space.Language">
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
    internal class CreateUserASTNode : CreateCommandASTNode<UserOptionEnum>
    {
        /// <summary>
        /// Options AST node.
        /// </summary>
        private CommandOptionListASTNode<UserOptionEnum> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateUserASTNode"/> class.
        /// </summary>
        public CreateUserASTNode() : base(PermissionsEnum.AlterAnyUser)
        {
            this.NotAllowedOptions.Add(UserOptionEnum.Name);
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[3]) as CommandOptionListASTNode<UserOptionEnum>;
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
            Dictionary<UserOptionEnum, object> optionsAux = new Dictionary<UserOptionEnum, object>();

            this.BeginEvaluate(thread);
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);

            if (this.options != null)
            {
                optionsAux = (Dictionary<UserOptionEnum, object>)this.options.Evaluate(thread);
            }

            this.CheckAllowedOptions(optionsAux);

            this.EndEvaluate(thread);
            return new CreateUserNode(commandObject, optionsAux, this.Location.Line, this.Location.Column, this.GetNodeText());
        }
    }
}
