//-----------------------------------------------------------------------
// <copyright file="DropCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Space action AST node class.
    /// </summary>
    internal class DropCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Space action name.
        /// </summary>
        private string spaceAction;

        /// <summary>
        /// Action enumerable type.
        /// </summary>
        private ActionCommandEnum action;

        /// <summary>
        /// Space user or role.
        /// </summary>
        private AstNodeBase systemObjectTypeName;

        /// <summary>
        /// Object identifier.
        /// </summary>
        private StatementListNode identifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropCommandASTNode"/> class.
        /// </summary>
        public DropCommandASTNode()
        {
            this.action = ActionCommandEnum.Unspecified;
        }

        /// <summary>
        /// Gets the action of the command.
        /// </summary>
        protected ActionCommandEnum Action
        {
            get
            {
                if (this.action == ActionCommandEnum.Unspecified)
                {
                    if (!System.Enum.TryParse(this.spaceAction, true, out this.action))
                    {
                        throw new Exceptions.SyntaxException(string.Format("Invalid action {0}.", this.spaceAction));
                    }
                }

                return this.action;
            }
        }

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.spaceAction = (string)ChildrenNodes[0].Token.Value;
            this.systemObjectTypeName = AddChild(NodeUseType.ValueRead, "SpaceObject", ChildrenNodes[1]) as AstNodeBase;
            this.identifiers = AddChild(NodeUseType.ValueRead, "SpaceObject", ChildrenNodes[2]) as StatementListNode;
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
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            
            SystemObjectEnum objectType = (SystemObjectEnum)this.systemObjectTypeName.Evaluate(thread);
            HashSet<string> identifiers = new HashSet<string>();
            foreach (IdentifierNode child in this.identifiers.GetChildNodes())
            {
                if (!identifiers.Add(child.AsString))
                {
                    throw new Exceptions.SyntaxException(string.Format("The identifier '{0}' is specified more than once."));
                }
            }

            this.EndEvaluate(thread);

            List<DropObjectNode> listOfDrops = new List<DropObjectNode>();

            CommandObject comandObject = null;
            foreach (string id in identifiers)
            {
                comandObject = new CommandObject(objectType, id, PermissionsEnum.Alter, false);
                listOfDrops.Add(new DropObjectNode(comandObject, this.Location.Line, this.Location.Column, this.GetNodeText(), null, databaseName));
            }

            return listOfDrops.ToArray();
        }
    }
}
