//-----------------------------------------------------------------------
// <copyright file="SpaceObjectWithIdASTNode.cs" company="Integra.Space.Language">
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
    internal class SpaceObjectWithIdASTNode : AstNodeBase
    {
        /// <summary>
        /// Space object.
        /// </summary>
        private AstNodeBase spaceObject;

        /// <summary>
        /// Space object identifier.
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

            this.spaceObject = AddChild(NodeUseType.ValueRead, "SpaceObject", ChildrenNodes[0]) as AstNodeBase;
            this.identifier = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "IDENTIFIER_WITH_PATH", ChildrenNodes[1]) as AstNodeBase; // (string)ChildrenNodes[1].Token.Value;
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
            SystemObjectEnum @object = (SystemObjectEnum)this.spaceObject.Evaluate(thread);
            System.Tuple<string, string, string> identifierWithPath = (System.Tuple<string, string, string>)this.identifier.Evaluate(thread);

            // gets the database if it was defined.
            Binding databaseBinding = thread.Bind("Database", BindingRequestFlags.Read);
            string databaseName = (string)databaseBinding.GetValueRef(thread);
            this.EndEvaluate(thread);
            
            if (!string.IsNullOrWhiteSpace(identifierWithPath.Item1))
            {
                databaseName = identifierWithPath.Item1;
            }

            return new CommandObject(@object, databaseName, identifierWithPath.Item2, identifierWithPath.Item3, PermissionsEnum.ControlServer, null); // new CommandObject(@object, this.identifier, PermissionsEnum.ControlServer, null);            
        }
    }
}
