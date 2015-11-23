//-----------------------------------------------------------------------
// <copyright file="ParameterNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.General
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ParameterNode class
    /// </summary>
    internal sealed class ParameterNode : AstNodeBase
    {
        /// <summary>
        /// parameter Type
        /// </summary>
        private AstNodeBase parameterType;

        /// <summary>
        /// reserved word @
        /// </summary>
        private string arrobaWord;

        /// <summary>
        /// name of the parameter
        /// </summary>
        private AstNodeBase parameterName;

        /// <summary>
        /// reserved word =
        /// </summary>
        private string equalSign;

        /// <summary>
        /// value of the parameter, only constants
        /// </summary>
        private AstNodeBase parameterValue;

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
            this.parameterType = AddChild(NodeUseType.Parameter, SR.TypeRole, ChildrenNodes[0]) as AstNodeBase;
            this.arrobaWord = (string)ChildrenNodes[1].Token.Value;
            this.parameterName = AddChild(NodeUseType.Parameter, SR.IdentifierRole, ChildrenNodes[2]) as AstNodeBase;
            this.equalSign = (string)ChildrenNodes[3].Token.Value;
            this.parameterValue = AddChild(NodeUseType.Parameter, SR.ValueRole, ChildrenNodes[4]) as AstNodeBase;
            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;
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
            PlanNode parameterTypeAux = (PlanNode)this.parameterType.Evaluate(thread);
            PlanNode paramterNameAux = (PlanNode)this.parameterName.Evaluate(thread);
            PlanNode parameterValueAux = (PlanNode)this.parameterValue.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.NodeText = SR.ParameterNodeText(parameterTypeAux.NodeText, this.arrobaWord, paramterNameAux.NodeText, this.equalSign, parameterValueAux.NodeText);

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(parameterTypeAux);
            this.result.Children.Add(paramterNameAux);
            this.result.Children.Add(parameterValueAux);

            return this.result;
        }
    }
}
