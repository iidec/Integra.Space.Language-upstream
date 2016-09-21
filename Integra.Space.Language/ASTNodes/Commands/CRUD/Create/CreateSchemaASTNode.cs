//-----------------------------------------------------------------------
// <copyright file="CreateSchemaASTNode.cs" company="Integra.Space.Language">
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
    internal class CreateSchemaASTNode : CreateCommandASTNode<SchemaOptionEnum>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateSchemaASTNode"/> class.
        /// </summary>
        public CreateSchemaASTNode() : base(PermissionsEnum.CreateSchema)
        {
            this.NotAllowedOptions.Add(SchemaOptionEnum.Name);
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
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
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            return new CreateSchemaNode(commandObject, new Dictionary<SchemaOptionEnum, object>(), this.Location.Line, this.Location.Column, this.GetNodeText(), databaseName);
        }
    }
}
