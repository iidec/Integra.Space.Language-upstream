﻿//-----------------------------------------------------------------------
// <copyright file="SpacePermissionWithObjectGroupASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Space object AST node class.
    /// </summary>
    internal class SpacePermissionWithObjectGroupASTNode : AstNodeBase
    {
        /// <summary>
        /// Space object identifier.
        /// </summary>
        private AstNodeBase definedPermission;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.definedPermission = AddChild(NodeUseType.ValueRead, "SpaceDefinePermission", ChildrenNodes[0]) as AstNodeBase;
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
            SpacePermission definedPermissionAux = (SpacePermission)this.definedPermission.Evaluate(thread);
            this.EndEvaluate(thread);

            return definedPermissionAux;
        }
    }
}
