//-----------------------------------------------------------------------
// <copyright file="PermissionsNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Permissions
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// GrantNode class
    /// </summary>
    internal sealed class PermissionsNode : AstNodeBase
    {
        /// <summary>
        /// grant, revoke, deny
        /// </summary>
        private string permissionType;

        /// <summary>
        /// permission body
        /// </summary>
        private AstNodeBase secureObject;

        /// <summary>
        /// secure object type: stream, role, server role
        /// </summary>
        private AstNodeBase secureObjectName;

        /// <summary>
        /// reserved word 'to'
        /// </summary>
        private string toWord;

        /// <summary>
        /// user or role to assign the permission
        /// </summary>
        private AstNodeBase userOrRole;

        /// <summary>
        /// result planNode
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
            this.permissionType = (string)ChildrenNodes[0].Token.Value;
            this.secureObject = AddChild(NodeUseType.Parameter, SR.ObjectRole, ChildrenNodes[1]) as AstNodeBase;
            this.secureObjectName = AddChild(NodeUseType.Parameter, SR.IdentifierRole, ChildrenNodes[2]) as AstNodeBase;
            this.toWord = (string)ChildrenNodes[3].Token.Value;
            this.userOrRole = AddChild(NodeUseType.Parameter, SR.ObjectRole, ChildrenNodes[4]) as AstNodeBase;

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
            string secureObjectAux = (string)this.secureObject.Evaluate(thread);
            PlanNode secureObjectNameAux = (PlanNode)this.secureObjectName.Evaluate(thread);
            PlanNode userOrRoleAux = (PlanNode)this.userOrRole.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.Properties.Add("SecureObjectType", secureObjectAux);
            this.result.NodeText = this.permissionType + " " + secureObjectAux + " " + secureObjectNameAux.NodeText + " " + this.toWord + " " + userOrRoleAux.NodeText;

            this.SelectPermission();

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(secureObjectNameAux);
            this.result.Children.Add(userOrRoleAux);

            return this.result;
        }

        /// <summary>
        /// set the PlanNode type
        /// </summary>
        private void SelectPermission()
        {
            string p = this.permissionType.ToLower();
            if (p == SR.ReservedWordGrant)
            {
                this.result.NodeType = PlanNodeTypeEnum.Grant;
            }
            else if (p == SR.ReservedWordRevoke)
            {
                this.result.NodeType = PlanNodeTypeEnum.Revoke;
            }
            else if (p == SR.ReservedWordDeny)
            {
                this.result.NodeType = PlanNodeTypeEnum.Deny;
            }
        }
    }
}
