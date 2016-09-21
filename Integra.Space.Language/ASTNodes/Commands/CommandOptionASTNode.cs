//-----------------------------------------------------------------------
// <copyright file="CommandOptionASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using Common;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space object AST node class.
    /// </summary>
    /// <typeparam name="TOptionEnum">Type of the options enumerable.</typeparam>
    internal class CommandOptionASTNode<TOptionEnum> : AstNodeBase where TOptionEnum : struct, System.IConvertible
    {
        /// <summary>
        /// Space object.
        /// </summary>
        private string spaceUserOption;

        /// <summary>
        /// Space object identifier.
        /// </summary>
        private object optionValue;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.spaceUserOption = (string)ChildrenNodes[0].Token.Value;
            this.optionValue = ChildrenNodes[2].Token.Value;
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

            TOptionEnum userOptionAux;
            if (System.Enum.TryParse(this.spaceUserOption, true, out userOptionAux) && this.optionValue != null)
            {
                return new CommandOption<TOptionEnum>(userOptionAux, this.optionValue);
            }
            else
            {
                throw new Exceptions.SyntaxException(string.Format("Invalid user option {0}.", this.spaceUserOption));
            }
        }
    }
}
