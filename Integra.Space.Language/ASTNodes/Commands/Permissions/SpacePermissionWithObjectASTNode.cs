//-----------------------------------------------------------------------
// <copyright file="SpacePermissionWithObjectASTNode.cs" company="Integra.Space.Language">
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
    internal class SpacePermissionWithObjectASTNode : AstNodeBase
    {
        /// <summary>
        /// Space object.
        /// </summary>
        private AstNodeBase spaceObject;

        /// <summary>
        /// Space object identifier.
        /// </summary>
        private AstNodeBase permission;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.permission = AddChild(NodeUseType.ValueRead, "SpacePermission", ChildrenNodes[0]) as AstNodeBase;
            this.spaceObject = AddChild(NodeUseType.ValueRead, "SpaceObject", ChildrenNodes[1]) as AstNodeBase;
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

            PermissionsEnum permission = (PermissionsEnum)this.permission.Evaluate(thread);

            System.Tuple<string, SystemObjectEnum> t = null;
            if (this.spaceObject is SpaceObjectASTNode)
            {
                SystemObjectEnum @object = (SystemObjectEnum)this.spaceObject.Evaluate(thread);
                t = System.Tuple.Create<string, SystemObjectEnum>(null, @object);
            }
            else if (this.spaceObject is SpaceObjectWithIdASTNode)
            {
                t = (System.Tuple<string, SystemObjectEnum>)this.spaceObject.Evaluate(thread);
            }

            this.EndEvaluate(thread);

            return new PermissionNode(permission, t.Item2, t.Item1);
        }
    }
}
