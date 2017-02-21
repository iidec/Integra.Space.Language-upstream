//-----------------------------------------------------------------------
// <copyright file="CreateSourceASTNode.cs" company="Integra.Space.Language">
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
    internal class CreateSourceASTNode : CreateCommandASTNode<SourceOptionEnum>
    {
        /// <summary>
        /// Options AST node.
        /// </summary>
        private CommandOptionListASTNode<SourceOptionEnum> options;

        /// <summary>
        /// Source column list.
        /// </summary>
        private ListASTNode<SourceColumnsASTNode, SourceColumnNode> columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSourceASTNode"/> class.
        /// </summary>
        public CreateSourceASTNode() : base(PermissionsEnum.CreateSource)
        {
            this.NotAllowedOptions.Add(SourceOptionEnum.Name);
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.columns = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "SOURCE_COLUMNS", ChildrenNodes[3]) as ListASTNode<SourceColumnsASTNode, SourceColumnNode>;
            this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[4]) as CommandOptionListASTNode<SourceOptionEnum>;
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
            List<SourceColumnNode> columnsAux = (List<SourceColumnNode>)this.columns.Evaluate(thread);

            Dictionary<SourceOptionEnum, object> optionsAux = new Dictionary<SourceOptionEnum, object>();
            if (this.options != null)
            {
                optionsAux = (Dictionary<SourceOptionEnum, object>)this.options.Evaluate(thread);
            }

            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            this.CheckAllowedOptions(optionsAux, thread);

            return new CreateSourceNode(commandObject, columnsAux, optionsAux, this.Location.Line, this.Location.Column, this.NodeText);
        }
    }
}
