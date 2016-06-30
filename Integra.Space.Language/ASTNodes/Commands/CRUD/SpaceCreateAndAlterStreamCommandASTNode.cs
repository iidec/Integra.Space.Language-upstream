//-----------------------------------------------------------------------
// <copyright file="SpaceCreateAndAlterStreamCommandASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System;
    using System.Collections.Generic;
    using CommandContext;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Parsing;
       
    /// <summary>
    /// Space command AST node class.
    /// </summary>
    internal class SpaceCreateAndAlterStreamCommandASTNode : AstNodeBase
    {
        /// <summary>
        /// Space command.
        /// </summary>
        private AstNodeBase action;

        /// <summary>
        /// Space user or role.
        /// </summary>
        private AstNodeBase spaceObject;

        /// <summary>
        /// Space query.
        /// </summary>
        private string query; 

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.action = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "CREATE_AND_ALTER_STREAM_COMMAND", ChildrenNodes[0]) as AstNodeBase;
            this.spaceObject = AddChild(Irony.Interpreter.Ast.NodeUseType.ValueRead, "SPACE_OBJECT", ChildrenNodes[1]) as AstNodeBase;
            this.query = ChildrenNodes[2].Token.Value.ToString().Trim();
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
            SpaceActionCommandEnum actionAux = (SpaceActionCommandEnum)this.action.Evaluate(thread);
            Tuple<string, SpaceObjectEnum> spaceObjectAux = (Tuple<string, SpaceObjectEnum>)this.spaceObject.Evaluate(thread);
            this.EndEvaluate(thread);

            List<Tuple<string, SpaceObjectEnum, bool?>> spaceObjectList = new List<Tuple<string, SpaceObjectEnum, bool?>>();
            spaceObjectList.Add(Tuple.Create<string, SpaceObjectEnum, bool?>(spaceObjectAux.Item1, spaceObjectAux.Item2, null));

            return new PipelineCommandContext(actionAux, spaceObjectList, this.query);
        }
    }
}
