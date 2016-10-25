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
        private AstNodeBase identifier;

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
                this.identifier = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "IDENTIFIER_WITH_PATH", ChildrenNodes[3]) as AstNodeBase; // (string)ChildrenNodes[3].Token.Value;
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

            // gets the database if it was defined.
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);

            PermissionNode permission = null;
            if (ChildrenNodes.Count == 1)
            {
                // esto se coloca por el comando 'use' a los permisos sobre bases de datos para que tengan la base de datos especificada en el comando 'use'.
                if (!string.IsNullOrWhiteSpace(databaseName) && (permissionEnum == PermissionsEnum.AlterAnySchema || permissionEnum == PermissionsEnum.AlterAnyRole || permissionEnum == PermissionsEnum.AlterAnyUser || permissionEnum == PermissionsEnum.CreateSchema || permissionEnum == PermissionsEnum.CreateSource || permissionEnum == PermissionsEnum.CreateStream || permissionEnum == PermissionsEnum.CreateView))
                {
                    CommandObject commandObject = new CommandObject(SystemObjectEnum.Database, null, null, databaseName, PermissionsEnum.Connect, false);
                    permission = new PermissionNode(permissionEnum, commandObject);
                }
                else
                {
                    permission = new PermissionNode(permissionEnum);
                }
            }
            else if (ChildrenNodes.Count == 4)
            {
                System.Tuple<string, string, string> identifierWithPath = (System.Tuple<string, string, string>)this.identifier.Evaluate(thread);
                if (!string.IsNullOrWhiteSpace(identifierWithPath.Item1))
                {
                    databaseName = identifierWithPath.Item1;
                }

                SystemObjectEnum objectTypeAux = (SystemObjectEnum)this.objectType.Evaluate(thread);
                CommandObject commandObject = new CommandObject(objectTypeAux, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, permissionEnum, false);
                permission = new PermissionNode(permissionEnum, commandObject);
            }

            this.EndEvaluate(thread);
                        
            return permission;
        }
    }
}
