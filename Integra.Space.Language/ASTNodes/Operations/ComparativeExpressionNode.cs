//-----------------------------------------------------------------------
// <copyright file="ComparativeExpressionNode.cs" company="Integra.Space.Language">
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
    /// ComparativeExpressionNode class
    /// </summary>
    internal sealed class ComparativeExpressionNode : AstNodeBase
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
                this.result = new PlanNode();
                this.result.Column = ChildrenNodes[1].Token.Location.Column;
                this.result.Properties.Add("DataType", typeof(bool).ToString());
                this.result.Line = ChildrenNodes[1].Token.Location.Line;
            }
            else if (childrenCount == 2)
            {
                this.rightNode = AddChild(NodeUseType.Parameter, "rightNode", ChildrenNodes[1]) as AstNodeBase;
                this.operationNode = (string)ChildrenNodes[0].Token.Value;
                this.result = new PlanNode();
                this.result.Column = ChildrenNodes[0].Token.Location.Column;
                this.result.Properties.Add("DataType", typeof(bool).ToString());
                this.result.Line = ChildrenNodes[0].Token.Location.Line;
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
        public void SelectOperation(string operacion, ScriptThread thread)
        {
            try
            {
                switch (operacion)
                {
                    case "==":
                        this.result.NodeType = PlanNodeTypeEnum.Equal;
                        break;
                    case "!=":
                        this.result.NodeType = PlanNodeTypeEnum.NotEqual;
                        break;
                    case "<=":
                        this.result.NodeType = PlanNodeTypeEnum.LessThanOrEqual;
                        break;
                    case "<":
                        this.result.NodeType = PlanNodeTypeEnum.LessThan;
                        break;
                    case ">=":
                        this.result.NodeType = PlanNodeTypeEnum.GreaterThanOrEqual;
                        break;
                    case ">":
                        this.result.NodeType = PlanNodeTypeEnum.GreaterThan;
                        break;
                    case "like":
                        this.result.NodeType = PlanNodeTypeEnum.Like;
                        break;
                    case "not":
                        this.result.NodeType = PlanNodeTypeEnum.Not;
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
                PlanNode casteoLeft = null;
                if (validate.ConvertLeftNode)
                {
                    casteoLeft = new PlanNode();
                    casteoLeft.Column = leftNode.Column;
                    casteoLeft.Line = leftNode.Line;
                    casteoLeft.NodeText = leftNode.NodeText;
                    casteoLeft.NodeType = PlanNodeTypeEnum.Cast;
                    casteoLeft.Properties.Add("DataType", selectedType.ToString());
                    casteoLeft.Children = new List<PlanNode>();
                    casteoLeft.Children.Add(leftNode);
                    this.result.Children.Add(casteoLeft);
                }

                PlanNode casteoRight = null;
                if (validate.ConvertRightNode)
                {
                    casteoRight = new PlanNode();
                    casteoRight.Column = rightNode.Column;
                    casteoRight.Line = rightNode.Line;
                    casteoRight.NodeText = rightNode.NodeText;
                    casteoRight.NodeType = PlanNodeTypeEnum.Cast;
                    casteoRight.Properties.Add("DataType", selectedType.ToString());
                    casteoRight.Children = new List<PlanNode>();
                    casteoRight.Children.Add(rightNode);
                    this.result.Children.Add(casteoRight);
                }

                if (casteoLeft == null)
                {
                    this.result.Children.Add(leftNode);
                }

                if (casteoRight == null)
                {
                    this.result.Children.Add(rightNode);
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
                    PlanNode casteo = new PlanNode();
                    casteo.Column = rightNode.Column;
                    casteo.Line = rightNode.Line;
                    casteo.NodeText = rightNode.NodeText;
                    casteo.NodeType = PlanNodeTypeEnum.Cast;
                    casteo.Properties.Add("DataType", typeof(bool).ToString());
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
        /// check if is a numeric type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if is numeric type</returns>
        public bool IsNumericType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            if (type.Equals(typeof(float)))
            {
                return true;
            }

            return false;
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
                    case "==":
                    case "!=":
                        if (leftType != rightType)
                        {
                            if (leftType != null && rightType != null)
                            {
                                error.Message = "No es posible realizar la operación '" + operacion + "' con los tipos " + leftType + " y " + rightType;
                                errores.AlmacenarError(error);
                            }
                        }

                        break;
                    case "<=":
                    case "<":
                    case ">=":
                    case ">":
                        if (!this.IsNumericType(leftType) || !this.IsNumericType(rightType))
                        {
                            if (!leftType.Equals(typeof(DateTime)) || !rightType.Equals(typeof(DateTime)))
                            {
                                if (leftType != null && rightType != null)
                                {
                                    if (!leftType.Equals(typeof(object)) || !rightType.Equals(typeof(object)))
                                    {
                                        error.Message = "La operación '" + operacion + "' solo puede realizarse con tipos de dato numérico y se intentó con " + leftType + " y " + rightType;
                                        errores.AlmacenarError(error);
                                    }
                                }
                                else
                                {
                                    error.Message = "La operación '" + operacion + "' solo puede realizarse con tipos de dato numérico y se intentó con " + leftType + " y " + rightType;
                                    errores.AlmacenarError(error);
                                }
                            }
                        }

                        break;
                    case "like":
                        if (leftType != null && rightType != null)
                        {
                            if (!leftType.Equals(typeof(string)) || !rightType.Equals(typeof(string)))
                            {
                                error.Message = "La operación '" + operacion + "' solo puede realizarse con tipos de dato cadena y se intentó con " + leftType + " y " + rightType;
                                errores.AlmacenarError(error);
                            }
                        }
                        else
                        {
                            error.Message = "La operación '" + operacion + "' solo puede realizarse con tipos de dato cadena y se intentó con " + leftType + " y " + rightType;
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
                error.Message = "No es posible realizar una operación '" + operacion + "' con los tipos de dato " + leftType + " y " + rightType;
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

            if (childrenCount == 3)
            {
                this.BeginEvaluate(thread);
                PlanNode l = (PlanNode)this.leftNode.Evaluate(thread);
                PlanNode r = (PlanNode)this.rightNode.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText = l.NodeText + " " + this.operationNode + " " + r.NodeText;
                this.SelectOperation(this.operationNode, thread);
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
                this.SelectOperation(this.operationNode, thread);
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
