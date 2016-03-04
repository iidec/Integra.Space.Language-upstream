//-----------------------------------------------------------------------
// <copyright file="ObjectIdOrNumberNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Objects.Object
{
    using Integra.Space.Language.ASTNodes.Base;

    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// ObjectIdOrNumberNode class
    /// </summary>
    internal sealed class ObjectIdOrNumberNode : AstNodeBase
    {
        /// <summary>
        /// id or number to search in the message
        /// </summary>
        private AstNodeBase value;

        /// <summary>
        /// text of the actual node
        /// </summary>
        private string text;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            int cantHijos = ChildrenNodes.Count;
            if (cantHijos == 1)
            {
                this.value = AddChild(NodeUseType.Keyword, SR.ValueRole, ChildrenNodes[0]) as AstNodeBase;
                this.text = ChildrenNodes[0].Token.Text;
            }
            else
            {
                this.value = AddChild(NodeUseType.Keyword, SR.ValueRole, ChildrenNodes[1]) as AstNodeBase;
                this.text = ChildrenNodes[0].Token.Text + ChildrenNodes[1].Token.Text;
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
            PlanNode result = (PlanNode)this.value.Evaluate(thread);
            this.EndEvaluate(thread);

            int cantHijos = ChildrenNodes.Count;
            if (cantHijos == 1)
            {
                string propertyName = string.Empty;
                if (result.Properties["DataType"].Equals(typeof(string).ToString()))
                {
                    result.NodeText = "[" + this.text + "]";
                    propertyName = ChildrenNodes[0].Token.Value.ToString().Trim().Replace(' ', '_');
                }
                else if (result.Properties["DataType"].Equals(typeof(object).ToString()))
                {
                    result.NodeText = this.text;
                    propertyName = ChildrenNodes[0].Token.Value.ToString();
                }

                result.Properties["DataType"] = typeof(string).ToString();
                result.Properties.Add("PropertyName", propertyName);
            }
            else
            {
                result.NodeText = this.text;
                string propertyName = ChildrenNodes[0].Token.Value.ToString() + ChildrenNodes[1].Token.Value.ToString();
                result.Properties.Add("PropertyName", propertyName);
            }

            return result;
        }
    }
}
