//-----------------------------------------------------------------------
// <copyright file="ObjectValueNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects.Object
{
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ObjectWithSuffix class
    /// </summary>
    internal sealed class ObjectValueNode : AstNodeBase
    {
        /// <summary>
        /// result plan
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// object node
        /// </summary>
        private AstNodeBase objeto;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            this.objeto = AddChild(NodeUseType.Keyword, SR.ObjectRole, ChildrenNodes[0]) as AstNodeBase;
            this.result = new PlanNode();
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            int cantHijos = ChildrenNodes.Count();

            this.BeginEvaluate(thread);
            PlanNode field = (PlanNode)this.objeto.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.Column = field.Column;
            this.result.Line = field.Line;
            this.result.NodeText = field.NodeText;
            this.result.Properties.Add("PropertyName", field.Properties["PropertyName"]);
            this.result.Properties.Add("IncidenciasEnOn", 0);
            this.result.Properties.Add(SR.DataTypeProperty, typeof(object));
            this.result.NodeType = PlanNodeTypeEnum.ObjectValue;
            this.result.Children = new List<PlanNode>();
            this.result.Children.Add(field);

            return this.result;
        }
    }
}
