//-----------------------------------------------------------------------
// <copyright file="AlterSourceASTNode.cs" company="Integra.Space.Language">
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
    internal class AlterSourceASTNode : AlterCommandASTNode<SourceOptionEnum>
    {
        /// <summary>
        /// Alter statements AST node.
        /// </summary>
        private AstNodeBase alterStatement;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterSourceASTNode"/> class.
        /// </summary>
        public AlterSourceASTNode() : base(PermissionsEnum.Alter)
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

            // this.with = (string)ChildrenNodes[3].Token.Value;
            // this.options = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[4]) as DictionaryCommandOptionASTNode<SourceOptionEnum>;
            this.alterStatement = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "COMMAND_OPTIONS", ChildrenNodes[3]) as AstNodeBase;
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

            var statement = this.alterStatement.Evaluate(thread);

            AlterSourceNode sourceNode = null;
            if (statement is Tuple<string, Dictionary<string, Type>>)
            {
                Tuple<string, Dictionary<string, Type>> columns = (Tuple<string, Dictionary<string, Type>>)statement;
                sourceNode = new AlterSourceNode(commandObject, columns, this.Location.Line, this.Location.Column, this.GetNodeText());
            }
            else if (statement is Dictionary<SourceOptionEnum, object>)
            {
                Dictionary<SourceOptionEnum, object> options = (Dictionary<SourceOptionEnum, object>)statement;
                sourceNode = new AlterSourceNode(commandObject, options, this.Location.Line, this.Location.Column, this.GetNodeText());
            }

            this.EndEvaluate(thread);

            return sourceNode;
        }
    }
}
