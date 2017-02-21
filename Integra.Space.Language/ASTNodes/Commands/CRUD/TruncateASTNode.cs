//-----------------------------------------------------------------------
// <copyright file="TruncateASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using Integra.Space.Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// JoinNode class
    /// </summary>
    internal sealed class TruncateASTNode : AstNodeBase
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
        private string systemObjectTypeName;

        /// <summary>
        /// Granular permission.
        /// </summary>
        private PermissionsEnum granularPermission;

        /// <summary>
        /// Object identifier.
        /// </summary>
        private AstNodeBase identifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="TruncateASTNode"/> class.
        /// </summary>
        public TruncateASTNode()
        {
            this.action = ActionCommandEnum.Truncate;
            this.granularPermission = PermissionsEnum.Alter;
        }

        /// <summary>
        /// Gets the action of the command.
        /// </summary>
        public Common.ActionCommandEnum Action
        {
            get
            {
                if (this.action == ActionCommandEnum.Unspecified)
                {
                    if (!System.Enum.TryParse(this.spaceAction, true, out this.action))
                    {
                        return ActionCommandEnum.Unspecified;
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
            if (!System.Enum.TryParse(this.spaceAction, true, out this.action))
            {
                context.AddMessage(Irony.ErrorLevel.Error, this.Location, Resources.ParseResults.InvalidCommandAction((int)ResultCodes.InvalidCommandAction, this.spaceAction));
            }

            this.systemObjectTypeName = (string)ChildrenNodes[1].Token.Value;
            this.identifier = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "IDENTIFIER_WITH_PATH", ChildrenNodes[2]) as AstNodeBase;
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
            System.Tuple<string, string, string> identifierWithPath = (System.Tuple<string, string, string>)this.identifier.Evaluate(thread);

            // gets the database if it was defined.
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);

            if (this.systemObjectTypeName.Equals("role", System.StringComparison.InvariantCultureIgnoreCase) | this.systemObjectTypeName.Equals("user", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.systemObjectTypeName = "database" + this.systemObjectTypeName;
            }

            SystemObjectEnum systemObjectType;
            if (!System.Enum.TryParse(this.systemObjectTypeName, true, out systemObjectType))
            {
                thread.App.Parser.Context.AddParserError(Resources.ParseResults.InvalidSystemObjectType((int)ResultCodes.InvalidSystemObjectType, this.systemObjectTypeName));
            }

            if (!string.IsNullOrWhiteSpace(identifierWithPath.Item1))
            {
                databaseName = identifierWithPath.Item1;
            }

            CommandObject commandObject = new CommandObject(systemObjectType, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, this.granularPermission, false);

            return new TruncateNode(this.Action, commandObject, this.Location.Line, this.Location.Column, this.NodeText);
        }
    }
}
