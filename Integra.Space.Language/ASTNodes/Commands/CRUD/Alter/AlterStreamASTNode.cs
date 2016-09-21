﻿//-----------------------------------------------------------------------
// <copyright file="AlterStreamASTNode.cs" company="Integra.Space.Language">
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
    internal class AlterStreamASTNode : AlterCommandASTNode<StreamOptionEnum>
    {
        /// <summary>
        /// Options AST node.
        /// </summary>
        private CommandOptionListASTNode<StreamOptionEnum> options;
        
        /// <summary>
        /// Reserved word with.
        /// </summary>
        private string with;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterStreamASTNode"/> class.
        /// </summary>
        public AlterStreamASTNode() : base(PermissionsEnum.Alter)
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
            this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[4]) as CommandOptionListASTNode<StreamOptionEnum>;
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

            Dictionary<StreamOptionEnum, object> optionsAux = new Dictionary<StreamOptionEnum, object>();
            if (this.options != null)
            {
                optionsAux = (Dictionary<StreamOptionEnum, object>)this.options.Evaluate(thread);
            }

            this.EndEvaluate(thread);

            return new AlterStreamNode(commandObject, optionsAux, this.Location.Line, this.Location.Column, this.GetNodeText(), null, databaseName);
        }
    }
}
