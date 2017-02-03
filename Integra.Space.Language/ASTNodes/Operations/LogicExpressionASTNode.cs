//-----------------------------------------------------------------------
// <copyright file="LogicExpressionASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Operations
{
    using System;
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Errors;
    
    using Integra.Space.Language.General.Validations;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// LogicExpressionNode class
    /// </summary>
    internal sealed class LogicExpressionASTNode : AstNodeBase
    {
        /// <summary>
        /// minuend of the subtract
        /// </summary>
        private AstNodeBase leftNode;

        /// <summary>
        /// operator of the expression
        /// </summary>
        private string operationNode;

        /// <summary>
        /// subtrahend of the subtract
        /// </summary>
        private AstNodeBase rightNode;

        /// <summary>
        /// this.result plan
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

            if (childrenCount == 3)
            {
                this.leftNode = AddChild(NodeUseType.Parameter, "leftNode", ChildrenNodes[0]) as AstNodeBase;
                this.operationNode = (string)ChildrenNodes[1].Token.Value;
                this.rightNode = AddChild(NodeUseType.Parameter, "rightNode", ChildrenNodes[2]) as AstNodeBase;
            }
            else if (childrenCount == 2)
            {
                this.rightNode = AddChild(NodeUseType.Parameter, "rightNode", ChildrenNodes[1]) as AstNodeBase;
                this.operationNode = (string)ChildrenNodes[0].Token.Value;
            }
            else
            {
                this.leftNode = AddChild(NodeUseType.Parameter, "leftNode", ChildrenNodes[0]) as AstNodeBase;
            }
        }

        /// <summary>
        /// selects the operations to execute
        /// </summary>
        /// <param name="operacion">operator symbol</param>
        /// <param name="thread">actual thread</param>
        /// <returns>The result node type.</returns>
        public PlanNodeTypeEnum SelectOperation(string operacion, ScriptThread thread)
        {
            PlanNodeTypeEnum resultType = PlanNodeTypeEnum.None;

            if (string.IsNullOrEmpty(operacion))
            {
                return resultType;
            }

            try
            {
                switch (operacion)
                {
                    case "and":
                        resultType = PlanNodeTypeEnum.And;
                        break;
                    case "or":
                        resultType = PlanNodeTypeEnum.Or;
                        break;
                    case "not":
                        resultType = PlanNodeTypeEnum.Not;
                        break;
                    default:
                        ErrorNode error = new ErrorNode();
                        error.Column = this.result.Column;
                        error.Line = this.result.Line;
                        error.NodeText = this.result.NodeText;
                        error.Title = "Operación invalida";
                        error.Message = "La operación " + operacion + " no es valida.";
                        Errors errores = new Errors(thread);
                        errores.AlmacenarError(error);
                        break;
                }
            }
            catch (Exception e)
            {
                ErrorNode error = new ErrorNode();
                error.Column = this.result.Column;
                error.Line = this.result.Line;
                error.NodeText = this.result.NodeText;
                error.Title = "Operación invalida";
                error.Message = "Ocurrio un error al seleccionar la operación a realizar";
                Errors errores = new Errors(thread);
                errores.AlmacenarError(error);
            }

            return resultType;
        }

        /// <summary>
        /// CreateChildrenForResult
        /// Create the children of the actual node
        /// </summary>
        /// <param name="leftNode">left child</param>
        /// <param name="rightNode">right child</param>
        /// <param name="thread">actual thread</param>
        public void CreateChildrensForResult(PlanNode leftNode, PlanNode rightNode, ScriptThread thread)
        {
            Type leftType = null;
            Type rightType = null;

            if (leftNode.Properties.ContainsKey("DataType"))
            {
                leftType = Type.GetType(leftNode.Properties["DataType"].ToString());
            }

            if (rightNode.Properties.ContainsKey("DataType"))
            {
                rightType = Type.GetType(rightNode.Properties["DataType"].ToString());
            }

            this.result.Children = new List<PlanNode>();

            TypeValidation validate = new TypeValidation(leftType, rightType, this.result, thread);
            Type selectedType = validate.SelectTypeToCast();

            if (selectedType != null)
            {
                if (validate.ConvertLeftNode)
                {
                    PlanNode casteo = new PlanNode(leftNode.Line, leftNode.Column, PlanNodeTypeEnum.Cast);
                    casteo.NodeText = leftNode.NodeText;
                    casteo.Properties.Add("DataType", typeof(bool));
                    casteo.Children = new List<PlanNode>();
                    casteo.Children.Add(leftNode);
                    this.result.Children.Add(casteo);
                    this.result.Children.Add(rightNode);
                }
                else if (validate.ConvertRightNode)
                {
                    PlanNode casteo = new PlanNode(rightNode.Line, rightNode.Column, PlanNodeTypeEnum.Cast);
                    casteo.NodeText = rightNode.NodeText;
                    casteo.Properties.Add("DataType", typeof(bool));
                    casteo.Children = new List<PlanNode>();
                    casteo.Children.Add(rightNode);
                    this.result.Children.Add(leftNode);
                    this.result.Children.Add(casteo);
                }
            }
            else
            {
                this.result.Children.Add(leftNode);
                this.result.Children.Add(rightNode);
            }
        }

        /// <summary>
        /// CreateChildrenForResult
        /// Create the children of the actual node
        /// </summary>
        /// <param name="rightNode">right child</param>
        /// <param name="thread">actual thread</param>
        public void CreateChildrensForResult(PlanNode rightNode, ScriptThread thread)
        {
            Type leftType = typeof(bool);
            Type rightType = null;

            if (rightNode.Properties.ContainsKey("DataType"))
            {
                rightType = Type.GetType(rightNode.Properties["DataType"].ToString());
            }

            this.result.Children = new List<PlanNode>();

            TypeValidation validate = new TypeValidation(leftType, rightType, this.result, thread);
            Type selectedType = validate.SelectTypeToCast();

            if (selectedType != null && rightType != null)
            {
                if (rightType.Equals(typeof(object)))
                {
                    PlanNode casteo = new PlanNode(rightNode.Line, rightNode.Column, PlanNodeTypeEnum.Cast);
                    casteo.NodeText = rightNode.NodeText;
                    casteo.Properties.Add("DataType", typeof(bool));
                    casteo.Children = new List<PlanNode>();
                    casteo.Children.Add(rightNode);
                    this.result.Children.Add(casteo);
                }
                else
                {
                    this.result.Children.Add(rightNode);
                }
            }
            else
            {
                this.result.Children.Add(rightNode);
            }
        }

        /// <summary>
        /// validate the types to operate
        /// </summary>
        /// <param name="leftNode">left child</param>
        /// <param name="rightNode">right child</param>
        /// <param name="operacion">operator symbol</param>
        /// <param name="thread">actual thread</param>
        public void ValidateTypesForOperation(PlanNode leftNode, PlanNode rightNode, string operacion, ScriptThread thread)
        {
            Type leftType = null;
            Type rightType = null;

            if (leftNode.Properties.ContainsKey("DataType"))
            {
                leftType = Type.GetType(leftNode.Properties["DataType"].ToString());
            }

            if (rightNode.Properties.ContainsKey("DataType"))
            {
                rightType = Type.GetType(rightNode.Properties["DataType"].ToString());
            }

            ErrorNode error = new ErrorNode();
            error.Column = this.result.Column;
            error.Line = this.result.Line;
            error.NodeText = this.result.NodeText;
            error.Title = "Operación invalida";
            Errors errores = new Errors(thread);

            try
            {
                switch (operacion)
                {
                    case "and":
                    case "or":
                        if (!leftType.Equals(typeof(bool)) && !rightType.Equals(typeof(object)))
                        {
                            error.Message = "No es posible realizar la operación booleana '" + operacion + "' con el tipo " + leftType;
                            errores.AlmacenarError(error);
                        }

                        if (!rightType.Equals(typeof(bool)) && !rightType.Equals(typeof(object)))
                        {
                            error.Message = "No es posible realizar la operación booleana '" + operacion + "' con el tipo " + leftType;
                            errores.AlmacenarError(error);
                        }

                        break;
                    default:
                        error.Message = "La operación '" + operacion + "' no es válida.";
                        errores.AlmacenarError(error);
                        break;
                }
            }
            catch (Exception e)
            {
                error.Message = "No es posible realizar una operación booleana '" + operacion + "' con los tipos de dato " + leftType + " y " + rightType;
                errores.AlmacenarError(error);
            }
        }

        /// <summary>
        /// validate the types to operate
        /// </summary>
        /// <param name="rightNode">right child</param>
        /// <param name="operacion">operator symbol</param>
        /// <param name="thread">actual thread</param>
        public void ValidateTypesForOperation(PlanNode rightNode, string operacion, ScriptThread thread)
        {
            Type leftType = typeof(bool);
            Type rightType = null;

            if (rightNode.Properties.ContainsKey("DataType"))
            {
                rightType = Type.GetType(rightNode.Properties["DataType"].ToString());
            }

            ErrorNode error = new ErrorNode();
            error.Column = this.result.Column;
            error.Line = this.result.Line;
            error.NodeText = this.result.NodeText;
            error.Title = "Operación invalida";
            Errors errores = new Errors(thread);

            try
            {
                switch (operacion)
                {
                    case "not":
                        if (!rightType.Equals(typeof(bool)) && !rightType.Equals(typeof(object)))
                        {
                            error.Message = "La operación '" + operacion + "' solo puede realizarse con tipos booleanos";
                            errores.AlmacenarError(error);
                        }

                        break;
                    default:
                        error.Message = "La operación '" + operacion + "' no es valida.";
                        errores.AlmacenarError(error);
                        break;
                }
            }
            catch (Exception e)
            {
                error.Message = "No es posible realizar una operación '" + operacion + "', solo puede realizarse con tipos booleanos";
                errores.AlmacenarError(error);
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
            int childrenCount = ChildrenNodes.Count;

            this.result = new PlanNode(this.Location.Line, this.Location.Column, this.SelectOperation(this.operationNode, thread));
            this.result.Properties.Add("DataType", typeof(bool));

            if (childrenCount == 3)
            {
                this.BeginEvaluate(thread);
                PlanNode l = (PlanNode)this.leftNode.Evaluate(thread);
                PlanNode r = (PlanNode)this.rightNode.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText = l.NodeText + " " + this.operationNode + " " + r.NodeText;
                this.CreateChildrensForResult(l, r, thread);
                this.ValidateTypesForOperation(l, r, this.operationNode, thread);
                if (bool.Parse(r.Properties["IsConstant"].ToString()) == true || bool.Parse(l.Properties["IsConstant"].ToString()) == true)
                {
                    this.result.Properties.Add("IsConstant", true);
                }
                else
                {
                    this.result.Properties.Add("IsConstant", false);
                }
            }
            else if (childrenCount == 2)
            {
                this.BeginEvaluate(thread);
                PlanNode r = (PlanNode)this.rightNode.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText = this.operationNode + "(" + r.NodeText + ")";
                this.CreateChildrensForResult(r, thread);
                this.ValidateTypesForOperation(r, this.operationNode, thread);
                this.result.Properties.Add("IsConstant", bool.Parse(r.Properties["IsConstant"].ToString()));
            }
            else
            {
                this.result = (PlanNode)this.leftNode.Evaluate(thread);
            }

            return this.result;
        }
    }
}
