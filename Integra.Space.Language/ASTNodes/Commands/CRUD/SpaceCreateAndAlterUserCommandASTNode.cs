//-----------------------------------------------------------------------
// <copyright file="SpaceCreateAndAlterUserCommandASTNode.cs" company="Integra.Space.Language">
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
    internal class SpaceCreateAndAlterUserCommandASTNode : AstNodeBase
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
        /// User options list.
        /// </summary>
        private ListASTNode<SpaceUserOptionASTNode, UserOption> userOptionList;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.action = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "CREATE_AND_ALTER_USER_COMMAND", ChildrenNodes[0]) as AstNodeBase;
            this.spaceObject = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "SPACE_OBJECT", ChildrenNodes[1]) as AstNodeBase;
            this.userOptionList = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "UserOptionsList", ChildrenNodes[2]) as ListASTNode<SpaceUserOptionASTNode, UserOption>;
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
            ActionCommandEnum actionAux = (ActionCommandEnum)this.action.Evaluate(thread);
            Tuple<string, SystemObjectEnum> spaceObjectAux = (Tuple<string, SystemObjectEnum>)this.spaceObject.Evaluate(thread);
            List<UserOption> userOptionListAux = (List<UserOption>)this.userOptionList.Evaluate(thread);
            this.EndEvaluate(thread);

            return new CreateAndAlterUserNode(actionAux, spaceObjectAux.Item1, userOptionListAux, this.Location.Line, this.Location.Column, this.AsString);
        }
    }
}
