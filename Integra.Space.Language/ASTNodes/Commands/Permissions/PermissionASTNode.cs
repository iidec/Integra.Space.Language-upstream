//-----------------------------------------------------------------------
// <copyright file="PermissionASTNode.cs" company="Integra.Space.Language">
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
    internal class PermissionASTNode : AstNodeBase
    {
        /// <summary>
        /// Granular permission node.
        /// </summary>
        private AstNodeBase granularPermission;

        /// <summary>
        /// Space object type.
        /// </summary>
        private AstNodeBase objectType;

        /// <summary>
        /// Reserved word on.
        /// </summary>
        private string terminalOn;

        /// <summary>
        /// Object identifier.
        /// </summary>
        private string identifier;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.granularPermission = AddChild(NodeUseType.ValueRead, "GranularPermissionForOn", ChildrenNodes[0]) as GranularPermissionASTNode;

            if (ChildrenNodes.Count == 4)
            {
                this.terminalOn = (string)ChildrenNodes[1].Token.Value;
                this.objectType = AddChild(NodeUseType.ValueRead, "SpaceObjectType", ChildrenNodes[2]) as SpaceObjectASTNode;
                this.identifier = (string)ChildrenNodes[3].Token.Value;
            }
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
            PermissionsEnum permissionEnum = (PermissionsEnum)this.granularPermission.Evaluate(thread);

            PermissionNode permission = null;
            if (ChildrenNodes.Count == 1)
            {
                permission = new PermissionNode(permissionEnum);
            }
            else if (ChildrenNodes.Count == 4)
            {
                SystemObjectEnum objectTypeAux = (SystemObjectEnum)this.objectType.Evaluate(thread);
                permission = new PermissionNode(permissionEnum, objectTypeAux, this.identifier);
            }

            this.EndEvaluate(thread);
                        
            return permission;
        }
    }
}
