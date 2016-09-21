//-----------------------------------------------------------------------
// <copyright file="TakeOwnershipASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class TakeOwnershipASTNode : AstNodeBase
    {
        /// <summary>
        /// Space action.
        /// </summary>
        private string action;

        /// <summary>
        /// Reserved world 'on'.
        /// </summary>
        private string on;

        /// <summary>
        /// System object type.
        /// </summary>
        private AstNodeBase objectType;

        /// <summary>
        /// System object type.
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

            this.action = ChildrenNodes[0].Token.Text + ChildrenNodes[1].Token.Text;
            this.on = ChildrenNodes[2].Token.Text;
            this.objectType = AddChild(NodeUseType.ValueRead, "SpaceObjectType", ChildrenNodes[3]) as SpaceObjectASTNode;
            this.identifier = ChildrenNodes[4].Token.ValueString;
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
            SystemObjectEnum objectTypeAux = (SystemObjectEnum)this.objectType.Evaluate(thread);

            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            ActionCommandEnum actionAux;
            if (!System.Enum.TryParse(this.action, true, out actionAux))
            {
                throw new Exceptions.SyntaxException(string.Format("Invalid action {0}.", this.action));
            }
            
            return new TakeOwnershipCommandNode(actionAux, new CommandObject(objectTypeAux, this.identifier, PermissionsEnum.TakeOwnership, false), this.Location.Line, this.Location.Column, this.GetNodeText(), null, databaseName);
        }
    }
}
