﻿//-----------------------------------------------------------------------
// <copyright file="ApplyExtesnsionNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.QuerySections
{
    using System.Collections.Generic;
    using System.Configuration;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ApplyWindowNode class
    /// </summary>
    internal sealed class ApplyExtesnsionNode : AstNodeBase
    {
        /// <summary>
        /// reserved word apply
        /// </summary>
        private string applyWord;

        /// <summary>
        /// reserved word
        /// </summary>
        private string extensionReservedWord;

        /// <summary>
        /// reserved word of
        /// </summary>
        private string reservedWordOf;

        /// <summary>
        /// first window size
        /// </summary>
        private AstNodeBase extensionValue;

        /// <summary>
        /// result plan
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            int childrenCount = ChildrenNodes.Count;

            this.applyWord = (string)ChildrenNodes[0].Token.Value;
            this.extensionReservedWord = (string)ChildrenNodes[1].Token.Value;
            this.reservedWordOf = (string)ChildrenNodes[2].Token.Value;

            this.extensionValue = AddChild(NodeUseType.Parameter, "ExtensionValue", ChildrenNodes[3]) as AstNodeBase;
            
            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
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
            PlanNode extensionValueAux = (PlanNode)this.extensionValue.Evaluate(thread);
            this.EndEvaluate(thread);

            PlanNodeTypeEnum nodeType;
            if (!System.Enum.TryParse<PlanNodeTypeEnum>(this.applyWord + this.extensionReservedWord, true, out nodeType))
            {
                throw new Exceptions.ParseException(string.Format("Invalid apply extension '{0}'", this.extensionReservedWord));
            }

            this.result.NodeType = nodeType;
            this.result.NodeText = this.applyWord + " " + this.extensionReservedWord + " " + this.reservedWordOf + " " + extensionValueAux.NodeText;
            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(extensionValueAux);

            return this.result;
        }
    }
}