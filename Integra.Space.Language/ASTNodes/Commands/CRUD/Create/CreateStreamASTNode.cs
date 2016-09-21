//-----------------------------------------------------------------------
// <copyright file="CreateStreamASTNode.cs" company="Integra.Space.Language">
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
    internal class CreateStreamASTNode : CreateCommandASTNode<StreamOptionEnum>
    {
        /// <summary>
        /// Space query.
        /// </summary>
        private string query;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateStreamASTNode"/> class.
        /// </summary>
        public CreateStreamASTNode() : base(PermissionsEnum.CreateStream)
        {
            this.NotAllowedOptions.Add(StreamOptionEnum.Name);
            this.NotAllowedOptions.Add(StreamOptionEnum.Query);
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.query = ChildrenNodes[3].Token.Value.ToString().Trim();
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

            return new CreateStreamNode(commandObject, this.query, new Dictionary<StreamOptionEnum, object>(), this.Location.Line, this.Location.Column, this.GetNodeText(), null, databaseName);
        }
    }
}
