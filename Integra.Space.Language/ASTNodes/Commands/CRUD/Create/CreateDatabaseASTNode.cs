//-----------------------------------------------------------------------
// <copyright file="CreateDatabaseASTNode.cs" company="Integra.Space.Language">
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
    internal class CreateDatabaseASTNode : CreateCommandASTNode<DatabaseOptionEnum>
    {
        /// <summary>
        /// Options AST node.
        /// </summary>
        private CommandOptionListASTNode<DatabaseOptionEnum> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateDatabaseASTNode"/> class.
        /// </summary>
        public CreateDatabaseASTNode() : base(PermissionsEnum.CreateDatabase)
        {
            this.NotAllowedOptions.Add(DatabaseOptionEnum.Name);
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[3]) as CommandOptionListASTNode<DatabaseOptionEnum>;
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
            Dictionary<DatabaseOptionEnum, object> optionsAux = new Dictionary<DatabaseOptionEnum, object>();
            if (this.options != null)
            {
                optionsAux = (Dictionary<DatabaseOptionEnum, object>)this.options.Evaluate(thread);
            }

            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            this.CheckAllowedOptions(optionsAux, thread);

            return new CreateDatabaseNode(commandObject, optionsAux, this.Location.Line, this.Location.Column, this.NodeText);
        }
    }
}
