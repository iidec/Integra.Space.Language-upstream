//-----------------------------------------------------------------------
// <copyright file="GranularPermissionASTNode.cs" company="Integra.Space.Language">
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
    internal class GranularPermissionASTNode : AstNodeBase
    {
        /// <summary>
        /// Space permission identifier.
        /// </summary>
        private string terminalPermissionName1;

        /// <summary>
        /// Space permission identifier.
        /// </summary>
        private string terminalPermissionName2;

        /// <summary>
        /// Reserved word any.
        /// </summary>
        private string terminalAny;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            if (ChildrenNodes.Count == 1)
            {
                this.terminalPermissionName1 = ChildrenNodes[0].Token.Text;
            }
            else if (ChildrenNodes.Count == 2)
            {
                this.terminalPermissionName1 = ChildrenNodes[0].Token.Text;
                this.terminalPermissionName2 = ChildrenNodes[1].Token.Text;
            }
            else if (ChildrenNodes.Count == 3)
            {
                this.terminalPermissionName1 = ChildrenNodes[0].Token.Text;
                this.terminalAny = ChildrenNodes[1].Token.Text;
                this.terminalPermissionName2 = ChildrenNodes[2].Token.Text;
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
            this.EndEvaluate(thread);

            string permissionString = string.Empty;
            
            if (ChildrenNodes.Count == 1)
            {
                permissionString = this.terminalPermissionName1;
            }
            else if (ChildrenNodes.Count == 2)
            {
                permissionString = this.terminalPermissionName1 + this.terminalPermissionName2;
            }
            else if (ChildrenNodes.Count == 3)
            {
                permissionString = this.terminalPermissionName1 + this.terminalAny + this.terminalPermissionName2;
            }

            PermissionsEnum granularPermission;
            if (!System.Enum.TryParse<PermissionsEnum>(permissionString, true, out granularPermission))
            {
                thread.App.Parser.Context.AddParserError(Resources.ParseResults.BadPermission((int)ResultCodes.BadPermission, permissionString));
            }

            return granularPermission;
        }
    }
}
