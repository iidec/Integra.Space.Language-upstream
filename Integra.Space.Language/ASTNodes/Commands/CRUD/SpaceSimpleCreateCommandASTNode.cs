//-----------------------------------------------------------------------
// <copyright file="SpaceSimpleCreateCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.CommandContext;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class SpaceSimpleCreateCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Space command.
        /// </summary>
        private AstNodeBase action;

        /// <summary>
        /// Space user or role.
        /// </summary>
        private AstNodeBase spaceObject;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.action = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "CREATE_ACTION", ChildrenNodes[0]) as AstNodeBase;
            this.spaceObject = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "SPACE_OBJECT", ChildrenNodes[1]) as AstNodeBase;
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
            SpaceActionCommandEnum actionAux = (SpaceActionCommandEnum)this.action.Evaluate(thread);
            Tuple<string, SpaceObjectEnum> spaceObjectAux = (Tuple<string, SpaceObjectEnum>)this.spaceObject.Evaluate(thread);
            this.EndEvaluate(thread);

            return new CreateObjectNode(spaceObjectAux.Item2, spaceObjectAux.Item1, this.Location.Line, this.Location.Column, this.AsString);
        }
    }
}
