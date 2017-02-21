//-----------------------------------------------------------------------
// <copyright file="InsertASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Insert AST node class.
    /// </summary>
    internal class InsertASTNode : AstNodeBase
    {
        /// <summary>
        /// Source identifier.
        /// </summary>
        private AstNodeBase sourceIdentifier;

        /// <summary>
        /// Source column structure.
        /// </summary>
        private StatementListNode columnStructure;

        /// <summary>
        /// List of values.
        /// </summary>
        private StatementListNode listOfValues;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.sourceIdentifier = AddChild(NodeUseType.ValueRead, "IDENTIFIER_WITH_PATH", ChildrenNodes[2]) as AstNodeBase;
            this.columnStructure = AddChild(NodeUseType.ValueRead, "COLUMN_STRUCTURE", ChildrenNodes[3]) as StatementListNode;
            this.listOfValues = AddChild(NodeUseType.ValueRead, "LIST_OF_VALUES", ChildrenNodes[5]) as StatementListNode;
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            this.BeginEvaluate(thread);

            System.Tuple<string, string, string> sourceIdentifierWithPath = (System.Tuple<string, string, string>)this.sourceIdentifier.Evaluate(thread);
            Common.CommandObject sourceObject = new Common.CommandObject(Common.SystemObjectEnum.Source, sourceIdentifierWithPath.Item1, sourceIdentifierWithPath.Item2, sourceIdentifierWithPath.Item3, Common.PermissionsEnum.Write, false);

            Dictionary<string, object> columnsWithValues = new Dictionary<string, object>();
            System.Tuple<string, string, string> identifierWithPath;
            Queue<string> columnNameQueue = new Queue<string>();
            foreach (AstNode child in this.columnStructure.GetChildNodes())
            {
                identifierWithPath = (System.Tuple<string, string, string>)child.Evaluate(thread);
                columnNameQueue.Enqueue(identifierWithPath.Item3);
            }

            Queue<object> valueQueue = new Queue<object>();
            foreach (AstNode child in this.listOfValues.GetChildNodes())
            {
                valueQueue.Enqueue(((PlanNode)child.Evaluate(thread)).Properties["Value"]);
            }

            foreach (string column in columnNameQueue)
            {
                if (columnsWithValues.ContainsKey(column))
                {
                    thread.App.Parser.Context.AddParserError(Resources.ParseResults.DuplicateColumn((int)ResultCodes.DuplicateColumn, column));
                }

                columnsWithValues.Add(column, valueQueue.Dequeue());
            }

            this.EndEvaluate(thread);

            return new InsertNode(sourceObject, columnsWithValues, this.Location.Line, this.Location.Column, this.NodeText);
        }
    }
}
