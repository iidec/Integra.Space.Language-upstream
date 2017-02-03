//-----------------------------------------------------------------------
// <copyright file="ComparativeExpressionASTNode.cs" company="Integra.Space.Language">
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
    internal sealed class ComparativeExpressionASTNode : AstNodeBase
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
        private AstNodeBase rightNode1;

        /// <summary>
        /// right part of the 'between' operator at the 'and' section 
        /// </summary>
        private AstNodeBase rightNode2;

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
                this.rightNode1 = AddChild(NodeUseType.Parameter, "rightNode", ChildrenNodes[2]) as AstNodeBase;
            }
            else if (childrenCount == 2)
            {
                this.rightNode1 = AddChild(NodeUseType.Parameter, "rightNode", ChildrenNodes[1]) as AstNodeBase;
                this.operationNode = (string)ChildrenNodes[0].Token.Value;
            }
            else if (childrenCount == 5)
            {
                this.leftNode = AddChild(NodeUseType.Parameter, "leftNode", ChildrenNodes[0]) as AstNodeBase;
                this.operationNode = (string)ChildrenNodes[1].Token.Value;
                this.rightNode1 = AddChild(NodeUseType.Parameter, "rightNode1", ChildrenNodes[2]) as AstNodeBase;
                this.rightNode2 = AddChild(NodeUseType.Parameter, "rightNode2", ChildrenNodes[4]) as AstNodeBase;
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
                    case "==":
                        resultType = PlanNodeTypeEnum.Equal;
                        break;
                    case "!=":
                        resultType = PlanNodeTypeEnum.NotEqual;
                        break;
                    case "<=":
                        resultType = PlanNodeTypeEnum.LessThanOrEqual;
                        break;
                    case "<":
                        resultType = PlanNodeTypeEnum.LessThan;
                        break;
                    case ">=":
                        resultType = PlanNodeTypeEnum.GreaterThanOrEqual;
                        break;
                    case ">":
                        resultType = PlanNodeTypeEnum.GreaterThan;
                        break;
                    case "like":
                        resultType = PlanNodeTypeEnum.Like;
                        break;
                    case "not":
                        resultType = PlanNodeTypeEnum.Not;
                        break;
                    case "between":
                        resultType = PlanNodeTypeEnum.And;
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
        /// <returns>The result plan nodes for left and right nodes</returns>
        public Tuple<PlanNode, PlanNode> CreateChildrensForResult(PlanNode leftNode, PlanNode rightNode, ScriptThread thread)
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

            TypeValidation validate = new TypeValidation(leftType, rightType, this.result, thread);
            Type selectedType = validate.SelectTypeToCast();
            Tuple<PlanNode, PlanNode> childResult;

            if (selectedType != null)
            {
                PlanNode leftResultNode = null;
                if (validate.ConvertLeftNode)
                {
                    leftResultNode = new PlanNode(leftNode.Line, leftNode.Column, PlanNodeTypeEnum.Cast);
                    leftResultNode.NodeText = leftNode.NodeText;
                    leftResultNode.Properties.Add("DataType", selectedType);
                    leftResultNode.Children = new List<PlanNode>();
                    leftResultNode.Children.Add(leftNode);
                }

                PlanNode rightResultNode = null;
                if (validate.ConvertRightNode)
                {
                    rightResultNode = new PlanNode(rightNode.Line, rightNode.Column, PlanNodeTypeEnum.Cast);
                    rightResultNode.NodeText = rightNode.NodeText;
                    rightResultNode.Properties.Add("DataType", selectedType);
                    rightResultNode.Children = new List<PlanNode>();
                    rightResultNode.Children.Add(rightNode);
                }

                if (leftResultNode == null)
                {
                    leftResultNode = leftNode;
                }

                if (rightResultNode == null)
                {
                    rightResultNode = rightNode;
                }

                childResult = Tuple.Create(leftResultNode, rightResultNode);
            }
            else
            {
                childResult = Tuple.Create(leftNode, rightNode);
            }

            return childResult;
        }

        /// <summary>
        /// CreateChildrenForResult
        /// Create the children of the actual node
        /// </summary>
        /// <param name="rightNode">right child</param>
        /// <param name="thread">actual thread</param>
        /// <returns>The result plan node for 'not' operation.</returns>
        public PlanNode CreateChildrensForResult(PlanNode rightNode, ScriptThread thread)
        {
            Type leftType = typeof(bool);
            Type rightType = null;

            if (rightNode.Properties.ContainsKey("DataType"))
            {
                rightType = Type.GetType(rightNode.Properties["DataType"].ToString());
            }
            
            TypeValidation validate = new TypeValidation(leftType, rightType, this.result, thread);
            Type selectedType = validate.SelectTypeToCast();
            PlanNode resultChild;

            if (selectedType != null && rightType != null)
            {
                if (rightType.Equals(typeof(object)))
                {
                    PlanNode casteo = new PlanNode(rightNode.Line, rightNode.Column, PlanNodeTypeEnum.Cast);
                    casteo.NodeText = rightNode.NodeText;
                    casteo.Properties.Add("DataType", typeof(bool));
                    casteo.Children = new List<PlanNode>();
                    casteo.Children.Add(rightNode);
                    resultChild = casteo;
                }
                else
                {
                    resultChild = rightNode;
                }
            }
            else
            {
                resultChild = rightNode;
            }

            return resultChild;
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

            this.result = new PlanNode(this.Location.Line, this.Location.Column, this.SelectOperation(this.operationNode, thread));
            this.result.Properties.Add("DataType", typeof(bool));

            if (childrenCount == 3)
            {
                this.BeginEvaluate(thread);
                PlanNode l = (PlanNode)this.leftNode.Evaluate(thread);
                PlanNode r = (PlanNode)this.rightNode1.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText = l.NodeText + " " + this.operationNode + " " + r.NodeText;

                Tuple<PlanNode, PlanNode> lr = this.CreateChildrensForResult(l, r, thread);
                this.result.Children = new List<PlanNode>();
                this.result.Children.Add(lr.Item1);
                this.result.Children.Add(lr.Item2);

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
                PlanNode r = (PlanNode)this.rightNode1.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText = this.operationNode + "(" + r.NodeText + ")";

                this.result.Children = new List<PlanNode>();
                PlanNode child = this.CreateChildrensForResult(r, thread);
                this.result.Children.Add(child);

                this.ValidateTypesForOperation(r, this.operationNode, thread);
                this.result.Properties.Add("IsConstant", bool.Parse(r.Properties["IsConstant"].ToString()));
            }
            else if (childrenCount == 5)
            {
                this.BeginEvaluate(thread);
                PlanNode l = (PlanNode)this.leftNode.Evaluate(thread);
                PlanNode r1 = (PlanNode)this.rightNode1.Evaluate(thread);
                PlanNode r2 = (PlanNode)this.rightNode2.Evaluate(thread);
                this.EndEvaluate(thread);

                this.result.NodeText = l.NodeText + " " + this.operationNode + " " + r1.NodeText + " and " + r2.NodeText;

                this.operationNode = ">";
                Tuple<PlanNode, PlanNode> lr1 = this.CreateChildrensForResult(l, r1, thread);
                PlanNode auxIz = new PlanNode(ChildrenNodes[1].Token.Location.Line, ChildrenNodes[1].Token.Location.Column, this.SelectOperation(this.operationNode, thread));
                auxIz.NodeText = this.result.NodeText;
                auxIz.Properties.Add("DataType", typeof(bool));
                auxIz.Children = new List<PlanNode>();
                auxIz.Children.Add(lr1.Item1);
                auxIz.Children.Add(lr1.Item2);
                this.ValidateTypesForOperation(l, r1, this.operationNode, thread);

                if (bool.Parse(l.Properties["IsConstant"].ToString()) == true || bool.Parse(r1.Properties["IsConstant"].ToString()) == true)
                {
                    auxIz.Properties.Add("IsConstant", true);
                }
                else
                {
                    auxIz.Properties.Add("IsConstant", false);
                }

                this.operationNode = "<";
                PlanNode leftNodecloned = l.Clone();
                Tuple<PlanNode, PlanNode> lr2 = this.CreateChildrensForResult(leftNodecloned, r2, thread);
                PlanNode auxDer = new PlanNode(ChildrenNodes[1].Token.Location.Line, ChildrenNodes[1].Token.Location.Column, this.SelectOperation(this.operationNode, thread));
                auxDer.NodeText = this.result.NodeText;
                auxDer.Properties.Add("DataType", typeof(bool));
                auxDer.Children = new List<PlanNode>();
                auxDer.Children.Add(lr2.Item1);
                auxDer.Children.Add(lr2.Item2);
                this.ValidateTypesForOperation(leftNodecloned, r2, this.operationNode, thread);

                if (bool.Parse(leftNodecloned.Properties["IsConstant"].ToString()) == true || bool.Parse(r2.Properties["IsConstant"].ToString()) == true)
                {
                    auxDer.Properties.Add("IsConstant", true);
                }
                else
                {
                    auxDer.Properties.Add("IsConstant", false);
                }

                this.result.Children = new List<PlanNode>();
                this.result.Children.Add(auxIz);
                this.result.Children.Add(auxDer);

                if ((bool.Parse(r1.Properties["IsConstant"].ToString()) == true && bool.Parse(r2.Properties["IsConstant"].ToString()) == true) || bool.Parse(l.Properties["IsConstant"].ToString()) == true)
                {
                    this.result.Properties.Add("IsConstant", true);
                }
                else
                {
                    this.result.Properties.Add("IsConstant", false);
                }
            }
            else
            {
                this.result = (PlanNode)this.leftNode.Evaluate(thread);
            }

            return this.result;
        }
    }
}
