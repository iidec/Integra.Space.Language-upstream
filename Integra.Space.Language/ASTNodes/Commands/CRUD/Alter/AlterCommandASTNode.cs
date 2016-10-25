//-----------------------------------------------------------------------
// <copyright file="AlterCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System.Collections.Generic;
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space action AST node class.
    /// </summary>
    /// <typeparam name="TOption">Command option type enumerable.</typeparam>
    internal abstract class AlterCommandASTNode<TOption> : AstNodeBase where TOption : struct, System.IConvertible
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
        /// Initializes a new instance of the <see cref="AlterCommandASTNode{TOption}"/> class.
        /// </summary>
        /// <param name="granularPermission">Granular permission.</param>
        public AlterCommandASTNode(PermissionsEnum granularPermission)
        {
            this.action = ActionCommandEnum.Unspecified;
            this.granularPermission = granularPermission;
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
            this.systemObjectTypeName = (string)ChildrenNodes[1].Token.Value;

            // this.identifier = (string)ChildrenNodes[2].Token.Value;
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
                throw new Exceptions.SyntaxException(string.Format("Invalid object {0}.", this.systemObjectTypeName));
            }

            if (!string.IsNullOrWhiteSpace(identifierWithPath.Item1))
            {
                databaseName = identifierWithPath.Item1;
            }

            return new CommandObject(systemObjectType, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, this.granularPermission, false);
        }

        /// <summary>
        /// Add a new option to the command dictionary of options.
        /// </summary>
        /// <param name="actualOptions">Actual dictionary of options.</param>
        /// <param name="optionToAdd">Command option that will be added to the dictionary of options.</param>
        protected void AddCommandOption(Dictionary<TOption, object> actualOptions, CommandOption<TOption> optionToAdd)
        {
            if (actualOptions.ContainsKey(optionToAdd.Option))
            {
                throw new Exceptions.SyntaxException(string.Format("{0} option is defined more than once.", optionToAdd.Option.ToString()));
            }
            else
            {
                actualOptions.Add(optionToAdd.Option, optionToAdd.Value);
            }
        }
    }
}
