//-----------------------------------------------------------------------
// <copyright file="SpaceObjectASTNode.cs" company="Integra.Space.Language">
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
    internal class SpaceObjectASTNode : AstNodeBase
    {
        /// <summary>
        /// Space object.
        /// </summary>
        private string spaceObject;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.spaceObject = (string)ChildrenNodes[0].Token.Value;
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

            SystemObjectEnum @object;
            if (this.spaceObject.Equals("role", System.StringComparison.InvariantCultureIgnoreCase) | this.spaceObject.Equals("user", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.spaceObject = "database" + this.spaceObject;
            }

            if (System.Enum.TryParse(this.spaceObject, true, out @object))
            {
                return @object;
            }
            else
            {
                thread.App.Parser.Context.AddParserError(Resources.ParseResults.InvalidSystemObjectType((int)LanguageResultCodes.InvalidSystemObjectType, this.spaceObject));
                return @object;
            }
        }
    }
}
