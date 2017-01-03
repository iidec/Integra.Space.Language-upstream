//-----------------------------------------------------------------------
// <copyright file="UnaryArithmeticExpressionASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Operations
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;

    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// UnaryArithmeticExpressionNode class
    /// </summary>
    internal sealed class UnaryArithmeticExpressionASTNode : AstNodeBase
    {
        /// <summary>
        /// operator '-' or '+'
        /// </summary>
        private string operationNode;

        /// <summary>
        /// numeric value
        /// </summary>
        private AstNodeBase rightNode;

        /// <summary>
        /// result plan
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            int childrenCount = ChildrenNodes.Count;
            if (childrenCount == 1)
            {
                this.rightNode = AddChild(NodeUseType.Parameter, "value", ChildrenNodes[0]) as AstNodeBase;
            }
            else if (childrenCount == 2)
            {
                this.operationNode = (string)ChildrenNodes[0].Token.Value;
                this.rightNode = AddChild(NodeUseType.Parameter, "rightNode", ChildrenNodes[1]) as AstNodeBase;
            }
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
            PlanNode r = (PlanNode)this.rightNode.Evaluate(thread);
            this.EndEvaluate(thread);

            int childrenCount = ChildrenNodes.Count;

            // se especifican los hijos y el tipo de nodo result si la operacion es de negacion aritmetica
            if (this.operationNode == "-")
            {
                this.result = new PlanNode(this.Location.Line, this.Location.Column, PlanNodeTypeEnum.Negate);

                // se iguala result al nodo hijo para mantener sus propiedades
                foreach (var property in r.Properties)
                {
                    this.result.Properties.Add(property.Key, property.Value);
                }

                // se especifica la nueva informacion para result
                this.result.Children = new List<PlanNode>();
                this.result.Children.Add(r);
            }
            else
            {
                this.result = r;
            }

            // se especifica el texto del nodo resultante
            this.result.NodeText = this.operationNode + r.NodeText;

            return this.result;
        }
    }
}
