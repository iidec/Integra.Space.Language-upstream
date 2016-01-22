//-----------------------------------------------------------------------
// <copyright file="ObservableConstructor.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive.Linq;
    using System.Reflection;
    using Exceptions;
    using Messaging;

    /// <summary>
    /// Observable constructor class
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1111:ClosingParenthesisMustBeOnLineOfLastParameter", Justification = "Reviewed.")]
    internal class ObservableConstructor
    {
        /// <summary>
        /// List to store parameter expressions
        /// </summary>
        private List<ParameterExpression> parameterList;

        /// <summary>
        /// Stack to store scope variables
        /// </summary>
        private Stack<ParameterExpression> scopeParam;

        /// <summary>
        /// Group expression
        /// </summary>
        private Expression groupExpression;
        
        /// <summary>
        /// Object prefix
        /// </summary>
        private string objectPrefix;

        /// <summary>
        /// Projection with dispose.
        /// </summary>
        private bool projectionWithDispose;

        /// <summary>
        /// Compilation context
        /// </summary>
        private CompileContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConstructor"/> class
        /// </summary>
        public ObservableConstructor()
        {
            this.context = new CompileContext();

            this.scopeParam = new Stack<ParameterExpression>();
            this.parameterList = new List<ParameterExpression>();
            this.projectionWithDispose = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConstructor"/> class
        /// </summary>
        /// <param name="context">Compilation context.</param>
        public ObservableConstructor(CompileContext context)
        {
            this.context = context;

            this.scopeParam = new Stack<ParameterExpression>();
            this.parameterList = new List<ParameterExpression>();
            this.projectionWithDispose = false;
        }

        /// <summary>
        /// Creates a delegate
        /// </summary>
        /// <param name="plan">Execution plan</param>
        /// <param name="inputType">Input type.</param>
        /// <param name="outputType">Output type.</param>
        /// <returns>Result delegate.</returns>
        public Delegate Compile(PlanNode plan, Type inputType, Type outputType)
        {
            var delegateType = typeof(Func<,>).MakeGenericType(inputType, outputType);
            Delegate result = Expression.Lambda(delegateType, this.GenerateExpressionTree(plan), this.parameterList.ToArray()).Compile();

            this.scopeParam = new Stack<ParameterExpression>();
            this.parameterList = new List<ParameterExpression>();
            this.projectionWithDispose = false;

            Console.WriteLine("La función fue compilada exitosamente.");
            return result;
        }

        /// <summary>
        /// Compile the result function.
        /// </summary>
        /// <typeparam name="In">Input type.</typeparam>
        /// <typeparam name="Out">Output type.</typeparam>
        /// <param name="plan">Execution plan.</param>
        /// <returns>Result function.</returns>
        public Func<In, Out> Compile<In, Out>(PlanNode plan)
        {
            Func<In, Out> funcResult = this.CreateLambda<In, Out>(plan).Compile();

            this.scopeParam = new Stack<ParameterExpression>();
            this.parameterList = new List<ParameterExpression>();
            this.projectionWithDispose = false;

            Console.WriteLine("La función fue compilada exitosamente.");
            return funcResult;
        }

        /// <summary>
        /// Compile the result function.
        /// </summary>
        /// <typeparam name="Out">Output type.</typeparam>
        /// <param name="plan">Execution plan.</param>
        /// <returns>Result function.</returns>
        public Func<Out> Compile<Out>(PlanNode plan)
        {
            Func<Out> funcResult = this.CreateLambda<Out>(plan).Compile();

            this.scopeParam = new Stack<ParameterExpression>();
            this.parameterList = new List<ParameterExpression>();
            this.projectionWithDispose = false;

            Console.WriteLine("La función fue compilada exitosamente.");
            return funcResult;
        }

        /// <summary>
        /// Creates a lambda expression
        /// </summary>
        /// <typeparam name="In">Input type</typeparam>
        /// <typeparam name="Out">Output type</typeparam>
        /// <param name="plan">Execution plan</param>
        /// <returns>Expression lambda</returns>
        public Expression<Func<In, Out>> CreateLambda<In, Out>(PlanNode plan)
        {
            Expression rootExpression = this.GenerateExpressionTree(plan);
            Expression<Func<In, Out>> result = Expression.Lambda<Func<In, Out>>(rootExpression, this.parameterList.ToArray());
            return result;
        }

        /// <summary>
        /// Creates a lambda expression
        /// </summary>
        /// <typeparam name="In">Input type</typeparam>
        /// <typeparam name="IScheduler">Scheduler type</typeparam>
        /// <typeparam name="Out">Output type</typeparam>
        /// <param name="plan">Execution plan</param>
        /// <returns>Expression lambda</returns>
        public Expression<Func<In, IScheduler, Out>> CreateLambda<In, IScheduler, Out>(PlanNode plan)
        {
            Expression rootExpression = this.GenerateExpressionTree(plan);            
            Expression<Func<In, IScheduler, Out>> result = Expression.Lambda<Func<In, IScheduler, Out>>(rootExpression, this.parameterList.ToArray());

            return result;
        }

        /// <summary>
        /// Creates a lambda expression
        /// </summary>
        /// <typeparam name="Out">Output type</typeparam>
        /// <param name="plan">Execution plan</param>
        /// <returns>Expression lambda</returns>
        public Expression<Func<Out>> CreateLambda<Out>(PlanNode plan)
        {
            Expression<Func<Out>> result = Expression.Lambda<Func<Out>>(this.GenerateExpressionTree(plan), this.parameterList.ToArray());
            return result;
        }

        /// <summary>
        /// Create a expression tree.
        /// </summary>
        /// <param name="plan">plan node to convert.</param>
        /// <returns>expression tree of actual plan.</returns>
        public Expression GenerateExpressionTree(PlanNode plan)
        {
            if (plan == null)
            {
                return null;
            }

            Expression leftExp = null;
            Expression rightExp = null;

            if (plan.Children != null)
            {
                if (plan.Children.Count() == 2)
                {
                    leftExp = this.GenerateExpressionTree(plan.Children.First<PlanNode>());

                    // se hace porque la proyección es un árbol independiente y solo el resultado, un lambda expression, debe ser el nodo derecho
                    if (plan.Children.ElementAt<PlanNode>(1).NodeType.Equals(PlanNodeTypeEnum.Projection))
                    {
                        rightExp = this.CreateProjectionExpression(plan.Children.ElementAt<PlanNode>(1));
                    }
                    else if (plan.Children.ElementAt<PlanNode>(1).NodeType.Equals(PlanNodeTypeEnum.ProjectionOfConstants))
                    {
                        rightExp = this.CreateProjectionOfConstantsExpression(plan.Children.ElementAt<PlanNode>(1));
                    }
                    else
                    {
                        rightExp = this.GenerateExpressionTree(plan.Children.ElementAt<PlanNode>(1));
                    }
                }
                else if (plan.Children.Count() == 1)
                {
                    leftExp = this.GenerateExpressionTree(plan.Children.First<PlanNode>());
                }
            }

            return this.CreateExpressionNode(plan, leftExp, rightExp);
        }

        /// <summary>
        /// Creates a expression tree.
        /// </summary>
        /// <param name="actualNode">Actual plan.</param>
        /// <param name="leftNode">Left child expression.</param>
        /// <param name="rightNode">Right child expression.</param>
        /// <returns>Expression tree of actual plan.</returns>
        private Expression CreateExpressionNode(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            PlanNodeTypeEnum nodeType = actualNode.NodeType;
            Expression expResult;

            switch (nodeType)
            {
                case PlanNodeTypeEnum.Constant:
                    expResult = this.GenerateConstant(actualNode);
                    break;
                case PlanNodeTypeEnum.Identifier:
                    expResult = this.GenerateIdentifier(actualNode);
                    break;
                case PlanNodeTypeEnum.StringLeftFunction:
                    Func<PlanNode, Expression, Expression> leftFunctionMethod = this.CreateStringLeftFunction;
                    expResult = leftFunctionMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.StringRightFunction:
                    Func<PlanNode, Expression, Expression> rightFunctionMethod = this.CreateStringRightFunction;
                    expResult = rightFunctionMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.StringUpperFunction:
                    Func<PlanNode, Expression, Expression> upperFunctionMethod = this.CreateStringUpperFunction;
                    expResult = upperFunctionMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.StringLowerFunction:
                    Func<PlanNode, Expression, Expression> lowerFunctionMethod = this.CreateStringLowerFunction;
                    expResult = lowerFunctionMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.Projection:
                    expResult = this.CreateProjectionExpression(actualNode);
                    break;
                case PlanNodeTypeEnum.EnumerableCount:
                    Func<PlanNode, Expression, Expression> enumerableCountMethod = this.CreateEnumerableCount<int>;
                    expResult = enumerableCountMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type })
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableSum:
                case PlanNodeTypeEnum.EnumerableMax:
                case PlanNodeTypeEnum.EnumerableMin:
                    Func<PlanNode, Expression, Expression, Expression> enumerableSumMethod = this.CreateEnumerableSum<int>;
                    expResult = enumerableSumMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableTake:
                    Func<PlanNode, Expression, Expression, Expression> enumerableTakeMethod = this.CreateEnumerableTake;
                    expResult = enumerableTakeMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.ObservableWhere:
                    Func<PlanNode, Expression, Expression, Expression> whereMethod = this.CreateObservableWhere<int>;
                    expResult = whereMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[0] })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.ObservableTake:
                    Func<PlanNode, Expression, Expression, Expression> observableTakeMethod = this.CreateObservableTake;
                    expResult = observableTakeMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.ObservableGroupBy:
                    Func<PlanNode, Expression, Expression, Expression> groupbyMethod = this.CreateObservableGroupBy<int, int>;
                    expResult = groupbyMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[0], rightNode.Type.GetGenericArguments()[1] })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableGroupBy:
                    Func<PlanNode, Expression, Expression, Expression> enumerableGroupbyMethod = this.CreateEnumerableGroupBy;
                    expResult = enumerableGroupbyMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableOrderBy:
                case PlanNodeTypeEnum.EnumerableOrderByDesc:
                    Func<PlanNode, Expression, Expression, Expression> enumerableOrderbyMethod = this.CreateEnumerableOrderBy;
                    expResult = enumerableOrderbyMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.ObservableWhereForEventLock:
                    Func<PlanNode, Expression, Expression> selectForEventLockMethod = this.CreateWhereForEventLock;
                    expResult = selectForEventLockMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.ObservableSelectForGroupBy:
                    Func<PlanNode, Expression, Expression, Expression> selectForGroupByMethod = this.CreateSelectForObservableGroupBy;
                    expResult = selectForGroupByMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableSelectForGroupBy:
                    Func<PlanNode, Expression, Expression, Expression> selectForEnumerableGroupByMethod = this.CreateSelectForEnumerableGroupBy;
                    expResult = selectForEnumerableGroupByMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.ObservableSelectForBuffer:
                    Func<PlanNode, Expression, Expression, Expression> selectForBufferMethod = this.CreateSelectForObservableBufferOrSource;
                    expResult = selectForBufferMethod.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableSelectForEnumerable:
                    Func<PlanNode, Expression, Expression, Expression> selectForEnumerable = this.CreateSelectForEnumerable;
                    expResult = selectForEnumerable.Method
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableToList:
                    Func<PlanNode, Expression, Expression> enumerableToList = this.EnumerableToList;
                    expResult = enumerableToList.Method
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.ObservableBuffer:
                    Func<PlanNode, Expression, Expression, Expression> bufferMethod = this.CreateObservableBuffer<int>;
                    if (leftNode.Type.Name.Equals("IGroupedObservable`2") || leftNode.Type.Name.Equals("IGrouping`2"))
                    {
                        expResult = bufferMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[1] })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    }
                    else
                    {
                        expResult = bufferMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[0] })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    }

                    break;
                case PlanNodeTypeEnum.ObservableBufferTimeAndSize:
                    Func<PlanNode, Expression, Expression, Expression> bufferTimeAndSizeMethod = this.CreateObservableBufferTimeAndSize<int>;
                    if (leftNode.Type.Name.Equals("IGroupedObservable`2") || leftNode.Type.Name.Equals("IGrouping`2"))
                    {
                        expResult = bufferTimeAndSizeMethod.Method.GetGenericMethodDefinition()
                                                                    .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[1] })
                                                                    .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    }
                    else
                    {
                        expResult = bufferTimeAndSizeMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[0] })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    }

                    break;
                case PlanNodeTypeEnum.ObservableFrom:
                    expResult = this.CreateFrom(actualNode, (ConstantExpression)leftNode);
                    break;
                case PlanNodeTypeEnum.ObservableFromForLambda:
                    expResult = this.CreateFromForLambda(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.ObservableMerge:
                    Func<PlanNode, Expression, Expression> mergeMethod = this.CreateObservableMerge<int, int>;
                    expResult = mergeMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type, leftNode.Type.GetGenericArguments()[0] })
                                            .Invoke(this, new object[] { actualNode, leftNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.NewScope:
                    this.CreateNewScope(leftNode);
                    return leftNode;
                case PlanNodeTypeEnum.GroupKey:
                    expResult = this.GenerateGroupKey(actualNode);
                    break;
                case PlanNodeTypeEnum.GroupKeyProperty:
                    expResult = this.GenerateGroupKeyProperty(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.GroupKeyValue:
                    expResult = this.GenerateGroupPropertyValue(actualNode, leftNode);
                    break;
                /********************************************************************************************************************************************************************************************************************/
                case PlanNodeTypeEnum.Cast:
                    return this.GenerateCast(actualNode, leftNode);
                case PlanNodeTypeEnum.Equal:
                    return this.GenerarEqual(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.NotEqual:
                    return this.GenerarNotEqual(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.LessThan:
                    return this.GenerarLessThan(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.LessThanOrEqual:
                    return this.GenerarLessThanOrEqual(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.GreaterThan:
                    return this.GenerarGreaterThan(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.GreaterThanOrEqual:
                    return this.GenerarGreaterThanOrEqual(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.Not:
                    return this.GenerarNot(actualNode, leftNode);
                case PlanNodeTypeEnum.Like:
                    return this.GenerarLike(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.Or:
                    return this.GenerateOr(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.And:
                    return this.GenerateAnd(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.ObjectPart:
                    return this.GenerateObjectPart(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.ObjectField:
                    if (actualNode.Children.First<PlanNode>().NodeType.Equals(PlanNodeTypeEnum.ObjectPart))
                    {
                        return this.GenerateObjectFieldFromPart(actualNode, leftNode, rightNode);
                    }
                    else
                    {
                        return this.GenerateObjectFieldFromField(actualNode, leftNode, rightNode);
                    }

                case PlanNodeTypeEnum.ObjectValue:
                    return this.GenerateValueOfObject(actualNode, leftNode);
                case PlanNodeTypeEnum.ObjectMessage:
                    return this.GenerateObjectMessage(actualNode, (ParameterExpression)leftNode);
                case PlanNodeTypeEnum.Event:
                    return this.GenerateEvent(actualNode);
                case PlanNodeTypeEnum.ObjectPrefix:
                    return this.GenerateObjectPrefix(actualNode);
                case PlanNodeTypeEnum.DateTimeFunction:
                    return this.GenerateDateFunction(actualNode, leftNode);
                case PlanNodeTypeEnum.Negate:
                    return this.GenerateNegate(actualNode, leftNode);
                case PlanNodeTypeEnum.Subtract:
                    return this.GenerateSubtract(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.Property:
                    return this.GenerateProperty(actualNode, leftNode);
                case PlanNodeTypeEnum.ObjectWithPrefix:
                    return this.GetEventWithPrefixValue(actualNode, leftNode, rightNode);
                default:
                    throw new CompilationException(Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE1, string.Empty));
            }

            return expResult;
        }

        /// <summary>
        /// Creates the source parameter expression.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="source">Name of the source.</param>
        /// <returns>Source parameter expression.</returns>
        private Expression CreateFrom(PlanNode actualNode, ConstantExpression source)
        {
            // se obtiene el SpaceContext del proyecto Integra.Space.Services
            /*string sourceName = source.Value.ToString() + "_converted";
            PropertyInfo propertyInfo = typeof(Integra.Space.Services.Signalr.Contexts.SpaceContext).GetProperty(sourceName);

            if (propertyInfo == null)
            {
                throw new NotImplementedException(string.Format("Fuente '{0}' no implementada.", sourceName));
            }*/

            ParameterExpression currentParameter = null;
            bool existsParameter = false;
            string parameterName;
            parameterName = source.Value.ToString();

            foreach (ParameterExpression parameter in this.parameterList)
            {
                if (parameter.Name.Equals(parameterName))
                {
                    currentParameter = parameter;
                    existsParameter = true;
                }
            }

            if (!existsParameter)
            {
                currentParameter = Expression.Parameter(typeof(IObservable<EventObject>), parameterName);
                this.parameterList.Add(currentParameter);
            }

            return currentParameter;
        }

        /// <summary>
        /// Return the scope variable, representing a source, for the actual lambda expression scope.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="leftNode">Left expression node.</param>
        /// <returns>The scope variable.</returns>
        private Expression CreateFromForLambda(PlanNode actualNode, Expression leftNode)
        {
            try
            {
                return this.scopeParam.Peek();
            }
            catch (InvalidOperationException e)
            {
                throw new CompilationException(Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE2, string.Empty), e);
            }
        }

        /// <summary>
        /// Add a new scope variable to the scope variables stack.
        /// </summary>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        private void CreateNewScope(Expression incomingObservable)
        {
            this.CreateNewScope(incomingObservable, null);
        }

        /// <summary>
        /// Add a new scope variable to the scope variables stack.
        /// </summary>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="paramName">Parameter name.</param>
        private void CreateNewScope(Expression incomingObservable, string paramName)
        {
            // si tiene parametros genéricos, creo una nueva variable global para el nuevo ambiente
            // de lo contrario se utilizará la que se tiene actualmente al tope de la pila
            if (incomingObservable.Type.GetGenericArguments().Count() > 0)
            {
                if (incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2"))
                {
                    this.scopeParam.Push(Expression.Parameter(incomingObservable.Type.GetGenericArguments()[1], paramName));
                }
                else
                {
                    this.scopeParam.Push(Expression.Parameter(incomingObservable.Type.GetGenericArguments()[0], paramName));
                }

                if (incomingObservable.Type.GetGenericArguments()[0].Name.Equals("IGroupedObservable`2") || incomingObservable.Type.GetGenericArguments()[0].Name.Equals("IGrouping`2"))
                {
                    this.groupExpression = this.scopeParam.Peek();
                }
            }
            else
            {
                this.scopeParam.Push(Expression.Parameter(incomingObservable.Type, paramName));
            }
        }

        /// <summary>
        /// Creates a new Substring expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <returns>Enumerable count expression.</returns>
        private Expression CreateStringLeftFunction(PlanNode actualNode, Expression incomingObservable)
        {
            if (!incomingObservable.Type.Equals(typeof(string)))
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE3, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(typeof(string), "resultFunctionLeft");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression cadenaResultante = Expression.Variable(incomingObservable.Type, "CadenaResultante");

            Expression numberExp = this.GenerateExpressionTree((PlanNode)actualNode.Properties["Number"]);

            try
            {
                MethodInfo method = typeof(string).GetMethods().Where(m => m.Name == "Substring" && m.GetParameters().Length == 2).Single();

                Expression tryCatchExpr =
                      Expression.Block(
                          new[] { cadenaResultante, param },
                          Expression.Assign(cadenaResultante, incomingObservable),
                          Expression.IfThenElse(
                            Expression.Equal(cadenaResultante, Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(typeof(string))),
                              Expression.TryCatch(
                                  Expression.Block(
                                      Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'left' function"))),
                                      Expression.Assign(param, Expression.Call(cadenaResultante, method, Expression.Constant(0), numberExp)),
                                      Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'left' function"))),
                                      Expression.Empty()
                                      ),
                                  Expression.Catch(
                                      paramException,
                                       Expression.Block(
                                          Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                          Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE1, actualNode.NodeText), typeof(string)), paramException))
                                      )
                                  )
                              )
                              ),
                          param
                          );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE4, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Substring expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <returns>Enumerable count expression.</returns>
        private Expression CreateStringRightFunction(PlanNode actualNode, Expression incomingObservable)
        {
            if (!incomingObservable.Type.Equals(typeof(string)))
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE5, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(typeof(string), "resultFunctionLeft");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            Expression numberExp = this.GenerateExpressionTree((PlanNode)actualNode.Properties["Number"]);
            Expression substringExp = Expression.Subtract(Expression.Property(incomingObservable, "Length"), numberExp);
            ParameterExpression cadenaResultante = Expression.Variable(incomingObservable.Type, "CadenaResultante");

            try
            {
                MethodInfo method = typeof(string).GetMethods().Where(m => m.Name == "Substring" && m.GetParameters().Length == 1).Single();

                Expression tryCatchExpr =
                      Expression.Block(
                          new[] { cadenaResultante, param },
                          Expression.Assign(cadenaResultante, incomingObservable),
                          Expression.IfThenElse(
                            Expression.Equal(cadenaResultante, Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(typeof(string))),
                              Expression.TryCatch(
                                  Expression.Block(
                                      Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'right' function"))),
                                      Expression.Assign(param, Expression.Call(cadenaResultante, method, substringExp)),
                                      Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'right' function"))),
                                      Expression.Empty()
                                      ),
                                  Expression.Catch(
                                      paramException,
                                       Expression.Block(
                                          Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                          Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE10, actualNode.NodeText), typeof(string)), paramException))
                                      )
                                  )
                              )
                              ),
                          param
                          );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE6, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Substring expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <returns>Enumerable count expression.</returns>
        private Expression CreateStringUpperFunction(PlanNode actualNode, Expression incomingObservable)
        {
            if (!incomingObservable.Type.Equals(typeof(string)))
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE7, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(typeof(string), "resultFunctionLeft");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression cadenaResultante = Expression.Variable(incomingObservable.Type, "CadenaResultante");

            try
            {
                MethodInfo method = typeof(string).GetMethods().Where(m => m.Name == "ToUpper" && m.GetParameters().Length == 0).Single();

                Expression tryCatchExpr =
                      Expression.Block(
                          new[] { cadenaResultante, param },
                          Expression.Assign(cadenaResultante, incomingObservable),
                          Expression.IfThenElse(
                            Expression.Equal(cadenaResultante, Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(typeof(string))),
                              Expression.TryCatch(
                                  Expression.Block(
                                      Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'upper' function"))),
                                      Expression.Assign(param, Expression.Call(cadenaResultante, method)),
                                      Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'upper' function"))),
                                      Expression.Empty()
                                      ),
                                  Expression.Catch(
                                      paramException,
                                       Expression.Block(
                                          Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                          Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE11, actualNode.NodeText), typeof(string)), paramException))
                                      )
                                  )
                              )
                              ),
                          param
                          );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE8, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Substring expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <returns>Enumerable count expression.</returns>
        private Expression CreateStringLowerFunction(PlanNode actualNode, Expression incomingObservable)
        {
            if (!incomingObservable.Type.Equals(typeof(string)))
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE9, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(typeof(string), "resultFunctionLeft");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression cadenaResultante = Expression.Variable(incomingObservable.Type, "CadenaResultante");

            try
            {
                MethodInfo method = typeof(string).GetMethods().Where(m => m.Name == "ToLower" && m.GetParameters().Length == 0).Single();

                Expression tryCatchExpr =
                      Expression.Block(
                          new[] { cadenaResultante, param },
                          Expression.Assign(cadenaResultante, incomingObservable),
                          Expression.IfThenElse(
                            Expression.Equal(cadenaResultante, Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(typeof(string))),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'lower' function"))),
                                    Expression.Assign(param, Expression.Call(cadenaResultante, method)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'lower' function"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE12, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                            ),
                          param
                          );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE10, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Enumerable.Count expression.
        /// </summary>
        /// <typeparam name="I">Input type.</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <returns>Enumerable count expression.</returns>
        private Expression CreateEnumerableCount<I>(PlanNode actualNode, Expression incomingObservable)
        {
            ParameterExpression param = Expression.Variable(typeof(int), "resultExtensionWithoutParametersObservable");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            try
            {
                if (!incomingObservable.Type.Namespace.Contains("System.Collections") && !incomingObservable.Type.Name.Equals("IGroupedObservable`2") && !incomingObservable.Type.Name.Equals("IGrouping`2"))
                {
                    if (this.scopeParam.Where(x => x.Type.Namespace.Contains("System.Collections") || incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2")).Count() > 0)
                    {
                        incomingObservable = this.scopeParam.Where(x => x.Type.Namespace.Contains("System.Collections") || incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2")).Last();
                    }
                    else
                    {
                        throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE11, actualNode.NodeText));
                    }
                }

                MethodInfo methodCount = null;
                if (incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2"))
                {
                    methodCount = typeof(System.Linq.Enumerable).GetMethods().Where(m => m.Name == "Count" && m.GetParameters().Length == 1).Single().MakeGenericMethod(incomingObservable.Type.GenericTypeArguments[1]);
                }
                else
                {
                    methodCount = typeof(System.Linq.Enumerable).GetMethods().Where(m => m.Name == "Count" && m.GetParameters().Length == 1).Single().MakeGenericMethod(incomingObservable.Type.GenericTypeArguments[0]);
                }

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'count' function"))),
                                    Expression.Assign(param, Expression.Call(methodCount, incomingObservable)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'count' function"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE13, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE12, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Enumerable.Sum expression.
        /// </summary>
        /// <typeparam name="I">Input type.</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="selector">Selector expression.</param>
        /// <returns>Enumerable sum expression.</returns>
        private Expression CreateEnumerableSum<I>(PlanNode actualNode, Expression incomingObservable, Expression selector)
        {
            try
            {
                ParameterExpression param = Expression.Variable(selector.Type, "resultExtensionWithoutParametersObservable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                if (!incomingObservable.Type.Namespace.Contains("System.Collections") && !incomingObservable.Type.Name.Equals("IGroupedObservable`2") && !incomingObservable.Type.Name.Equals("IGrouping`2"))
                {
                    if (this.scopeParam.Where(x => x.Type.Namespace.Contains("System.Collections") || incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2")).Count() > 0)
                    {
                        incomingObservable = this.scopeParam.Where(x => x.Type.Namespace.Contains("System.Collections") || incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2")).Last();
                    }
                    else
                    {
                        throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE13, actualNode.NodeText));
                    }
                }

                Type incommingTypeForFunction = null;
                if (incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2"))
                {
                    incommingTypeForFunction = incomingObservable.Type.GetGenericArguments()[1];
                }
                else
                {
                    incommingTypeForFunction = incomingObservable.Type.GetGenericArguments()[0];
                }

                string functionName = actualNode.Properties["FunctionName"].ToString();

                Type delegateType = typeof(Func<,>).MakeGenericType(incommingTypeForFunction, selector.Type);
                MethodInfo methodSum = typeof(System.Linq.Enumerable).GetMethods().Where(m => m.Name == functionName && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource," + selector.Type.ToString() + "]")).Single().MakeGenericMethod(incommingTypeForFunction);

                Expression selectorLambda = Expression.Lambda(delegateType, selector, new ParameterExpression[] { this.scopeParam.Pop() });

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'sum' function"))),
                                    Expression.Assign(param, Expression.Call(methodSum, incomingObservable, selectorLambda)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'sum' function"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE14, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE14, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Observable.Buffer expression.
        /// </summary>
        /// <typeparam name="I">Input type.</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="bufferTimeOrSize">Time or size expression for the buffer.</param>
        /// <returns>Observable buffer expression.</returns>
        private Expression CreateObservableBuffer<I>(PlanNode actualNode, Expression incomingObservable, Expression bufferTimeOrSize)
        {
            ParameterExpression result = Expression.Variable(typeof(IObservable<IList<I>>), "ResultObservableBuffer");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            try
            {
                ConstructionValidator.Validate(actualNode.NodeType, null, incomingObservable, bufferTimeOrSize);
                Expression bufferScheduler;

                if (this.context.Scheduler == null)
                {
                    bufferScheduler = Expression.Constant(new System.Reactive.Concurrency.NewThreadScheduler());
                }
                else
                {
                    System.Reactive.Concurrency.IScheduler scheduler = this.context.Scheduler.GetScheduler();
                    bufferScheduler = Expression.Constant(scheduler);
                }

                MethodInfo methodBuffer = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    if (bufferTimeOrSize.Type.Equals(typeof(TimeSpan)))
                    {
                        return m.Name == "Buffer" && m.GetParameters().Length == 3 && m.GetParameters()[1].ParameterType.Equals(bufferTimeOrSize.Type) && m.GetParameters()[2].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler));
                    }
                    else if (bufferTimeOrSize.Type.Equals(typeof(int)))
                    {
                        return m.Name == "Buffer" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.Equals(bufferTimeOrSize.Type);
                    }
                    else
                    {
                        return false;
                    }
                })
                .Single().MakeGenericMethod(typeof(I));

                Expression assign = Expression.Throw(Expression.New(typeof(Exception).GetConstructor(new Type[] { typeof(string), typeof(RuntimeException) }), Expression.Constant(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, "Invalid 'apply window of'.", actualNode.NodeText), typeof(string)), paramException));
                if (bufferTimeOrSize.Type.Equals(typeof(TimeSpan)))
                {
                    assign = Expression.Assign(result, Expression.Call(methodBuffer, incomingObservable, bufferTimeOrSize, bufferScheduler));
                }
                else if (bufferTimeOrSize.Type.Equals(typeof(int)))
                {
                    assign = Expression.Assign(result, Expression.Call(methodBuffer, incomingObservable, bufferTimeOrSize));
                }

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'apply window of' function."))),
                                    assign,
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'apply window of' function.")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE15, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE15, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Observable.Buffer expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="takeSize">Time or size expression for the buffer.</param>
        /// <returns>Observable buffer expression.</returns>
        private Expression CreateObservableTake(PlanNode actualNode, Expression incomingObservable, Expression takeSize)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ResultObservableTake");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodTake = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "Take" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.Equals(takeSize.Type);
                })
                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the observable 'top' function"))),
                                    Expression.Assign(result, Expression.Call(methodTake, incomingObservable, takeSize)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the observable 'top' function")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE16, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE16, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Observable.Buffer expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="takeSize">Time or size expression for the buffer.</param>
        /// <returns>Observable buffer expression.</returns>
        private Expression CreateEnumerableTake(PlanNode actualNode, Expression incomingObservable, Expression takeSize)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ResultEnumerableTake");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodGroupBy = typeof(System.Linq.Enumerable).GetMethods().Where(m =>
                {
                    return m.Name == "Take" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.Equals(takeSize.Type);
                })
                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the enumerable 'top' function."))),
                                    Expression.Assign(result, Expression.Call(methodGroupBy, incomingObservable, takeSize)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable 'top' function.")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE17, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE17, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Observable.Buffer expression.
        /// </summary>
        /// <typeparam name="I">Input type.</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="bufferTimeAndSize">Time or size expression for the buffer.</param>
        /// <returns>Observable buffer expression.</returns>
        private Expression CreateObservableBufferTimeAndSize<I>(PlanNode actualNode, Expression incomingObservable, Expression bufferTimeAndSize)
        {
            try
            {
                ConstructionValidator.Validate(actualNode.NodeType, null, incomingObservable, bufferTimeAndSize);

                ParameterExpression result = Expression.Variable(typeof(IObservable<IList<I>>), "ResultObservableBufferTimeAndSize");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodGroupBy = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "Buffer" && m.GetParameters().Length == 3 && m.GetParameters()[1].ParameterType.Equals(bufferTimeAndSize.Type.GetProperty("TimeSpanValue").PropertyType) && m.GetParameters()[2].ParameterType.Equals(bufferTimeAndSize.Type.GetProperty("IntegerValue").PropertyType);
                })
                .Single().MakeGenericMethod(typeof(I));

                MethodInfo get1 = bufferTimeAndSize.Type.GetProperty("TimeSpanValue").GetMethod;
                MethodInfo get2 = bufferTimeAndSize.Type.GetProperty("IntegerValue").GetMethod;

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the expression observable buffer with time and size"))),
                                    Expression.Assign(result, Expression.Call(methodGroupBy, incomingObservable, Expression.Call(bufferTimeAndSize, get1), Expression.Call(bufferTimeAndSize, get2))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the expression observable buffer with time and size"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE18, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE18, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Observable.Where expression.
        /// </summary>
        /// <typeparam name="I">Input type.</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="filter">Filter expression.</param>
        /// <returns>Observable where expression.</returns>
        private Expression CreateObservableWhere<I>(PlanNode actualNode, Expression incomingObservable, Expression filter)
        {
            ParameterExpression result = Expression.Variable(typeof(IObservable<I>), "ResultObservableWhere");
            ParameterExpression conditionResult = Expression.Variable(typeof(bool));
            ParameterExpression paramExceptionForWhereBody = Expression.Variable(typeof(Exception));
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            try
            {
                MethodInfo unlockMethod = typeof(EventObject).GetMethods()
                                                .Where(m => { return m.Name == "Unlock"; })
                                                .Single();

                Expression selectBlock =
                    Expression.Block(
                        new[] { conditionResult },
                        Expression.Assign(conditionResult, filter),
                        Expression.TryCatch(
                                    Expression.IfThen(                                            
                                        Expression.Equal(conditionResult, Expression.Constant(false)),
                                        Expression.Block(
                                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start call event unlock method."))),
                                            Expression.Call(this.scopeParam.Peek(), unlockMethod, Expression.Constant(this.context.QueryName)),
                                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End call event unlock method.")))
                                        )
                                    ),
                                    Expression.Catch(
                                        paramExceptionForWhereBody,
                                         Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE58, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                    )
                                ),
                        conditionResult
                        );

                var delegateType = typeof(Func<,>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0], filter.Type);
                Expression selectorLambda = Expression.Lambda(delegateType, filter, new ParameterExpression[] { this.scopeParam.Pop() });

                MethodInfo methodWhere = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "Where" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,System.Boolean]");
                })
                .Single().MakeGenericMethod(typeof(I));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the expression observable where"))),
                                    Expression.Assign(result, Expression.Call(methodWhere, incomingObservable, selectorLambda)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the expression observable where"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE19, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE19, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a select for group by expression
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="expSelect">Selector expression.</param>
        /// <returns>Select for group by expression.</returns>
        private Expression CreateSelectForObservableGroupBy(PlanNode actualNode, Expression incomingObservable, Expression expSelect)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(((LambdaExpression)expSelect).ReturnType), "newObservable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodSelectMany = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)expSelect).ReturnType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, Expression.Call(methodSelectMany, incomingObservable, expSelect)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for observable group by")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE2, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                // pop del ámbito
                this.scopeParam.Pop();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a select for group by expression
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="expSelect">Selector expression.</param>
        /// <returns>Select for group by expression.</returns>
        private Expression CreateSelectForEnumerableGroupBy(PlanNode actualNode, Expression incomingObservable, Expression expSelect)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(((LambdaExpression)expSelect).ReturnType), "SelectForEnumerableGroupBy");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodSelect = typeof(System.Linq.Enumerable).GetMethods()
                                                .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)expSelect).ReturnType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for enumerable group by"))),
                                    Expression.Assign(result, Expression.Call(methodSelect, incomingObservable, expSelect)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for enumerable group by")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE20, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                // pop del ámbito
                this.scopeParam.Pop();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE21, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a select for buffer expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="expProjection">Projection lambda expression.</param>
        /// <returns>Select for buffer expression.</returns>
        private Expression CreateSelectForObservableBufferOrSource(PlanNode actualNode, Expression incomingObservable, Expression expProjection)
        {
            try
            {
                if (expProjection.NodeType == ExpressionType.Lambda)
                {
                    // cuando su cuerpo es una proyección
                    ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(((LambdaExpression)expProjection).ReturnType), "SelectForObservableBufferOrSourceLambda");
                    ParameterExpression paramException = Expression.Variable(typeof(Exception));

                    MethodInfo methodSelectMany = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)expProjection).ReturnType);

                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { result },
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable buffer con lambda"))),
                                        Expression.Assign(result, Expression.Call(methodSelectMany, incomingObservable, expProjection)),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for observable buffer con lambda")))
                                        ),
                                    Expression.Catch(
                                        paramException,
                                         Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE21, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                    )
                                ),
                            result
                            );

                    // pop del ámbito
                    this.scopeParam.Pop();

                    return tryCatchExpr;
                }
                else
                {
                    // cuando su cuerpo no es una proyección, es decir, tiene extensiones 
                    ParameterExpression resultSelectBlock = Expression.Variable(expProjection.Type, "LambdaProjectionSelectForObservableBufferOrSource");
                    ParameterExpression paramSelectBlockException = Expression.Variable(typeof(Exception));

                    Expression selectBlock = null;
                    if (this.projectionWithDispose)
                    {
                        selectBlock = expProjection;
                    }
                    else
                    {
                        selectBlock = Expression.Block(
                                    new[] { resultSelectBlock },
                                        Expression.TryCatch(
                                            Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Inicia llamada a Dispose fuera de la proyección")),
                                                Expression.Assign(resultSelectBlock, expProjection),
                                                this.DisposeEvents(actualNode),
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Termina llamada a Dispose fuera de la proyección"))
                                                ),
                                            Expression.Catch(
                                                paramSelectBlockException,
                                                 Expression.Block(
                                                    Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                                    Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE22, actualNode.NodeText), typeof(string)), paramSelectBlockException))
                                                )
                                            )
                                        ),
                                    resultSelectBlock
                                    );
                    }

                    Type delegateType = typeof(Func<,>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0], selectBlock.Type);
                    LambdaExpression selectorLambda = Expression.Lambda(delegateType, selectBlock, new ParameterExpression[] { this.scopeParam.Peek() });

                    ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(selectBlock.Type), "SelectForObservableBufferOrSourceNOLambda");
                    ParameterExpression paramException = Expression.Variable(typeof(Exception));

                    MethodInfo methodSelect = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], selectBlock.Type);

                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { result },
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable buffer sin lambda"))),
                                        Expression.Assign(result, Expression.Call(methodSelect, incomingObservable, selectorLambda)),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for observable buffer sin lambda")))
                                        ),
                                    Expression.Catch(
                                        paramException,
                                         Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE22, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                    )
                                ),
                            result
                            );

                    // pop del ámbito
                    this.scopeParam.Pop();

                    return tryCatchExpr;
                }
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE22, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a select for buffer expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="expProjection">Projection lambda expression.</param>
        /// <returns>Select for buffer expression.</returns>
        private Expression CreateSelectForEnumerable(PlanNode actualNode, Expression incomingObservable, Expression expProjection)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(((LambdaExpression)expProjection).ReturnType), "SelectForEnumerable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodSelectMany = typeof(System.Linq.Enumerable).GetMethods()
                                                .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)expProjection).ReturnType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start the select for enumerable"))),
                                    Expression.Assign(result, Expression.Call(methodSelectMany, incomingObservable, expProjection)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End the select for enumerable")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE23, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                // pop del ámbito
                this.scopeParam.Pop();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE23, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a select for lock expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <returns>Select for buffer expression.</returns>
        private Expression CreateWhereForEventLock(PlanNode actualNode, Expression incomingObservable)
        {            
            ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(typeof(EventObject)), "WhereForEventLock");
            ParameterExpression lambdaResult = Expression.Variable(typeof(bool), "LambdaWhereEventLock");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression paramExceptionForSelect = Expression.Variable(typeof(Exception));

            try
            {
                MethodInfo lockMethod = typeof(EventObject).GetMethods()
                                                .Where(m => { return m.Name == "Lock"; })
                                                .Single();

                Expression selectBlock =
                    Expression.Block(
                        new[] { lambdaResult },
                        Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start call event lock method."))),
                                        Expression.Assign(lambdaResult, Expression.Call(this.scopeParam.Peek(), lockMethod, Expression.Constant(this.context.QueryName))),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End call event lock method.")))
                                        ),
                                    Expression.Catch(
                                        paramException,
                                         Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "Message"))
                                        )
                                    )
                                ),
                        lambdaResult
                        );

                Type delegateType = typeof(Func<,>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0], selectBlock.Type);
                LambdaExpression lambdaSelect = Expression.Lambda(delegateType, selectBlock, new ParameterExpression[] { this.scopeParam.Peek() });
                
                MethodInfo methodWhere = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "Where" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,System.Boolean]");
                })
                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start the select for lock event"))),
                                    Expression.Assign(result, Expression.Call(methodWhere, incomingObservable, lambdaSelect)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End the select for lock event")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE57, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                // pop del ambito
                this.scopeParam.Pop();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE58, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Generate the select block expression.
        /// </summary>
        /// <param name="plans">Plan that contains the projection plans.</param>
        /// <returns>Select block expression.</returns>
        private Expression CreateProjectionExpression(PlanNode plans)
        {
            List<Expression> expressionList = new List<Expression>();

            // se agrega el print log que indica el inicio de la proyección
            expressionList.Add(Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the projection"))));

            Dictionary<string, Tuple<ConstantExpression, Expression>> keyValueList = new Dictionary<string, Tuple<ConstantExpression, Expression>>();
            List<dynamic> listOfFields = new List<dynamic>();

            // obtengo la llave y el valor de cada columna, en forma de expresiones
            foreach (var plan in plans.Children)
            {
                ConstantExpression key = (ConstantExpression)this.GenerateExpressionTree(plan.Children[0]);
                Expression value = this.GenerateExpressionTree(plan.Children[1]);
                if (keyValueList.ContainsKey(key.Value.ToString()))
                {
                    throw new CompilationException(Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE24, string.Empty));
                }
                else
                {
                    keyValueList.Add(key.Value.ToString(), new Tuple<ConstantExpression, Expression>(key, value));
                }
            }

            // por cada tupla llave/valor creo un objeto con las propiedades: nombre (nombre de la propiedad), tipo (tipo de dato de la propiedad)
            dynamic newField = new System.Dynamic.ExpandoObject();
            foreach (KeyValuePair<string, Tuple<ConstantExpression, Expression>> c in keyValueList)
            {
                newField = new System.Dynamic.ExpandoObject();
                newField.FieldName = c.Value.Item1.Value;
                newField.FieldType = c.Value.Item2.Type;
                listOfFields.Add(newField);
            }

            // creo el tipo a retornar para la proyección, si es una proyección para el select el tipo debe heredar de EventResult
            Type myType = default(Type);
            if (((PlanNodeTypeEnum)plans.Properties["ProjectionType"]).Equals(PlanNodeTypeEnum.ObservableSelect))
            {
                myType = LanguageTypeBuilder.CompileResultType(listOfFields, typeof(EventResult));
            }
            else
            {
                myType = LanguageTypeBuilder.CompileResultType(listOfFields);
            }

            // inicializo el tipo creado, si es para un select hereda de EventResult e inicializo los parametros de su constructor
            ParameterExpression y = Expression.Variable(myType);
            if (((PlanNodeTypeEnum)plans.Properties["ProjectionType"]).Equals(PlanNodeTypeEnum.ObservableSelect))
            {
                expressionList.Add(
                    Expression.Assign(
                        y,
                        Expression.New(
                            myType.GetConstructor(typeof(EventResult).GetProperties().Select(x => x.PropertyType).ToArray()),
                            Expression.Property(null, typeof(DateTime).GetProperty("Now"))
                        )
                    )
                );
            }
            else
            {
                expressionList.Add(Expression.Assign(y, Expression.New(myType)));
            }

            // establezco el valor cada propiedad del tipo creado al valor en expresiones de la columna con el nombre(alias) de propiedad(columna)
            foreach (PropertyInfo p in myType.GetProperties())
            {
                if (keyValueList.ContainsKey(p.Name))
                {
                    expressionList.Add(Expression.Call(y, p.GetSetMethod(), keyValueList[p.Name].Item2));
                }
            }

            // se libera la memoria ocupada por el mensaje del evento siempre que el nodo actual sea un select
            // esto es lo último que debe hacerse en el procesamiento del evento
            if (plans.Properties.ContainsKey("DisposeEvents"))
            {
                if (((bool)plans.Properties["DisposeEvents"]).Equals(true))
                {
                    expressionList.Add(this.DisposeEvents(plans));                    
                    this.projectionWithDispose = true;
                }
            }

            // se agrega el print log que indica la finalización de la proyección
            expressionList.Add(Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the projection"))));

            // se agrega el objeto de la proyección resultante
            expressionList.Add(y);

            // se crea el bloque que establece los valores a retornar en el proyección
            Expression expProjectionObject = Expression.Block(
                                                        new[] { y },
                                                        expressionList
                                                );

            // creo la funcion Func<EventObject, "tipo creado en tiempo de ejecución"> del cual se creará el lambda
            var delegateType = typeof(Func<,>).MakeGenericType(this.scopeParam.Peek().Type, myType);

            // creo el lambda que será retornado como hijo derecho del nodo padre, por ejemplo un nodo select
            Expression lambda2 = Expression.Lambda(delegateType, expProjectionObject, new ParameterExpression[] { this.scopeParam.Peek() });

            // retorno el lambda creado
            return lambda2;
        }

        /// <summary>
        /// Generate projection of group by section
        /// </summary>
        /// <param name="plans">Plan that contains the projection plans.</param>
        /// <returns>Select block expression.</returns>
        private Expression CreateProjectionOfConstantsExpression(PlanNode plans)
        {
            List<Expression> expressionList = new List<Expression>();

            // se agrega el print log que indica el inicio de la proyección
            expressionList.Add(Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the constants projection"))));

            Dictionary<string, Tuple<ConstantExpression, Expression>> keyValueList = new Dictionary<string, Tuple<ConstantExpression, Expression>>();
            List<dynamic> listOfFields = new List<dynamic>();

            // obtengo la llave y el valor de cada columna, en forma de expresiones
            foreach (var plan in plans.Children)
            {
                ConstantExpression key = (ConstantExpression)this.GenerateExpressionTree(plan.Children[0]);
                Expression value = this.GenerateExpressionTree(plan.Children[1]);
                if (keyValueList.ContainsKey(key.Value.ToString()))
                {
                    throw new CompilationException(Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE25, string.Empty));
                }
                else
                {
                    keyValueList.Add(key.Value.ToString(), new Tuple<ConstantExpression, Expression>(key, value));
                }
            }

            dynamic newField = new System.Dynamic.ExpandoObject();

            // por cada tupla llave/valor creo un objeto con las propiedades: nombre (nombre de la propiedad), tipo (tipo de dato de la propiedad)
            foreach (KeyValuePair<string, Tuple<ConstantExpression, Expression>> c in keyValueList)
            {
                newField = new System.Dynamic.ExpandoObject();
                newField.FieldName = c.Value.Item1.Value;
                newField.FieldType = c.Value.Item2.Type;
                listOfFields.Add(newField);
            }

            Type myType = LanguageTypeBuilder.CompileResultType(listOfFields);
            ParameterExpression y = Expression.Variable(myType);
            expressionList.Add(Expression.Assign(y, Expression.New(myType)));

            // establezco el valor cada propiedad del tipo creado al valor en expresiones de la columna con el nombre(alias) de propiedad(columna)
            foreach (PropertyInfo p in myType.GetProperties())
            {
                if (keyValueList.ContainsKey(p.Name))
                {
                    expressionList.Add(Expression.Call(y, p.GetSetMethod(), keyValueList[p.Name].Item2));
                }
            }

            // se agrega el print log que indica la finalización de la proyección
            expressionList.Add(Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the constants projection"))));

            // se agrega el objeto de la proyección resultante
            expressionList.Add(y);

            // se crea el bloque que establece los valores a retornar en el proyección
            Expression expProjectionObject = Expression.Block(
                                                        new[] { y },
                                                        expressionList
                                                );

            return expProjectionObject;
        }

        /// <summary>
        /// Call enumerable ToList extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression ToList.</returns>
        private Expression EnumerableToList(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                Type paramGenericType = null;
                if (incomingObservable.Type.Name.Equals("IGroupedObservable`2") || incomingObservable.Type.Name.Equals("IGrouping`2"))
                {
                    paramGenericType = incomingObservable.Type.GetGenericArguments()[1];
                }
                else
                {
                    paramGenericType = incomingObservable.Type.GetGenericArguments()[0];
                }

                ParameterExpression result = Expression.Variable(typeof(List<>).MakeGenericType(new Type[] { paramGenericType }), "ToListParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                ParameterExpression objetoAConvertir = Expression.Variable(incomingObservable.Type, "ObjetoAConvertirALista");

                MethodInfo methodToList = typeof(System.Linq.Enumerable).GetMethods().Where(m =>
                {
                    return m.Name == "ToList" && m.GetParameters().Length == 1;
                })
                .Single().MakeGenericMethod(paramGenericType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { objetoAConvertir, result },
                        Expression.Assign(objetoAConvertir, incomingObservable),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the enumerable to list"))),
                                    Expression.IfThen(
                                        Expression.NotEqual(objetoAConvertir, Expression.Constant(null)),
                                        Expression.Assign(result, Expression.Call(methodToList, objetoAConvertir))
                                    ),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable to list")))
                                    ),
                                Expression.Catch(
                                     paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE54, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE55, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Call dispose method of the Message event of type Integra.Messaging.Message.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <returns>Method call expression.</returns>
        private Expression DisposeEvents(PlanNode actualNode)
        {
            MethodInfo unlockMethod = typeof(EventObject).GetMethods()
                                                .Where(m => { return m.Name == "Unlock"; })
                                                .Single();

            if (this.scopeParam.Peek().Type.IsGenericType)
            {
                try
                {
                    // se hace el ToList del observable entrante para poder hacer ForEach
                    Expression toList = this.EnumerableToList(actualNode, this.scopeParam.Peek());

                    // new scope para el foreach
                    this.CreateNewScope(toList, "ForEachParam");

                    // obtenemos el metodo foreach
                    MethodInfo methodForEach = typeof(System.Collections.Generic.List<>).MakeGenericType(toList.Type.GetGenericArguments()[0]).GetMethods().Where(m =>
                    {
                        return m.Name == "ForEach" && m.GetParameters().Length == 1;
                    })
                    .Single();

                    // este pop es del CreateNewScope al inicio de este bloque
                    ParameterExpression evento = this.scopeParam.Pop();
                    ParameterExpression paramException = Expression.Variable(typeof(Exception));
                    
                    Expression dispose =
                        Expression.Block(
                                    Expression.IfThen(
                                        Expression.NotEqual(evento, Expression.Constant(null, evento.Type)),
                                        Expression.Block(
                                            Expression.IfThen(
                                                Expression.NotEqual(Expression.Call(evento, evento.Type.GetMethod("get_Message")), Expression.Constant(null, typeof(Integra.Messaging.Message))),
                                                Expression.Block(
                                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start call event unlock method."))),
                                                    Expression.Call(evento, unlockMethod, Expression.Constant(this.context.QueryName)),
                                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End call event unlock method.")))
                                                    /*Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Hará dispose del mensaje.")),
                                                    Expression.Call(Expression.Property(evento, "Message"), evento.Type.GetProperty("Message").PropertyType.GetMethod("Dispose")),
                                                    Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Hizo dispose del mensaje."))*/
                                                    )
                                                ))
                                        )
                                    );

                    Type delegateType = typeof(Action<>).MakeGenericType(toList.Type.GetGenericArguments()[0]);
                    LambdaExpression lambda = Expression.Lambda(delegateType, dispose, new ParameterExpression[] { evento });

                    return Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the enumerable to list"))),
                                        Expression.Call(toList, methodForEach, lambda),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable to list")))
                                        ),
                                    Expression.Catch(
                                         paramException,
                                         Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE53, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                    )
                                );
                }
                catch (Exception e)
                {
                    throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE56, actualNode.NodeText), e);
                }
            }
            else
            {
                try
                {
                    ParameterExpression evento = this.scopeParam.Peek();
                    BlockExpression disposeAction =
                        Expression.Block(
                            Expression.IfThen(
                                Expression.NotEqual(evento, Expression.Constant(null, evento.Type)),
                                Expression.IfThen(
                                Expression.NotEqual(Expression.Call(evento, evento.Type.GetMethod("get_Message")), Expression.Constant(null, typeof(Integra.Messaging.Message))),
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start call event unlock method."))),
                                    Expression.Call(evento, unlockMethod, Expression.Constant(this.context.QueryName)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End call event unlock method.")))
                                /*Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the dispose event"))),
                                Expression.Call(Expression.Property(evento, "Message"), evento.Type.GetProperty("Message").PropertyType.GetMethod("Dispose")),
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the dispose event")))*/
                                )
                            )
                        )
                    );

                    ParameterExpression paramException = Expression.Variable(typeof(Exception));
                    Expression tryCatchExpr =
                                    Expression.TryCatch(
                                        disposeAction,
                                        Expression.Catch(
                                             paramException,
                                             Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE55, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                        )
                                    );

                    return tryCatchExpr;
                }
                catch (Exception e)
                {
                    throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE57, actualNode.NodeText), e);
                }
            }
        }

        /// <summary>
        /// Creates a Observable.GroupBy expression
        /// </summary>
        /// <typeparam name="I">Input type.</typeparam>
        /// <typeparam name="O">Output type, IObservable of IGroupedObservable of (O, I).</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="keySelector">Key selector lambda expression</param>
        /// <returns>Group by expression</returns>
        private Expression CreateObservableGroupBy<I, O>(PlanNode actualNode, Expression incomingObservable, Expression keySelector)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<IGroupedObservable<O, I>>), "ResultObservableGroupBy");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodGroupBy = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "GroupBy" && m.GetParameters().Length == 3 && m.GetParameters()[2].ParameterType.ToString().Equals("System.Collections.Generic.IEqualityComparer`1[TKey]");
                })
                .Single().MakeGenericMethod(typeof(I), typeof(O));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the observable group by"))),
                                    Expression.Assign(result, Expression.Call(methodGroupBy, incomingObservable, keySelector, Expression.New(typeof(GroupByKeyComparer<O>)))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the observable group by")))
                                    ),
                                Expression.Catch(
                                     paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE24, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                // pop del ámbito
                this.scopeParam.Pop();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE26, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a Enumerable.GroupBy expression
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="keySelector">Key selector lambda expression</param>
        /// <returns>Group by expression</returns>
        private Expression CreateEnumerableGroupBy(PlanNode actualNode, Expression incomingObservable, Expression keySelector)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(typeof(IGrouping<,>).MakeGenericType(((LambdaExpression)keySelector).ReturnType, incomingObservable.Type.GetGenericArguments()[0])), "ResultEnumerableGroupBy");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodGroupBy = typeof(System.Linq.Enumerable).GetMethods().Where(m =>
                {
                    return m.Name == "GroupBy" && m.GetParameters().Length == 3 && m.GetParameters()[2].ParameterType.ToString().Equals("System.Collections.Generic.IEqualityComparer`1[TKey]");
                })
                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)keySelector).ReturnType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the enumerable group by"))),
                                    Expression.Assign(result, Expression.Call(methodGroupBy, incomingObservable, keySelector, Expression.New(typeof(GroupByKeyComparer<>).MakeGenericType(((LambdaExpression)keySelector).ReturnType)))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable group by")))
                                    ),
                                Expression.Catch(
                                     paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE25, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                // pop del ámbito
                this.scopeParam.Pop();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE27, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a Observable.GroupBy expression
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="keySelector">Key selector lambda expression</param>
        /// <returns>Group by expression</returns>
        private Expression CreateEnumerableOrderBy(PlanNode actualNode, Expression incomingObservable, Expression keySelector)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IOrderedEnumerable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "resultOrderByObservable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                string functionName = string.Empty;

                if (actualNode.NodeType.Equals(PlanNodeTypeEnum.EnumerableOrderBy))
                {
                    functionName = "OrderBy";
                }
                else
                {
                    functionName = "OrderByDescending";
                }

                MethodInfo methodGroupBy = typeof(System.Linq.Enumerable).GetMethods().Where(m =>
                {
                    return m.Name == functionName && m.GetParameters().Length == 3 && m.GetParameters()[2].ParameterType.ToString().Equals("System.Collections.Generic.IComparer`1[TKey]");
                })
                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)keySelector).ReturnType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the enumerable order by"))),
                                    Expression.Assign(result, Expression.Call(methodGroupBy, incomingObservable, keySelector, Expression.New(typeof(OrderByKeyComparer<>).MakeGenericType(((LambdaExpression)keySelector).ReturnType)))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable order by"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                     paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE26, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                // pop del ámbito
                this.scopeParam.Pop();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE54, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create and Expression representing a Observable.Merge
        /// </summary>
        /// <typeparam name="I">Input type</typeparam>
        /// <typeparam name="O">Output type</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable</param>
        /// <returns>Result expression</returns>
        private Expression CreateObservableMerge<I, O>(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(O), "ResultObservableMerge");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodGroupBy = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "Merge" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.ToString().Equals("System.IObservable`1[System.IObservable`1[TSource]]");
                })
                .Single().MakeGenericMethod(typeof(I).GetGenericArguments()[0].GetGenericArguments()[0]);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the observable merge"))),
                                    Expression.Assign(result, Expression.Call(methodGroupBy, incomingObservable)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the observable merge"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE27, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE28, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="plan">plan node to convert</param>
        /// <returns>expression tree of actual plan</returns>
        private ConstantExpression GenerateConstant(PlanNode plan)
        {
            if (!plan.Properties.ContainsKey("Value"))
            {
                return Expression.Constant(null);
            }

            if (!plan.Properties.ContainsKey("DataType"))
            {
                return Expression.Constant(null);
            }

            try
            {
                if (plan.Properties["Value"] == null)
                {
                    return Expression.Constant(null);
                }

                Type tipo = Type.GetType(plan.Properties["DataType"].ToString());
                return Expression.Constant(plan.Properties["Value"], tipo);
            }
            catch (Exception e)
            {
                return Expression.Constant(null);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="plan">plan node to convert</param>
        /// <returns>expression tree of actual plan</returns>
        private ConstantExpression GenerateIdentifier(PlanNode plan)
        {
            if (!plan.Properties.ContainsKey("Value"))
            {
                return Expression.Constant(null);
            }

            if (!plan.Properties.ContainsKey("DataType"))
            {
                return Expression.Constant(null);
            }

            try
            {
                if (plan.Properties["Value"] == null)
                {
                    return Expression.Constant(null);
                }

                Type tipo = Type.GetType(plan.Properties["DataType"].ToString());
                return Expression.Constant(plan.Properties["Value"], tipo);
            }
            catch (Exception e)
            {
                return Expression.Constant(null);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">plan node to convert</param>
        /// <returns>expression tree of actual plan</returns>
        private ParameterExpression GenerateEvent(PlanNode actualNode)
        {
            return this.scopeParam.Peek();
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="objeto">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateValueOfObject(PlanNode actualNode, Expression objeto)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(object), "ValorDelCampo");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                ParameterExpression f = Expression.Variable(objeto.Type, "CampoResultanteParaValor");

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { f, param },
                        Expression.Assign(f, objeto),
                        Expression.IfThenElse(
                            Expression.Equal(f, Expression.Constant(null)),
                            Expression.Assign(param, Expression.Constant(null)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("You will get the value of " + actualNode.NodeText))),
                                    Expression.Assign(param, Expression.Call(f, typeof(MessageField).GetMethod("get_Value"))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("The value of " + actualNode.NodeText + " is: "))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), param)),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                       Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener el valor del campo del mensaje, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, Resources.RUNTIME_ERRORS.RE28(actualNode.NodeText), actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE29(actualNode.NodeText), actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="eventNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateObjectMessage(PlanNode actualNode, ParameterExpression eventNode)
        {
            ParameterExpression param = Expression.Variable(typeof(Message), "MensajeDelEvento");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            try
            {
                Expression mensaje =
                        Expression.Block(
                            new ParameterExpression[] { param },
                            Expression.IfThenElse(
                                Expression.Equal(Expression.Constant(eventNode.Type.GetProperty("Message")), Expression.Constant(null)),
                                Expression.Assign(param, Expression.Default(typeof(Message))),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("You will get the message"))),
                                        Expression.Assign(param, Expression.Property(eventNode, "Message")),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("The message was obtained"))),
                                        Expression.Empty()
                                    ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la propiedad 'Message' del evento, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE29, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                    )
                                )
                            ),
                            param
                            );

                return mensaje;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE30, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="field">left child expression</param>
        /// <param name="subFieldId">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateObjectFieldFromField(PlanNode actualNode, Expression field, Expression subFieldId)
        {
            Expression methodCall = Expression.Constant(null);
            ParameterExpression v = Expression.Variable(typeof(MessageField), "CampoDelCampo");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ConstantExpression auxSubField = (ConstantExpression)subFieldId;
            Type tipo = auxSubField.Type;

            ParameterExpression f = Expression.Variable(field.Type, "CampoResultante");

            try
            {
                methodCall = Expression.Block(
                    new ParameterExpression[] { f, v },
                    Expression.Assign(f, field),
                    Expression.IfThen(
                        Expression.Call(
                            f,
                            typeof(MessageField).GetMethod("Contains", new Type[] { tipo }),
                            auxSubField
                        ),
                        Expression.TryCatch(
                            Expression.Block(
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("You will get the subfield: " + auxSubField.Value))),
                                Expression.Assign(v, Expression.Call(f, typeof(MessageField).GetMethod("get_Item", new Type[] { tipo }), auxSubField)),
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("The subfield was obtained: " + auxSubField.Value)))
                            ),
                            Expression.Catch(
                                paramException,
                                Expression.Block(
                                    Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la subsección de la subsección del mensaje, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                    Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, Resources.RUNTIME_ERRORS.RE3(actualNode.NodeText), actualNode.NodeText), typeof(string)), paramException))
                                )
                            )
                        )
                        ),
                    v);
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE31(actualNode.NodeText), actualNode.NodeText), e);
            }

            return methodCall;
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="part">left child expression</param>
        /// <param name="fieldId">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateObjectFieldFromPart(PlanNode actualNode, Expression part, Expression fieldId)
        {
            ParameterExpression v = Expression.Variable(typeof(MessageField), "CampoDeParte");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ConstantExpression auxField = (ConstantExpression)fieldId;
            Type tipo = auxField.Type;

            ParameterExpression p = Expression.Variable(part.Type, "ParteResultante");

            try
            {
                Expression methodCall = Expression.Block(
                    new ParameterExpression[] { p, v },
                    Expression.Assign(p, part),
                    Expression.IfThen(
                        Expression.Call(
                            p,
                            typeof(MessagePart).GetMethod("Contains", new Type[] { tipo }),
                            auxField
                        ),
                        Expression.TryCatch(
                            Expression.Block(
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("You will get the field: " + auxField.Value))),
                                Expression.Assign(v, Expression.Call(p, typeof(MessagePart).GetMethod("get_Item", new Type[] { tipo }), auxField)),
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("The field was obtained: " + auxField.Value)))
                            ),
                            Expression.Catch(
                                paramException,
                                Expression.Block(
                                    Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la subsección de la sección del mensaje, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                    Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, Resources.RUNTIME_ERRORS.RE30(actualNode.NodeText), actualNode.NodeText), typeof(string)), paramException))
                                )
                            )
                        )
                        ),
                     v);

                return methodCall;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE32(actualNode.NodeText), actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="mensaje">left child expression</param>
        /// <param name="partId">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateObjectPart(PlanNode actualNode, Expression mensaje, Expression partId)
        {
            ParameterExpression v = Expression.Parameter(typeof(MessagePart), "ParteDelMensaje");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ConstantExpression auxPart = (ConstantExpression)partId;
            Type tipo = auxPart.Type;

            ParameterExpression m = Expression.Variable(mensaje.Type, "MensajeResultante");

            try
            {
                Expression parte = Expression.Block(
                    new ParameterExpression[] { m, v },
                    Expression.Assign(m, mensaje),
                    Expression.IfThen(
                        Expression.Call(
                            m,
                            typeof(Message).GetMethod("Contains", new Type[] { tipo }),
                            auxPart
                        ),
                        Expression.TryCatch(
                            Expression.Block(
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("You will get the part: " + auxPart.Value))),
                                Expression.Assign(v, Expression.Call(m, typeof(Message).GetMethod("get_Item", new Type[] { tipo }), auxPart)),
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("The part was obtained: " + auxPart.Value)))
                            ),
                            Expression.Catch(
                                paramException,
                                    Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la sección del objeto, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, Resources.RUNTIME_ERRORS.RE31(actualNode.NodeText), actualNode.NodeText), typeof(string)), paramException))
                                        )
                            )
                        )
                    ),
                    v);

                return parte;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE33(actualNode.NodeText), actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Get the object prefix value
        /// </summary>
        /// <param name="actualNode">Current node</param>
        /// <returns>Object prefix expression</returns>
        private Expression GenerateObjectPrefix(PlanNode actualNode)
        {
            if (!actualNode.Properties.ContainsKey("Value"))
            {
                return Expression.Constant(null);
            }

            if (!actualNode.Properties.ContainsKey("DataType"))
            {
                return Expression.Constant(null);
            }

            this.objectPrefix = actualNode.Properties["Value"].ToString();
            return Expression.Constant(actualNode.Properties["Value"], Type.GetType(actualNode.Properties["DataType"].ToString()));
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateCast(PlanNode actualNode, Expression leftNode)
        {
            Expression resultExp = Expression.Constant(null);
            Type typeToCast = Type.GetType(actualNode.Properties["DataType"].ToString());

            if (leftNode is ConstantExpression)
            {
                if ((leftNode as ConstantExpression).Value == null)
                {
                    return this.StandardizeType(leftNode, this.ConvertToNullable(typeToCast));
                }
            }

            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression valorACastear = Expression.Variable(leftNode.Type, "ValorResultanteParaCastear");

            try
            {
                ParameterExpression retorno = Expression.Parameter(this.ConvertToNullable(typeToCast));
                resultExp =
                Expression.Block(
                    new[] { valorACastear, retorno },
                    Expression.Assign(valorACastear, leftNode),
                    Expression.IfThen(
                        Expression.NotEqual(Expression.Convert(valorACastear, this.ConvertToNullable(leftNode.Type)), Expression.Constant(null)),
                        Expression.TryCatch(
                            Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Unbox a " + typeToCast + " of the following value: "))),
                                    Expression.Assign(retorno, Expression.Unbox(valorACastear, this.ConvertToNullable(typeToCast))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'Unbox' operation to type " + typeToCast))),
                                    Expression.Empty()
                                ),
                            Expression.Catch(
                                    paramException,
                                        Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue castear (unbox) el valor u objeto, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE32, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                )
                        )
                    ),
                    retorno
                );
            }
            catch (Exception e)
            {
                try
                {
                    ParameterExpression retorno = Expression.Parameter(this.ConvertToNullable(typeToCast));
                    resultExp =
                    Expression.Block(
                        new[] { valorACastear, retorno },
                        Expression.Assign(valorACastear, leftNode),
                        Expression.IfThen(
                            Expression.NotEqual(Expression.Convert(valorACastear, this.ConvertToNullable(leftNode.Type)), Expression.Constant(null)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Convert a " + typeToCast + " of the following value: "))),
                                    Expression.Assign(retorno, Expression.Convert(valorACastear, this.ConvertToNullable(typeToCast))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'Convert' operation to type " + typeToCast))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue castear (convert) el valor u objeto, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE33, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        retorno
                    );
                }
                catch (Exception ex)
                {
                    throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE34, actualNode.NodeText), ex);
                }
            }

            return resultExp;
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarLike(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            Expression resultExp = Expression.Constant(null);

            try
            {
                string cadenaLike = rightNode.ToString().Replace("\"", string.Empty);
                string cadenaAComparar = cadenaLike.Replace("%", string.Empty);
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                if (cadenaLike.StartsWith("%") && cadenaLike.EndsWith("%"))
                {
                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation with two wildcards: "))),
                                        Expression.Assign(param, Expression.Call(leftNode, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation with two wildcards"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like '%...%', error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE34, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                    )
                                )
                            ),
                            param
                            );

                    return tryCatchExpr;
                }
                else if (cadenaLike.StartsWith("%"))
                {
                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation with left wildcard: "))),
                                        Expression.Assign(param, Expression.Call(leftNode, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation with left wildcard"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like '%..., error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE35, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                    )
                                )
                            ),
                            param
                            );

                    return tryCatchExpr;
                }
                else if (cadenaLike.EndsWith("%"))
                {
                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation with right wildcard: "))),
                                        Expression.Assign(param, Expression.Call(leftNode, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation with right wildcard"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like '...%, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE36, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                    )
                                )
                            ),
                            param
                            );

                    return tryCatchExpr;
                }
                else
                {
                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation without wildcards: "))),
                                        Expression.Assign(param, Expression.Call(leftNode, typeof(string).GetMethod("Equals", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation without wildcards"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like sin comodines, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE37, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                    )
                                )
                            ),
                            param
                            );

                    return tryCatchExpr;
                }
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE35, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarNot(PlanNode actualNode, Expression leftNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.TypeEqual(leftNode, typeof(bool)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'not' operation: "))),
                                    Expression.Assign(param, Expression.Not(leftNode)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'not' operation"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible negar la expresión de comparación, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE38, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                )
                            ),
                            Expression.Assign(param, Expression.Constant(false))
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE36, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarGreaterThanOrEqual(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                            Expression.Assign(param, Expression.Constant(false)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the greater than or equal operation '>=': "))),
                                    Expression.Assign(param, Expression.GreaterThanOrEqual(leftNode, rightNode)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the greater than or equal operation '>='"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación mayor o igual que '>=', error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE39, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE37, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarGreaterThan(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                            Expression.Assign(param, Expression.Constant(false)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the greater than operation '>': "))),
                                    Expression.Assign(param, Expression.GreaterThan(this.StandardizeType(leftNode, leftNode.Type), this.StandardizeType(rightNode, rightNode.Type))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the greater than operation '>'"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación mayor que '>', error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE4, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE38, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarLessThanOrEqual(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                            Expression.Assign(param, Expression.Constant(false)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the less than or equal operation '<=': "))),
                                    Expression.Assign(param, Expression.LessThanOrEqual(leftNode, rightNode)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the less than or equal operation '<='"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación menor o igual que '<=', error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE40, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE39, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarLessThan(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                         Expression.IfThenElse(
                                Expression.Or(Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the less operation '<': "))),
                                        Expression.Assign(param, Expression.LessThan(this.StandardizeType(leftNode, leftNode.Type), this.StandardizeType(rightNode, rightNode.Type))),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the less operation '<'"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación menor que '<', error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE41, actualNode.NodeText), typeof(string)), paramException))
                                            )
                                    )
                                )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE40, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarNotEqual(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new ParameterExpression[] { param },
                        Expression.TryCatch(
                            Expression.Block(
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the not equal operation '!=': "))),
                                Expression.Assign(param, Expression.NotEqual(this.StandardizeType(leftNode, leftNode.Type), this.StandardizeType(rightNode, rightNode.Type))),
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the not equal operation"))),
                                Expression.Empty()
                                ),
                            Expression.Catch(
                                paramException,
                                Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error con la expresion de desigualdad en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE42, actualNode.NodeText), typeof(string)), paramException))
                                    )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE41, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerarEqual(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new ParameterExpression[] { param },
                        Expression.TryCatch(
                            Expression.Block(
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the equal operation '==': "))),
                                Expression.Assign(param, Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), this.StandardizeType(rightNode, rightNode.Type))),
                                Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the equal operation"))),
                                Expression.Empty()
                                ),
                            Expression.Catch(
                                paramException,
                                Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error con la expresion de igualdad en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE43, actualNode.NodeText), typeof(string)), paramException))
                                    )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE42, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateSubtract(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            Type tipo = Type.GetType(actualNode.Properties["DataType"].ToString());

            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            if (tipo.Equals(typeof(TimeSpan)))
            {
                try
                {
                    ParameterExpression param = Expression.Variable(tipo, "variable");

                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThen(
                                Expression.And(Expression.NotEqual(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.NotEqual(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error with the substract operation '-' for timespan: "))),
                                        Expression.Assign(param, Expression.Call(leftNode, typeof(DateTime).GetMethod("Subtract", new Type[] { typeof(DateTime) }), rightNode)),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the substract operation '-' for timespan"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                         Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error con la expresion aritmetica de resta '-' en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                         Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE44, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                    )
                                )
                            ),
                            param
                            );

                    return tryCatchExpr;
                }
                catch (Exception e)
                {
                    throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE43, actualNode.NodeText), e);
                }
            }
            else
            {
                try
                {
                    ParameterExpression param = Expression.Variable(tipo, "variable");

                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThen(
                                Expression.And(Expression.NotEqual(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))), Expression.NotEqual(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start with the substract operation '-': "))),
                                        Expression.Assign(param, Expression.Subtract(leftNode, rightNode)),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the substract operation '-'"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                        Expression.Constant("Error con la expresion aritmetica de resta '-' en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE45, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                    )
                                )
                            ),
                            param
                            );

                    return tryCatchExpr;
                }
                catch (Exception e)
                {
                    throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE44, actualNode.NodeText), e);
                }
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateNegate(PlanNode actualNode, Expression leftNode)
        {
            Type tipo;

            if (!actualNode.Properties.ContainsKey("DataType"))
            {
                tipo = typeof(decimal);
            }
            else
            {
                tipo = Type.GetType(actualNode.Properties["DataType"].ToString());
            }

            try
            {
                ParameterExpression param = Expression.Variable(tipo, "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThen(
                            Expression.NotEqual(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the negate operation '-': "))),
                                    Expression.Assign(param, Expression.Negate(leftNode)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the negate operation '-': "))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error con la expresion aritmetica unaria de negacion '-' en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE46, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE45, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateAnd(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.And(Expression.TypeEqual(leftNode, typeof(bool)), Expression.TypeEqual(rightNode, typeof(bool))),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the and operation 'and': "))),
                                    Expression.Assign(param, Expression.AndAlso(leftNode, rightNode)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the and operation 'and'"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error con la expresion booleana 'and' en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE47, actualNode.NodeText), typeof(string)), paramException))
                                )
                                )
                            ),
                            Expression.Assign(param, Expression.Constant(false))
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE46, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <param name="rightNode">right child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateOr(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.And(Expression.TypeEqual(leftNode, typeof(bool)), Expression.TypeEqual(rightNode, typeof(bool))),
                            Expression.TryCatch(
                                Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the or operation 'or': "))),
                                        Expression.Assign(param, Expression.OrElse(leftNode, rightNode)),
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the or operation 'or'"))),
                                        Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error con la expresion booleana 'or' en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE48, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                            Expression.Assign(param, Expression.Constant(false))
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE47, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateDateFunction(PlanNode actualNode, Expression leftNode)
        {
            Type tipo = (Type)actualNode.Properties["DataType"];
            string propiedad = actualNode.Properties["Property"].ToString();

            ParameterExpression param = Expression.Variable(tipo, "variable");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            try
            {
                // se evalua las propiedades que terminan con 's' y sin 's' ya que pueden venir dos tipos de valores timespan y datetime, 
                // por ejemplo la funcion hour('<>'), si el valor es de tipo datetime tengo que obtener la propiedad Hour,
                // pero si el valor de tipo timespman tengo que obtener la propiedad Hours.
                PropertyInfo p = leftNode.Type.GetProperties().Where(x => x.Name.Equals(propiedad) || x.Name.Equals(propiedad + "s")).Single();

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.Equal(Expression.Constant(p), Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(tipo)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the get property of a date type operation: " + propiedad))),
                                    Expression.Assign(param, Expression.Call(leftNode, p.GetMethod)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the get property of a date type operation: " + propiedad))),
                                    Expression.Empty()
                                ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error al compilar la funcion '" + propiedad + "', no es posible obtener el valor solicitado. Error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE49, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param);

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE48(propiedad), actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">plan node to convert</param>
        /// <param name="leftNode">Left expression</param>
        /// <param name="rightNode">Right expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GetEventWithPrefixValue(PlanNode actualNode, Expression leftNode, Expression rightNode)
        {
            return rightNode;
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateProperty(PlanNode actualNode, Expression leftNode)
        {
            string propiedad = actualNode.Properties["Property"].ToString();
            MemberExpression expGetProperty = Expression.Property(leftNode, propiedad);
            Type tipoDeLaPropiedad = expGetProperty.Type;

            try
            {
                ParameterExpression param = Expression.Variable(tipoDeLaPropiedad, "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                PropertyInfo prop = leftNode.Type.GetProperty(propiedad);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.Equal(Expression.Constant(prop), Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(tipoDeLaPropiedad)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the get property operation: " + propiedad))),
                                    Expression.Assign(param, expGetProperty),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the get property operation: " + propiedad))),
                                    Expression.Empty()
                                ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la propiedad " + propiedad + ", error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE5, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param);

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE49(propiedad), actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <returns>expression tree to get the IGroupedObservable Key property</returns>
        private Expression GenerateGroupKey(PlanNode actualNode)
        {
            if (this.groupExpression == null)
            {
                throw new CompilationException(Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE50, string.Empty));
            }

            Type tipo = this.groupExpression.Type;
            string propiedad = actualNode.Properties["Value"].ToString();

            try
            {
                ParameterExpression param = Expression.Variable(tipo.GetGenericArguments()[0], "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThenElse(
                            Expression.Equal(Expression.Constant(this.groupExpression.Type.GetProperty(propiedad)), Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(this.groupExpression.Type.GetProperty(propiedad).PropertyType)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the get group key operation: " + propiedad))),
                                    Expression.Assign(param, Expression.Property(this.groupExpression, propiedad)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the get group key operation: " + propiedad))),
                                    Expression.Empty()
                                ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la propiedad " + propiedad + ", error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE50, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param);

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE51(propiedad), actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateGroupKeyProperty(PlanNode actualNode, Expression leftNode)
        {
            Type tipo = leftNode.Type;
            string propiedad = actualNode.Properties["Value"].ToString();

            ParameterExpression g = Expression.Variable(leftNode.Type, "LlaveDeGrupoResultante");

            try
            {
                ParameterExpression param = Expression.Variable(tipo.GetProperty(propiedad).PropertyType, "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { g, param },
                        Expression.Assign(g, leftNode),
                        Expression.IfThenElse(
                            Expression.Equal(Expression.Constant(g.Type.GetProperty(propiedad)), Expression.Constant(null)),
                            Expression.Assign(param, Expression.Default(tipo.GetProperty(propiedad).PropertyType)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the get group key property operation: " + propiedad))),
                                    Expression.Assign(param, Expression.Property(g, propiedad)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the get group key property operation: " + propiedad))),
                                    Expression.Empty()
                                ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la propiedad " + propiedad + ", error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE51, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param);

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE52(propiedad), actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="groupKeyProperty">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateGroupPropertyValue(PlanNode actualNode, Expression groupKeyProperty)
        {
            try
            {
                ParameterExpression param = Expression.Variable(groupKeyProperty.Type, "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                ParameterExpression gkp = Expression.Variable(groupKeyProperty.Type, "PropiedadDeLlaveDeGrupoResultante");

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { gkp, param },
                        Expression.Assign(gkp, groupKeyProperty),
                        Expression.IfThenElse(
                            Expression.Equal(gkp, Expression.Constant(null)),
                            Expression.Assign(param, Expression.Constant(null)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("You will get the property value of " + actualNode.NodeText))),
                                    Expression.Assign(param, gkp),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("The group key property value of " + actualNode.NodeText + " is: "))),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), param))
                                    ),
                                Expression.Catch(
                                    paramException,
                                       Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener el valor del campo del mensaje, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE52, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE53, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Try to convert the no null-able type to a null-able type
        /// </summary>
        /// <param name="tipo">Type to convert</param>
        /// <returns>Converted type</returns>
        private Type ConvertToNullable(Type tipo)
        {
            if (tipo.IsValueType)
            {
                if (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return tipo;
                }
                else
                {
                    return typeof(Nullable<>).MakeGenericType(tipo);
                }
            }
            else
            {
                return tipo;
            }
        }

        /// <summary>
        /// Standardize the type of the expression
        /// </summary>
        /// <param name="exp">Expression to standardize</param>
        /// <param name="type">Type to convert</param>
        /// <returns>Expression standardized</returns>
        private Expression StandardizeType(Expression exp, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return exp;
            }

            if (exp is ConstantExpression)
            {
                ConstantExpression expAux = exp as ConstantExpression;

                if (expAux.Value == null)
                {
                    return Expression.Constant(null, this.ConvertToNullable(type));
                }
                else
                {
                    return Expression.Convert(exp, this.ConvertToNullable(type));
                }
            }
            else
            {
                return Expression.Convert(exp, this.ConvertToNullable(type));
            }
        }
    }
}