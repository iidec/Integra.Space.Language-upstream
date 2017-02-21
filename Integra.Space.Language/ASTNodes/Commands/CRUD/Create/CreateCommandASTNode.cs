//-----------------------------------------------------------------------
// <copyright file="CreateCommandASTNode.cs" company="Integra.Space.Language">
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
    using Irony.Parsing;

    /// <summary>
    /// Space action AST node class.
    /// </summary>
    /// <typeparam name="TOption">Command option type enumerable.</typeparam>
    internal abstract class CreateCommandASTNode<TOption> : AstNodeBase where TOption : struct, System.IConvertible
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
        /// Not allowed options.
        /// </summary>
        private List<TOption> notAllowedOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandASTNode{TOption}"/> class.
        /// </summary>
        /// <param name="granularPermission">Granular permission.</param>
        public CreateCommandASTNode(PermissionsEnum granularPermission)
        {
            this.action = ActionCommandEnum.Unspecified;
            this.granularPermission = granularPermission;
            this.notAllowedOptions = new List<TOption>();
        }

        /// <summary>
        /// Gets the not allowed options of the actual command.
        /// </summary>
        protected List<TOption> NotAllowedOptions
        {
            get
            {
                return this.notAllowedOptions;
            }
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
            this.identifier = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "IDENTIFIER_WITH_PATH", ChildrenNodes[2]) as AstNodeBase; // (string)ChildrenNodes[2].Token.Value;
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

            return new CommandObject(systemObjectType, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, this.granularPermission, true);
        }

        /// <summary>
        /// Checks whether the command contains options that are not allowed in the actual command.
        /// </summary>
        /// <param name="actualOptions">Specified options at the command.</param>
        /// <param name="thread">Thread of the evaluated grammar</param>
        protected void CheckAllowedOptions(Dictionary<TOption, object> actualOptions, ScriptThread thread)
        {
            foreach (TOption option in actualOptions.Keys)
            {
                if (this.notAllowedOptions.Contains(option))
                {
                    thread.App.Parser.Context.AddParserError(Resources.ParseResults.InvalidCommandOption((int)ResultCodes.InvalidCommandOption, option.ToString()));
                }
            }
        }

        /// <summary>
        /// Add a new option to the command dictionary of options.
        /// </summary>
        /// <param name="actualOptions">Actual dictionary of options.</param>
        /// <param name="optionToAdd">Command option that will be added to the dictionary of options.</param>
        /// <param name="thread">Thread of the evaluated grammar</param>
        protected void AddCommandOption(Dictionary<TOption, object> actualOptions, CommandOption<TOption> optionToAdd, ScriptThread thread)
        {
            if (actualOptions.ContainsKey(optionToAdd.Option))
            {
                thread.App.Parser.Context.AddParserError(Resources.ParseResults.DuplicateCommandOption((int)ResultCodes.DuplicateCommandOption, optionToAdd.Option.ToString()));
            }
            else
            {
                actualOptions.Add(optionToAdd.Option, optionToAdd.Value);
            }
        }
    }
}
