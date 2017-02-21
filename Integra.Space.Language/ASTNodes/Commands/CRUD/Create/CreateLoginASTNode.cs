//-----------------------------------------------------------------------
// <copyright file="CreateLoginASTNode.cs" company="Integra.Space.Language">
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
    internal class CreateLoginASTNode : CreateCommandASTNode<LoginOptionEnum>
    {
        /// <summary>
        /// Options AST node.
        /// </summary>
        private CommandOptionListASTNode<LoginOptionEnum> options;

        /// <summary>
        /// Password options AST node.
        /// </summary>
        private CommandOptionASTNode<LoginOptionEnum> passwordOption;

        /// <summary>
        /// Reserved word with.
        /// </summary>
        private string with;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateLoginASTNode"/> class.
        /// </summary>
        public CreateLoginASTNode() : base(PermissionsEnum.AlterAnyLogin)
        {
            this.NotAllowedOptions.Add(LoginOptionEnum.Name);
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

            this.passwordOption = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "PASSWORD_OPTION", ChildrenNodes[4]) as CommandOptionASTNode<LoginOptionEnum>;
            this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[5]) as CommandOptionListASTNode<LoginOptionEnum>;
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
            CommandOption<LoginOptionEnum> passwordOptionAux = (CommandOption<LoginOptionEnum>)this.passwordOption.Evaluate(thread);

            Dictionary<LoginOptionEnum, object> optionsAux = new Dictionary<LoginOptionEnum, object>();
            if (this.options != null)
            {
                optionsAux = (Dictionary<LoginOptionEnum, object>)this.options.Evaluate(thread);
            }

            this.EndEvaluate(thread);

            this.AddCommandOption(optionsAux, passwordOptionAux, thread);
            this.CheckAllowedOptions(optionsAux, thread);

            return new CreateLoginNode(commandObject, optionsAux, this.Location.Line, this.Location.Column, this.NodeText);
        }
    }
}
