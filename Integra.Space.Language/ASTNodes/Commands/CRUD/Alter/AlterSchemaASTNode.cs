//-----------------------------------------------------------------------
// <copyright file="AlterSchemaASTNode.cs" company="Integra.Space.Language">
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
    internal class AlterSchemaASTNode : AlterCommandASTNode<SchemaOptionEnum>
    {
        /// <summary>
        /// Space object.
        /// </summary>
        private string spaceUserOption;

        /// <summary>
        /// Space object identifier.
        /// </summary>
        private object optionValue;

        /// <summary>
        /// Reserved word with.
        /// </summary>
        private string with;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterSchemaASTNode"/> class.
        /// </summary>
        public AlterSchemaASTNode() : base(PermissionsEnum.Alter)
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
            this.spaceUserOption = (string)ChildrenNodes[4].Token.Value;
            this.optionValue = ChildrenNodes[6].Token.Value;
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
            Dictionary<SchemaOptionEnum, object> optionsAux = new Dictionary<SchemaOptionEnum, object>();
            CommandOption<SchemaOptionEnum> commandOption = null;
            SchemaOptionEnum userOptionAux;
            if (System.Enum.TryParse(this.spaceUserOption, true, out userOptionAux) && this.optionValue != null)
            {
                commandOption = new CommandOption<SchemaOptionEnum>(userOptionAux, this.optionValue);
            }
            else
            {
                throw new Exceptions.SyntaxException(string.Format("Invalid user option {0}.", this.spaceUserOption));
            }
            
            this.AddCommandOption(optionsAux, commandOption);

            this.BeginEvaluate(thread);
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            return new AlterSchemaNone(commandObject, optionsAux, this.Location.Line, this.Location.Column, this.GetNodeText(), databaseName);
        }
    }
}
