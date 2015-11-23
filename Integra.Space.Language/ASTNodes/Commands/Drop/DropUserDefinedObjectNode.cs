//-----------------------------------------------------------------------
// <copyright file="DropUserDefinedObjectNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.Drop
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// DropUserDefinedObjectNode class
    /// </summary>
    internal sealed class DropUserDefinedObjectNode : AstNodeBase
    {
        /// <summary>
        /// reserved word
        /// </summary>
        private string dropWord;

        /// <summary>
        /// user defined object: user, role, assembly, adapter, stream, source, trigger
        /// </summary>
        private AstNodeBase userDefinedObject;

        /// <summary>
        /// name of the object to drop
        /// </summary>
        private AstNodeBase objectName;

        /// <summary>
        /// result planNode
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
            this.dropWord = (string)ChildrenNodes[0].Token.Value;
            this.userDefinedObject = AddChild(NodeUseType.Parameter, SR.UserDefinedObjectRole, ChildrenNodes[1]) as AstNodeBase;
            this.objectName = AddChild(NodeUseType.Parameter, SR.IdentifierRole, ChildrenNodes[2]) as AstNodeBase;

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
            string userDefinedObjectAux = (string)this.userDefinedObject.Evaluate(thread);
            PlanNode objectNameAux = (PlanNode)this.objectName.Evaluate(thread);
            this.EndEvaluate(thread);

            this.SelectNodeType(userDefinedObjectAux);
            this.result.NodeText = SR.DropNodeText(this.dropWord, userDefinedObjectAux, objectNameAux.NodeText);

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(objectNameAux);

            return this.result;
        }

        /// <summary>
        /// select the drop node type specify
        /// </summary>
        /// <param name="objectType">object to drop: user, role, assembly, adapter, stream, source, trigger</param>
        private void SelectNodeType(string objectType)
        {
            objectType = objectType.ToLower();

            if (objectType == SR.AssemblyObject)
            {
                this.result.NodeType = PlanNodeTypeEnum.DropAssembly;
            }
            else if (objectType == SR.AdapterObject)
            {
                this.result.NodeType = PlanNodeTypeEnum.DropAdapter;
            }
            else if (objectType == SR.SourceObject)
            {
                this.result.NodeType = PlanNodeTypeEnum.DropSource;
            }
            else if (objectType == SR.StreamObject)
            {
                this.result.NodeType = PlanNodeTypeEnum.DropStream;
            }
            else if (objectType == SR.TriggerObject)
            {
                this.result.NodeType = PlanNodeTypeEnum.DropTrigger;
            }
            else if (objectType == SR.RoleObject)
            {
                this.result.NodeType = PlanNodeTypeEnum.DropRole;
            }
            else if (objectType == SR.UserObject)
            {
                this.result.NodeType = PlanNodeTypeEnum.DropUser;
            }
        }
    }
}
