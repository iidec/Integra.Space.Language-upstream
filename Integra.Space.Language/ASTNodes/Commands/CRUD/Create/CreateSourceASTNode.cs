﻿//-----------------------------------------------------------------------
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
                        
            return new CreateSourceNode(commandObject, new Dictionary<SourceOptionEnum, object>(), this.Location.Line, this.Location.Column, this.GetNodeText(), null, databaseName);
        }
    }
}
