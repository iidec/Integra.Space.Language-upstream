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
    using System.Reactive;
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
        #region Field definitions

        /// <summary>
        /// Name of the left source.
        /// </summary>
        private const int LEFTSOURCE = 0;

        /// <summary>
        /// Name of the right source.
        /// </summary>
        private const int RIGHTSOURCE = 1;

        /// <summary>
        /// Dictionary of sources
        /// </summary>
        private Dictionary<int, string> sources;

        /// <summary>
        /// List to store parameter expressions
        /// </summary>
        private List<ParameterExpression> parameterList;

        /// <summary>
        /// Actual scope.
        /// </summary>
        private Scope actualScope;

        /// <summary>
        /// actual scope
        /// </summary>
        private int scopeLevel;

        /// <summary>
        /// Group expression
        /// </summary>
        private Expression groupExpression;

        /// <summary>
        /// ON node.
        /// </summary>
        private PlanNode onNode;

        /// <summary>
        /// Projection with dispose.
        /// </summary>
        private bool projectionWithDispose;

        /// <summary>
        /// Compilation context.
        /// </summary>
        private CompileContext context;

        /// <summary>
        /// Observer of the ObservableCreate node.
        /// </summary>
        private ParameterExpression observer;

        /// <summary>
        /// Indicates whether the compiler is generating the condition of the query section 'ON'
        /// </summary>
        private bool isInConditionOn;

        /// <summary>
        /// Indicates whether the projection for extract event data is for the second source
        /// </summary>
        private bool isSecondSource;

        #endregion Field definitions

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConstructor"/> class.
        /// </summary>
        public ObservableConstructor()
        {
            this.context = new CompileContext();

            this.scopeLevel = 0;
            this.parameterList = new List<ParameterExpression>();
            this.sources = new Dictionary<int, string>();
            this.projectionWithDispose = false;
            this.isInConditionOn = false;
            this.isSecondSource = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableConstructor"/> class
        /// </summary>
        /// <param name="context">Compilation context.</param>
        public ObservableConstructor(CompileContext context)
        {
            this.context = context;

            this.scopeLevel = 0;
            this.parameterList = new List<ParameterExpression>();
            this.sources = new Dictionary<int, string>();
            this.projectionWithDispose = false;
            this.isInConditionOn = false;
            this.isSecondSource = false;
        }

        #endregion Constructors

        #region Compile methods

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

            this.actualScope = null;
            this.parameterList.Clear();
            this.sources.Clear();
            this.projectionWithDispose = false;
            this.scopeLevel = 0;
            this.isInConditionOn = false;
            this.isSecondSource = false;

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

            this.actualScope = null;
            this.parameterList.Clear();
            this.sources.Clear();
            this.projectionWithDispose = false;
            this.scopeLevel = 0;
            this.isInConditionOn = false;
            this.isSecondSource = false;

            Console.WriteLine("La función fue compilada exitosamente.");
            return funcResult;
        }

        /// <summary>
        /// Compile the result function.
        /// </summary>
        /// <typeparam name="In1">Left input type.</typeparam>
        /// <typeparam name="In2">Right input type.</typeparam>
        /// <typeparam name="Out">Output type.</typeparam>
        /// <param name="plan">Execution plan.</param>
        /// <returns>Result function.</returns>
        public Func<In1, In2, Out> Compile<In1, In2, Out>(PlanNode plan)
        {
            Func<In1, In2, Out> funcResult = this.CreateLambda<In1, In2, Out>(plan).Compile();

            this.actualScope = null;
            this.parameterList.Clear();
            this.sources.Clear();
            this.projectionWithDispose = false;
            this.scopeLevel = 0;
            this.isInConditionOn = false;
            this.isSecondSource = false;

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

            this.actualScope = null;
            this.parameterList.Clear();
            this.sources.Clear();
            this.projectionWithDispose = false;
            this.scopeLevel = 0;
            this.isInConditionOn = false;
            this.isSecondSource = false;

            Console.WriteLine("La función fue compilada exitosamente.");
            return funcResult;
        }

        #endregion Compile methods

        #region CreateLambda methods

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
            Expression<Func<In, Out>> result2 = Expression.Lambda<Func<In, Out>>(rootExpression, this.parameterList.ToArray());
            return result2;
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

        #endregion CreateLambda methods

        #region Depth-first search (post-order) and method selector

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
                /*if (plan.Children.ElementAt(0).NodeType.Equals(PlanNodeTypeEnum.LeftJoin) || plan.Children.ElementAt(0).NodeType.Equals(PlanNodeTypeEnum.RightJoin) || plan.Children.ElementAt(0).NodeType.Equals(PlanNodeTypeEnum.CrossJoin) || plan.Children.ElementAt(0).NodeType.Equals(PlanNodeTypeEnum.InnerJoin))
                {
                    leftExp = this.CreateExpressionNode(plan.Children.First<PlanNode>(), null, null);
                }*/
                if (plan.Children.Count() == 2)
                {
                    leftExp = this.GenerateExpressionTree(plan.Children.First());

                    // se hace porque la proyección es un árbol independiente y solo el resultado, un lambda expression, debe ser el nodo derecho
                    if (plan.Children.ElementAt<PlanNode>(1).NodeType.Equals(PlanNodeTypeEnum.Projection) || plan.Children.ElementAt<PlanNode>(1).NodeType.Equals(PlanNodeTypeEnum.ProjectionOfConstants))
                    {
                        rightExp = this.CreateExpressionNode(plan.Children.ElementAt(1), null, null);
                    }
                    else
                    {
                        rightExp = this.GenerateExpressionTree(plan.Children.ElementAt(1));
                    }
                }
                else if (plan.Children.Count() == 1)
                {
                    leftExp = this.GenerateExpressionTree(plan.Children.First());
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
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerateConstant(actualNode);
                    break;
                case PlanNodeTypeEnum.Identifier:
                    expResult = this.GenerateIdentifier(actualNode);
                    break;
                case PlanNodeTypeEnum.StringLeftFunction:
                    expResult = this.CreateStringLeftFunction(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.StringRightFunction:
                    expResult = this.CreateStringRightFunction(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.StringUpperFunction:
                    expResult = this.CreateStringUpperFunction(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.StringLowerFunction:
                    expResult = this.CreateStringLowerFunction(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.Projection:
                    expResult = this.CreateProjectionExpression(actualNode);
                    break;
                case PlanNodeTypeEnum.ProjectionOfConstants:
                    expResult = this.CreateProjectionOfConstantsExpression(actualNode);
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
                    Func<PlanNode, Expression, Expression, Expression> enumerableSumMethod = this.CreateEnumerableAgregationFunctionWithArgument<int>;
                    expResult = enumerableSumMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableTake:
                    expResult = this.CreateEnumerableTake(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.ObservableWhere:
                    Func<PlanNode, Expression, Expression, Expression> observableWhereMethod = this.CreateObservableWhere<int>;
                    expResult = observableWhereMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[0] })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableWhere:
                    expResult = this.CreateEnumerableWhere(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.ObservableTake:
                    expResult = this.CreateObservableTake(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.ObservableGroupBy:
                    Func<PlanNode, Expression, Expression, Expression> groupbyMethod = this.CreateObservableGroupBy<int, int>;
                    expResult = groupbyMethod.Method.GetGenericMethodDefinition()
                                            .MakeGenericMethod(new[] { leftNode.Type.GetGenericArguments()[0], rightNode.Type.GetGenericArguments()[1] })
                                            .Invoke(this, new object[] { actualNode, leftNode, rightNode }) as Expression;
                    break;
                case PlanNodeTypeEnum.EnumerableGroupBy:
                    expResult = this.CreateEnumerableGroupBy(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.EnumerableOrderBy:
                case PlanNodeTypeEnum.EnumerableOrderByDesc:
                    expResult = this.CreateEnumerableOrderBy(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.ObservableWhereForEventLock:
                    expResult = this.CreateWhereForEventLock(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.ObservableSelectForGroupBy:
                    expResult = this.CreateSelectForObservableGroupBy(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.EnumerableSelectForGroupBy:
                    expResult = this.CreateSelectForEnumerableGroupBy(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource:
                    expResult = this.CreateSelectForObservableBufferOrSource(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.EnumerableSelectForEnumerable:
                    expResult = this.CreateSelectForEnumerable(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.EnumerableToList:
                    expResult = this.EnumerableToList(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.EnumerableToArray:
                    expResult = this.EnumerableToArray(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.EnumerableToObservable:
                    expResult = this.EnumerableToObservable(actualNode, leftNode);
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
                    this.CreateNewScope(actualNode, leftNode, rightNode);
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
                case PlanNodeTypeEnum.SelectForResult:
                    expResult = this.CreateSelectForResult(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.SelectForResultProjection:
                    expResult = this.CreateProjectionForResult(actualNode);
                    break;
                case PlanNodeTypeEnum.Subscription:
                    expResult = this.CreateSubscription(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.ObservableCreate:
                    expResult = this.CreateObservableCreate(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.ObservableNever:
                    expResult = this.CreateObservableNever();
                    break;
                case PlanNodeTypeEnum.ObservableEmpty:
                    expResult = this.CreateObservableEmpty();
                    break;
                case PlanNodeTypeEnum.ObservableTimeout:
                    expResult = this.CreateObservableTimeout(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.ObservableCatch:
                    expResult = this.CreateObservableCatch(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.JoinProjection:
                    expResult = this.CreateJoinProjection(actualNode);
                    break;
                case PlanNodeTypeEnum.LeftJoin:
                case PlanNodeTypeEnum.RightJoin:
                case PlanNodeTypeEnum.CrossJoin:
                case PlanNodeTypeEnum.InnerJoin:
                    expResult = this.CreateObservableJoin(actualNode);
                    break;
                case PlanNodeTypeEnum.ObservablePublish:
                    expResult = this.CreatePublish(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.ObservableRefCount:
                    expResult = this.CreateRefCount(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.ObservableSwitch:
                    expResult = this.ObservableSwitch(actualNode, leftNode);
                    break;
                /********************************************************************************************************************************************************************************************************************/
                case PlanNodeTypeEnum.Cast:
                    return this.GenerateCast(actualNode, leftNode);
                case PlanNodeTypeEnum.Equal:
                    return this.GenerarEqual(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.NotEqual:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerarNotEqual(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.LessThan:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerarLessThan(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.LessThanOrEqual:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerarLessThanOrEqual(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.GreaterThan:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerarGreaterThan(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.GreaterThanOrEqual:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerarGreaterThanOrEqual(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.Not:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerarNot(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.Like:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

                    expResult = this.GenerarLike(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.Or:
                    if (this.isInConditionOn)
                    {
                        throw new CompilationException(string.Empty);
                    }

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
                    return this.GenerateEvent(actualNode, (ConstantExpression)leftNode);
                case PlanNodeTypeEnum.DateTimeFunction:
                    return this.GenerateDateFunction(actualNode, leftNode);
                case PlanNodeTypeEnum.Negate:
                    return this.GenerateNegate(actualNode, leftNode);
                case PlanNodeTypeEnum.Subtract:
                    return this.GenerateSubtract(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.Property:
                case PlanNodeTypeEnum.EventProperty:
                    return this.GenerateProperty(actualNode, leftNode);
                default:
                    throw new CompilationException(Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE1, string.Empty));
            }

            return expResult;
        }

        #endregion Depth-first search (post-order) and method selector

        #region Observable Create and Subscription

        /// <summary>
        /// Creates the Observable.Create to return.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression of the Observable.Create.</returns>
        private Expression CreateObservableCreate(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(this.observer.Type.GetGenericArguments()[0]), "FinalResultObservable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                // creo la funcion Func<EventObject, "tipo creado en tiempo de ejecución"> del cual se creará el lambda
                var delegateType = typeof(Func<,>).MakeGenericType(this.observer.Type, typeof(IDisposable));

                // creo el lambda que será retornado como hijo derecho del nodo padre, por ejemplo un nodo select
                LambdaExpression lambdaOfTheSubscribe = Expression.Lambda(delegateType, incomingObservable, this.observer);

                MethodInfo methodCreate = typeof(System.Reactive.Linq.Observable).GetMethods()
                                            .Where(x => x.Name == "Create" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.ToString().Equals("System.Func`2[System.IObserver`1[TResult],System.IDisposable]"))
                                            .Single().MakeGenericMethod(this.observer.Type.GetGenericArguments()[0]);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, Expression.Call(methodCreate, lambdaOfTheSubscribe)),
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

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates the subscription to the observable and send the result through the observer.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>IDisposable expression</returns>
        private Expression CreateSubscription(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IDisposable), "ResultDisposable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                if (this.observer == null)
                {
                    this.observer = Expression.Parameter(typeof(IObserver<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ObserverDePrueba");
                }

                MethodInfo onCompletedMethod = this.observer.Type.GetMethod("OnCompleted");

                // creo el Action del cual se creará el lambda
                LambdaExpression lambdaOfTheOnCompleted = Expression.Lambda(typeof(Action), Expression.Call(this.observer, onCompletedMethod));

                MethodInfo onNextMethod = this.observer.Type.GetMethod("OnNext");

                // creo el Action<"tipo creado en tiempo de ejecución"> del cual se creará el lambda
                Type delegateType = typeof(Action<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]);

                // creo el lambda que será retornado como hijo derecho del nodo padre, por ejemplo un nodo select
                LambdaExpression lambdaOfTheOnNext = Expression.Lambda(delegateType, Expression.Call(this.observer, onNextMethod, this.actualScope.GetFirstParameter()), new ParameterExpression[] { this.actualScope.GetFirstParameter() });

                // sacamos el parámetro de la pila
                this.PopScope();

                MethodInfo subscribeMethod = typeof(ObservableExtensions).GetMethods()
                                                .Where(m => { return m.Name == "Subscribe" && m.GetParameters().Length == 3 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Action`1[T]") && m.GetParameters()[2].ParameterType.ToString().Equals("System.Action"); })
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, Expression.Call(subscribeMethod, incomingObservable, lambdaOfTheOnNext, lambdaOfTheOnCompleted)),
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

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
            }
        }

        #endregion Observable Create and Subscription

        #region Join

        /// <summary>
        /// Creates an Observable.Never[Unit]() expression.
        /// </summary>
        /// <returns>Result expression.</returns>
        private Expression CreateObservableNever()
        {
            Type genericType = typeof(Unit);
            ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(genericType), "ObservableNever");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            MethodInfo methodNever = typeof(Observable).GetMethods()
                                            .Where(m => m.Name == "Never" && m.GetParameters().Length == 0)
                                            .Single().MakeGenericMethod(genericType);

            Expression tryCatchExpr =
                Expression.Block(
                    new[] { result },
                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                        Expression.Assign(result, Expression.Call(null, methodNever)),
                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for observable group by"))),
                    result
                    );

            return tryCatchExpr;
        }

        /// <summary>
        /// Creates an Observable.Empty[Unit]() expression.
        /// </summary>
        /// <returns>Result expression.</returns>
        private Expression CreateObservableEmpty()
        {
            Type genericType = typeof(Unit);
            ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(genericType), "ObservableEmpty");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            MethodInfo methodEmpty = typeof(Observable).GetMethods()
                                            .Where(m => m.Name == "Empty" && m.GetParameters().Length == 0)
                                            .Single().MakeGenericMethod(genericType);

            Expression tryCatchExpr =
                Expression.Block(
                    new[] { result },
                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                        Expression.Assign(result, Expression.Call(null, methodEmpty)),
                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for observable group by"))),
                    result
                    );

            return tryCatchExpr;
        }

        /// <summary>
        /// Create an Observable.Timeout[T] expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <param name="right">TimeSpan value.</param>
        /// <returns>Result expression IObservable[T].</returns>
        private Expression CreateObservableTimeout(PlanNode actualNode, Expression incomingObservable, Expression right)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ObservableTimeout");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression valueResult = null;
                if (actualNode.Properties.ContainsKey("ReturnObservable") && bool.Parse(actualNode.Properties["ReturnObservable"].ToString()))
                {
                    MethodInfo timeoutMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Timeout" && m.GetParameters().Length == 4 && m.GetParameters()[1].ParameterType.Equals(typeof(TimeSpan)) && m.GetParameters()[3].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler)); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                    valueResult = Expression.Call(timeoutMethod, incomingObservable, right, this.CreateObservableEmpty(), Expression.Constant(this.context.Scheduler.GetScheduler()));
                }
                else
                {
                    MethodInfo timeoutMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Timeout" && m.GetParameters().Length == 3 && m.GetParameters()[1].ParameterType.Equals(typeof(TimeSpan)) && m.GetParameters()[2].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler)); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                    valueResult = Expression.Call(timeoutMethod, incomingObservable, right, Expression.Constant(this.context.Scheduler.GetScheduler()));
                }

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, valueResult),
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

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create an Observable.Timeout[T] expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <param name="right">TimeSpan value.</param>
        /// <returns>Result expression IObservable[T].</returns>
        private Expression CreateObservableCatch(PlanNode actualNode, Expression incomingObservable, Expression right)
        {
            try
            {
                // observer de prueba, quitar la siguiente línea cuando ya este todo unído
                if (this.observer == null)
                {
                    this.observer = Expression.Parameter(typeof(IObserver<>).MakeGenericType(right.Type), "ObserverDePrueba");
                }

                MethodInfo onNextMethod = this.observer.Type.GetMethod("OnNext");

                var body =
                    Expression.Block(
                        Expression.Call(this.observer, onNextMethod, right),
                        this.CreateObservableEmpty()
                        );

                MethodInfo[] catchMethods = typeof(System.Reactive.Linq.Observable).GetMethods().Where(x => x.Name == "Catch" && x.GetParameters().Length == 2).ToArray();
                MethodInfo catchMethod = catchMethods
                                                .Where(m => m.GetParameters()[0].ParameterType.ToString().Equals("System.IObservable`1[TSource]") && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TException,System.IObservable`1[TSource]]"))
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], typeof(TimeoutException));

                // creo el Action<"tipo creado en tiempo de ejecución"> del cual se creará el lambda
                var delegateType = typeof(Func<,>).MakeGenericType(typeof(TimeoutException), typeof(IObservable<>).MakeGenericType(typeof(Unit)));

                // creo el lambda que será retornado como hijo derecho del nodo padre, por ejemplo un nodo select
                LambdaExpression lambdaOfTheOnNext = Expression.Lambda(delegateType, body, new ParameterExpression[] { this.actualScope.GetParameterByType(typeof(TimeoutException)) });

                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ObservableCatch");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, Expression.Call(catchMethod, incomingObservable, lambdaOfTheOnNext)),
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

                this.PopScope();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Create an Enumerable.Join expression.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <returns>Result expression.</returns>
        private Expression CreateObservableJoin(PlanNode actualNode)
        {
            try
            {
                // on condition
                this.onNode = actualNode.Children[1];

                // get the sources of the join node
                Expression source1 = this.GenerateExpressionTree(actualNode.Children[0].Children[0]);
                Expression source2 = this.GenerateExpressionTree(actualNode.Children[0].Children[1]);

                // creates the scope for the join with two parameters
                ScopeParameter[] newScopeParameters = new ScopeParameter[] { new ScopeParameter(0, source1.Type.GetGenericArguments()[0]), new ScopeParameter(1, source2.Type.GetGenericArguments()[0]) };
                actualNode.Children[0].Properties["ScopeParameters"] = newScopeParameters;
                this.CreateNewScope(actualNode.Children[0], source1, source2);

                // get the durations of the observable join
                InternalPlanNodes ipn = new InternalPlanNodes();
                Expression leftDuration = null;
                PlanNode timeoutConstant = actualNode.Children[2].Children.First();
                if (actualNode.NodeType.Equals(PlanNodeTypeEnum.LeftJoin) || actualNode.NodeType.Equals(PlanNodeTypeEnum.CrossJoin))
                {
                    leftDuration = this.GenerateExpressionTree(ipn.DurationWithSendingResults(timeoutConstant, 0, PlanNodeTypeEnum.JoinLeftDuration));
                }
                else if (actualNode.NodeType.Equals(PlanNodeTypeEnum.RightJoin) || actualNode.NodeType.Equals(PlanNodeTypeEnum.InnerJoin))
                {
                    leftDuration = this.GenerateExpressionTree(ipn.DurationWithoutSendingResults(timeoutConstant));
                }
                else
                {
                    throw new CompilationException(string.Empty);
                }

                Type delegateTypeLeftDuration = typeof(Func<,>).MakeGenericType(newScopeParameters[0].Type, typeof(IObservable<>).MakeGenericType(typeof(Unit)));
                LambdaExpression lambdaLeftDuration = Expression.Lambda(delegateTypeLeftDuration, leftDuration, new ParameterExpression[] { this.actualScope.GetParameterByIndex(0) });

                Expression rightDuration = null;
                if (actualNode.NodeType.Equals(PlanNodeTypeEnum.LeftJoin) || actualNode.NodeType.Equals(PlanNodeTypeEnum.InnerJoin))
                {
                    rightDuration = this.GenerateExpressionTree(ipn.DurationWithoutSendingResults(timeoutConstant));
                }
                else if (actualNode.NodeType.Equals(PlanNodeTypeEnum.RightJoin) || actualNode.NodeType.Equals(PlanNodeTypeEnum.CrossJoin))
                {
                    rightDuration = this.GenerateExpressionTree(ipn.DurationWithSendingResults(timeoutConstant, 0, PlanNodeTypeEnum.JoinRightDuration));
                }
                else
                {
                    throw new CompilationException(string.Empty);
                }

                Type delegateTypeRightDuration = typeof(Func<,>).MakeGenericType(newScopeParameters[1].Type, typeof(IObservable<>).MakeGenericType(typeof(Unit)));
                LambdaExpression lambdaRightDuration = Expression.Lambda(delegateTypeRightDuration, rightDuration, new ParameterExpression[] { this.actualScope.GetParameterByIndex(1) });

                this.PopScope();

                /************************************** START ENUMERABLE JOIN *****************************************/

                // creates the scope for the join with two parameters
                ScopeParameter[] selectorScopeParameters = new ScopeParameter[] { new ScopeParameter(0, source1.Type.GetGenericArguments()[0]), new ScopeParameter(1, source2.Type.GetGenericArguments()[0]) };
                actualNode.Children[0].Properties["ScopeParameters"] = selectorScopeParameters;
                this.CreateNewScope(actualNode.Children[0], source1, source2);

                // left key
                ParameterExpression leftKeyParam = Expression.Parameter(selectorScopeParameters[0].Type.GetGenericArguments()[0]);
                Expression leftKey = Expression.Block(
                    /*Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Convert(Expression.Call(leftKeyParam, leftKeyParam.Type.GetMethod("GetHashCode")), typeof(object)))),*/
                    leftKeyParam
                    );
                Type delegateTypeLeftKey = typeof(Func<,>).MakeGenericType(leftKeyParam.Type, leftKeyParam.Type);
                LambdaExpression lambdaLeftKey = Expression.Lambda(delegateTypeLeftKey, leftKey, new ParameterExpression[] { leftKeyParam });

                // right key
                ParameterExpression rightKeyParam = Expression.Parameter(selectorScopeParameters[1].Type.GetGenericArguments()[0]);
                Expression rightKey = Expression.Block(
                    /*Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Convert(Expression.Call(rightKeyParam, rightKeyParam.Type.GetMethod("GetHashCode")), typeof(object)))),*/
                    rightKeyParam
                    );
                Type delegateTypeRightKey = typeof(Func<,>).MakeGenericType(rightKeyParam.Type, rightKeyParam.Type);
                LambdaExpression lambdaRightKey = Expression.Lambda(delegateTypeRightKey, rightKey, new ParameterExpression[] { rightKeyParam });

                // enumerable selector
                ParameterExpression enumerableSelectorLeftParam = Expression.Parameter(selectorScopeParameters[0].Type.GetGenericArguments()[0]);
                ParameterExpression enumerableSelectorRightParam = Expression.Parameter(selectorScopeParameters[1].Type.GetGenericArguments()[0]);
                MethodInfo methodCreate = typeof(Tuple).GetMethods()
                                                .Where(m => m.Name == "Create" && m.GetParameters().Length == 2)
                                                .Single().MakeGenericMethod(enumerableSelectorLeftParam.Type, enumerableSelectorRightParam.Type);

                Expression enumerableSelector = Expression.Block(
                    Expression.Call(methodCreate, enumerableSelectorLeftParam, enumerableSelectorRightParam)
                    );

                Type delegateTypeEnumerableSelector = typeof(Func<,,>).MakeGenericType(enumerableSelectorLeftParam.Type, enumerableSelectorRightParam.Type, methodCreate.ReturnType);
                LambdaExpression lambdaEnumerableSelector = Expression.Lambda(delegateTypeEnumerableSelector, enumerableSelector, new ParameterExpression[] { enumerableSelectorLeftParam, enumerableSelectorRightParam });

                // enumerable join
                MethodInfo methodEnumerableJoin = typeof(System.Linq.Enumerable).GetMethods()
                                                .Where(m => m.Name == "Join" && m.GetParameters().Length == 5)
                                                .Single().MakeGenericMethod(enumerableSelectorLeftParam.Type, enumerableSelectorRightParam.Type, typeof(ExtractedEventData), methodCreate.ReturnType);

                ParameterExpression enumerableJoinResult = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(methodCreate.ReturnType), "EnumerableJoinSelectorResult");
                ParameterExpression paramExp = Expression.Variable(typeof(Exception), "EnumerableJoinException");

                Expression tryCatchExprEnumJoin =
                    Expression.Block(
                        new[] { enumerableJoinResult },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for enumerable join"))),
                                    Expression.Assign(enumerableJoinResult, Expression.Call(methodEnumerableJoin, this.actualScope.GetParameterByIndex(0), this.actualScope.GetParameterByIndex(1), lambdaLeftKey, lambdaRightKey, lambdaEnumerableSelector)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for enumerable join")))
                                    ),
                                Expression.Catch(
                                    paramExp,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE2, actualNode.NodeText), typeof(string)), paramExp))
                                    )
                                )
                            ),
                        enumerableJoinResult
                        );

                /************************************** FIN ENUMERABLE JOIN *****************************************/

                ParameterExpression projectionResult = Expression.Variable(tryCatchExprEnumJoin.Type.GetGenericArguments()[0], "EnumerableJoinProjectionResult");
                ParameterExpression projectionResultAux = Expression.Variable(projectionResult.Type, "EnumerableJoinProjectionResultAux");
                Expression projectionBlock = Expression.Block(
                    new[] { projectionResultAux },
                    Expression.Assign(projectionResultAux, projectionResult),
                    /*Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("+++++++++++++++++++++++++++++++++++++++++++++++"))),
                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(projectionResultAux, "Item1"))),
                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(projectionResultAux, "Item2"))),
                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("+++++++++++++++++++++++++++++++++++++++++++++++"))),*/
                    Expression.Assign(Expression.Property(Expression.Property(projectionResultAux, "Item1"), "Matched"), Expression.Constant(true)),
                    Expression.Assign(Expression.Property(Expression.Property(projectionResultAux, "Item2"), "Matched"), Expression.Constant(true)),
                    projectionResultAux
                    );

                LambdaExpression lambdaEnumerableSelect = Expression.Lambda(typeof(Func<,>).MakeGenericType(projectionResult.Type, projectionResult.Type), projectionBlock, new ParameterExpression[] { projectionResult });

                this.CreateNewScope(actualNode, tryCatchExprEnumJoin, null);
                Expression enumerableSelect = this.CreateSelectForEnumerable(actualNode, tryCatchExprEnumJoin, lambdaEnumerableSelect);

                Expression toArrayEnumJoin = this.EnumerableToArray(actualNode, enumerableSelect);
                Expression toObservable = this.EnumerableToObservable(actualNode, toArrayEnumJoin);

                Type delegateTypeObservableSelector = typeof(Func<,,>).MakeGenericType(this.actualScope.GetParameterByIndex(0).Type, this.actualScope.GetParameterByIndex(1).Type, toObservable.Type);
                LambdaExpression lambdaObservableSelector = Expression.Lambda(delegateTypeObservableSelector, toObservable, new ParameterExpression[] { this.actualScope.GetParameterByIndex(0), this.actualScope.GetParameterByIndex(1) });

                this.PopScope();
                /************************************** OBSERVABLE JOIN *****************************************/

                MethodInfo methodObservableJoin = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                .Where(m => m.Name == "Join")
                                                .Single().MakeGenericMethod(source1.Type.GetGenericArguments()[0], source2.Type.GetGenericArguments()[0], lambdaLeftDuration.ReturnType.GetGenericArguments()[0], lambdaRightDuration.ReturnType.GetGenericArguments()[0], lambdaObservableSelector.ReturnType);

                ParameterExpression obvJoinResult = Expression.Variable(typeof(IObservable<>).MakeGenericType(lambdaObservableSelector.ReturnType), "ObservableJoinResult");
                ParameterExpression paramException2 = Expression.Variable(typeof(Exception), "ObservableJoinException");

                Expression tryCatchExprObvJoin =
                    Expression.Block(
                        new[] { obvJoinResult },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(obvJoinResult, Expression.Call(methodObservableJoin, source1, source2, lambdaLeftDuration, lambdaRightDuration, lambdaObservableSelector)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the select for observable group by")))
                                    ),
                                Expression.Catch(
                                    paramException2,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE2, actualNode.NodeText), typeof(string)), paramException2))
                                    )
                                )
                            ),
                        obvJoinResult
                        );

                return tryCatchExprObvJoin;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE63, actualNode.NodeText), e);
            }
        }

        #endregion Join

        #region Publish and RefCount

        /// <summary>
        /// Creates the expression for Observable.Publish.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Expression result of type IConnectableObservable[InOb].</returns>
        private Expression CreatePublish(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                Type incomingType = incomingObservable.Type.GetGenericArguments()[0];
                ParameterExpression result = Expression.Variable(typeof(System.Reactive.Subjects.IConnectableObservable<>).MakeGenericType(incomingType), "PublishResult");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo publishMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                .Where(m => m.Name == "Publish" && m.GetParameters().Length == 1)
                                                .Single().MakeGenericMethod(incomingType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, Expression.Call(publishMethod, incomingObservable)),
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

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE63, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates the expression for Observable.Publish.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Expression result of type IConnectableObservable[InOb].</returns>
        private Expression CreateRefCount(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                Type incomingType = incomingObservable.Type.GetGenericArguments()[0];
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingType), "RefCountResult");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo refCountMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                .Where(m => m.Name == "RefCount" && m.GetParameters().Length == 1)
                                                .Single().MakeGenericMethod(incomingType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, Expression.Call(refCountMethod, incomingObservable)),
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

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE63, actualNode.NodeText), e);
            }
        }

        #endregion Publish and RefCount

        #region Creates the query result

        /// <summary>
        /// Create the select that transforms the IEnumerable{EventResult} in a QueryResult{EventResult} object
        /// </summary>
        /// <param name="actualNode">Actual plan node</param>
        /// <param name="incomingObservable">Incoming observable</param>
        /// <param name="expSelect">Lambda of the select expression.</param>
        /// <returns>Expression result</returns>
        private Expression CreateSelectForResult(PlanNode actualNode, Expression incomingObservable, Expression expSelect)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(((LambdaExpression)expSelect).ReturnType), "ObservableQueryResult");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodSelect = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)expSelect).ReturnType);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the select for observable group by"))),
                                    Expression.Assign(result, Expression.Call(methodSelect, incomingObservable, expSelect)),
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

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
            }
        }

        #endregion Creates the query result

        #region Sources and scopes

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
                if (int.Parse(actualNode.Properties["SourcePosition"].ToString()).Equals(0))
                {
                    // LEFTSOURCE = parameterName;
                    this.sources.Add(LEFTSOURCE, parameterName);
                }
                else if (int.Parse(actualNode.Properties["SourcePosition"].ToString()).Equals(1))
                {
                    // RIGHTSOURCE = parameterName;
                    this.sources.Add(RIGHTSOURCE, parameterName);
                }

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
                if (actualNode.Properties.ContainsKey("ParameterType"))
                {
                    return this.actualScope.GetParameterByType((Type)actualNode.Properties["ParameterType"]);
                }
                else if (actualNode.Properties.ContainsKey("ParameterGenericType"))
                {
                    if (actualNode.Properties.ContainsKey("index"))
                    {
                        return this.actualScope.GetParameterByGenericType(int.Parse(actualNode.Properties["index"].ToString()), (Type)actualNode.Properties["ParameterGenericType"]);
                    }
                    else
                    {
                        return this.actualScope.GetParameterByGenericType(0, (Type)actualNode.Properties["ParameterGenericType"]);
                    }
                }
                else if (actualNode.Properties.ContainsKey("ParameterPosition"))
                {
                    return this.actualScope.GetParameterByIndex(int.Parse(actualNode.Properties["ParameterPosition"].ToString()));
                }
                else
                {
                    return this.actualScope.GetFirstParameter();
                }
            }
            catch (InvalidOperationException e)
            {
                throw new CompilationException(Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE2, string.Empty), e);
            }
        }

        /// <summary>
        /// Add a new scope variable to the scope variables stack.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="leftObservable">Left observable expression.</param>
        /// <param name="rightObservable">Right observable expression.</param>
        private void CreateNewScope(PlanNode actualNode, Expression leftObservable, Expression rightObservable)
        {
            this.actualScope = new Scope(this.scopeLevel++, this.actualScope);
            this.CreateScopeParameters(actualNode, leftObservable);
        }

        /// <summary>
        /// Add a new scope variable to the scope variables stack.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        private void CreateScopeParameters(PlanNode actualNode, Expression incomingObservable)
        {
            if (actualNode.Properties.ContainsKey("ScopeParameters"))
            {
                ScopeParameter[] scopeParameters = (ScopeParameter[])actualNode.Properties["ScopeParameters"];
                foreach (ScopeParameter sp in scopeParameters)
                {
                    string paramName = string.Format("NewScopeParameter_{0}", sp.Position);
                    if (sp.Type == null)
                    {
                        if (incomingObservable.Type.IsGenericType)
                        {
                            this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type.GetGenericArguments()[0], paramName));
                        }
                        else
                        {
                            this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type, paramName));
                        }
                    }
                    else
                    {
                        this.actualScope.AddParameter(Expression.Parameter(sp.Type, paramName));
                    }
                }
            }
            else
            {
                string paramName = "NewScopeParameter";

                // si tiene parametros genéricos, creo una nueva variable global para el nuevo ambiente
                // de lo contrario se utilizará la que se tiene actualmente al tope de la pila
                if (incomingObservable.Type.IsGenericType)
                {
                    Type gt1 = incomingObservable.Type.GetGenericTypeDefinition();
                    if (gt1.Equals(typeof(IGroupedObservable<,>)) || gt1.Equals(typeof(IGrouping<,>)))
                    {
                        this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type.GetGenericArguments()[1], paramName));
                    }
                    else
                    {
                        this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type.GetGenericArguments()[0], paramName));
                    }

                    if (incomingObservable.Type.GetGenericArguments()[0].IsGenericType)
                    {
                        Type gt2 = incomingObservable.Type.GetGenericArguments()[0].GetGenericTypeDefinition();
                        if (gt2.Equals(typeof(IGroupedObservable<,>)) || gt2.Equals(typeof(IGrouping<,>)))
                        {
                            this.groupExpression = this.actualScope.GetParameterByGenericType(0, typeof(IGroupedObservable<,>), typeof(IGrouping<,>));
                        }
                    }
                }
                else
                {
                    this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type, paramName));
                }
            }
        }

        /// <summary>
        /// Removes the actual scope and makes the parent scope the actual scope
        /// </summary>
        private void PopScope()
        {
            try
            {
                int level = this.actualScope.Level;
                Scope innerScope = this.actualScope;
                this.actualScope = this.actualScope.ParentScope;
                if (this.actualScope != null)
                {
                    this.actualScope.RemoveInnerScope(innerScope);
                }
            }
            catch (Exception e)
            {
                throw new CompilationException("Internal error: bad scope management.");
            }
        }

        #endregion Sources and scopes

        #region Functions

        #region String functions

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

            int cutNumber = int.Parse(((PlanNode)actualNode.Properties["Number"]).Properties["Value"].ToString());
            Expression numberExp = Expression.Constant(cutNumber, typeof(int));

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

            int cutNumber = int.Parse(((PlanNode)actualNode.Properties["Number"]).Properties["Value"].ToString());
            Expression numberExp = Expression.Constant(cutNumber, typeof(int));

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

        #endregion String functions

        #region Timespan or date and time functions

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

        #endregion Timespan or date and time functions 

        #endregion Functions

        #region Observable extensions

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
                ConstructionValidator.Validate(actualNode.NodeType, actualNode, incomingObservable, bufferTimeOrSize);
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

                this.PopScope();

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
        /// <typeparam name="I">Input type.</typeparam>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="bufferTimeAndSize">Time or size expression for the buffer.</param>
        /// <returns>Observable buffer expression.</returns>
        private Expression CreateObservableBufferTimeAndSize<I>(PlanNode actualNode, Expression incomingObservable, Expression bufferTimeAndSize)
        {
            try
            {
                ConstructionValidator.Validate(actualNode.NodeType, actualNode, incomingObservable, bufferTimeAndSize);

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
                var delegateType = typeof(Func<,>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0], filter.Type);
                Expression selectorLambda = Expression.Lambda(delegateType, filter, new ParameterExpression[] { this.actualScope.GetFirstParameter() });
                this.PopScope();

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
                this.PopScope();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE26, actualNode.NodeText), e);
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
                this.PopScope();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
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
                    this.PopScope();

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
                    LambdaExpression selectorLambda = Expression.Lambda(delegateType, selectBlock, new ParameterExpression[] { this.actualScope.GetFirstParameter() });

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
                    this.PopScope();

                    return tryCatchExpr;
                }
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE22, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Call Observable.Switch extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression ToObservable.</returns>
        private Expression ObservableSwitch(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(incomingObservable.Type.GetGenericArguments()[0], "SwitchParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                ParameterExpression objetoAConvertir = Expression.Variable(incomingObservable.Type, "ObjetoAConvertirAHacerSwitch");

                // se usa Last() porque ya que aun no ha sido posible diferenciarlos por el tipo de parametro
                MethodInfo methodToObservable = typeof(System.Reactive.Linq.Observable).GetMethods().First(m =>
                {
                    return m.Name == "Switch" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IObservable<>);
                }).MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0].GetGenericArguments()[0]);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { objetoAConvertir, result },
                        Expression.Assign(objetoAConvertir, incomingObservable),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the enumerable to list"))),
                                    Expression.IfThen(
                                        Expression.NotEqual(objetoAConvertir, Expression.Constant(null)),
                                        Expression.Assign(result, Expression.Call(methodToObservable, objetoAConvertir))
                                    ),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable to list")))
                                    ),
                                Expression.Catch(
                                     paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE59, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE62, actualNode.NodeText), e);
            }
        }

        #endregion Observable extensions

        #region Enumerable extensions

        #region Enumerable agregation functions

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
                if (incomingObservable.Type.IsGenericType)
                {
                    if (!incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IList<>)) && !incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGroupedObservable<,>)) && !incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGrouping<,>)))
                    {
                        try
                        {
                            incomingObservable = this.actualScope.GetParameterByGenericType(0, typeof(IList<>), typeof(IGroupedObservable<,>), typeof(IGrouping<,>));
                        }
                        catch (Exception e)
                        {
                            throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE11, actualNode.NodeText), e);
                        }
                    }
                }
                else
                {
                    try
                    {
                        incomingObservable = this.actualScope.GetParameterByGenericType(0, typeof(IList<>), typeof(IGroupedObservable<,>), typeof(IGrouping<,>));
                    }
                    catch (Exception e)
                    {
                        throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE11, actualNode.NodeText), e);
                    }
                }

                MethodInfo methodCount = null;
                if (incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGroupedObservable<,>)) || incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGrouping<,>)))
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
        private Expression CreateEnumerableAgregationFunctionWithArgument<I>(PlanNode actualNode, Expression incomingObservable, Expression selector)
        {
            try
            {
                ParameterExpression param = Expression.Variable(selector.Type, "resultExtensionWithoutParametersObservable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                Type incommingTypeForFunction = null;

                if (incomingObservable.Type.IsGenericType)
                {
                    if (!incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IList<>)) && !incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGroupedObservable<,>)) && !incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGrouping<,>)))
                    {
                        try
                        {
                            incomingObservable = this.actualScope.GetParameterByGenericType(0, typeof(IList<>), typeof(IGroupedObservable<,>), typeof(IGrouping<,>));
                        }
                        catch (Exception e)
                        {
                            throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE13, actualNode.NodeText), e);
                        }
                    }
                }
                else
                {
                    try
                    {
                        incomingObservable = this.actualScope.GetParameterByGenericType(0, typeof(IList<>), typeof(IGroupedObservable<,>), typeof(IGrouping<,>));
                    }
                    catch (Exception e)
                    {
                        throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE13, actualNode.NodeText), e);
                    }
                }

                if (incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGroupedObservable<,>)) || incomingObservable.Type.GetGenericTypeDefinition().Equals(typeof(IGrouping<,>)))
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

                Expression selectorLambda = Expression.Lambda(delegateType, selector, new ParameterExpression[] { this.actualScope.GetFirstParameter() });
                this.PopScope();

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

        #endregion Enumerable agregation functions

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
        /// Call enumerable ToList extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression ToList.</returns>
        private Expression EnumerableToArray(PlanNode actualNode, Expression incomingObservable)
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

                ParameterExpression result = Expression.Variable(incomingObservable.Type.GetGenericArguments()[0].MakeArrayType(), "ToArrayParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                ParameterExpression objetoAConvertir = Expression.Variable(incomingObservable.Type, "ObjetoAConvertirAArreglo");

                MethodInfo methodToArray = typeof(System.Linq.Enumerable).GetMethods().Where(m =>
                {
                    return m.Name == "ToArray" && m.GetParameters().Length == 1;
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
                                        Expression.NotEqual(objetoAConvertir, Expression.Constant(null, objetoAConvertir.Type)),
                                        Expression.Assign(result, Expression.Call(methodToArray, objetoAConvertir))
                                    ),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable to list")))
                                    ),
                                Expression.Catch(
                                     paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("************************* INCOMING TYPE: " + incomingObservable.Type.ToString())),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("************************* Message ")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "Message")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("************************* Source ")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "Source")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("************************* TargetSite ")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "TargetSite")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("************************* StackTrace ")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "StackTrace")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("************************* HelpLink ")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "HelpLink")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("************************* InnerException ")),
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "InnerException")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE6, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE62, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Call enumerable ToList extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression ToList.</returns>
        private Expression EnumerableToObservable(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingObservable.Type.GetElementType()), "ToObservableParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                ParameterExpression objetoAConvertir = Expression.Variable(incomingObservable.Type, "ObjetoAConvertirAObservable");

                MethodInfo methodToObservable = typeof(System.Reactive.Linq.Observable)
                    .GetMethods()
                    .Where(m => m.Name == "ToObservable" && m.GetParameters().Length == 2)
                    .Single().MakeGenericMethod(incomingObservable.Type.GetElementType());

                System.Reactive.Concurrency.IScheduler scheduler = this.context.Scheduler.GetScheduler();
                Expression schedulerExp = Expression.Constant(scheduler);

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { objetoAConvertir, result },
                        Expression.Assign(objetoAConvertir, incomingObservable),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the enumerable to list"))),
                                    Expression.IfThen(
                                        Expression.NotEqual(objetoAConvertir, Expression.Constant(null)),
                                        Expression.Assign(result, Expression.Call(methodToObservable, objetoAConvertir, schedulerExp))
                                    ),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the enumerable to list")))
                                    ),
                                Expression.Catch(
                                     paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE61, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        result
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE62, actualNode.NodeText), e);
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
        /// Creates a new Observable.Where expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="filter">Filter expression.</param>
        /// <returns>Observable where expression.</returns>
        private Expression CreateEnumerableWhere(PlanNode actualNode, Expression incomingObservable, Expression filter)
        {
            Type incommingGenericArgument = incomingObservable.Type.GetGenericArguments()[0];
            ParameterExpression result = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(incommingGenericArgument), "ResultEnumerableWhere");
            ParameterExpression conditionResult = Expression.Variable(typeof(bool));
            ParameterExpression paramExceptionForWhereBody = Expression.Variable(typeof(Exception));
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            try
            {
                var delegateType = typeof(Func<,>).MakeGenericType(incommingGenericArgument, filter.Type);
                Expression selectorLambda = Expression.Lambda(delegateType, filter, new ParameterExpression[] { this.actualScope.GetParameterByType(incommingGenericArgument) });
                this.PopScope();

                MethodInfo methodWhere = typeof(System.Linq.Enumerable).GetMethods().Where(m =>
                {
                    return m.Name == "Where" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,System.Boolean]");
                })
                .Single().MakeGenericMethod(incommingGenericArgument);

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
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE59, actualNode.NodeText), e);
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
                this.PopScope();

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
                this.PopScope();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE54, actualNode.NodeText), e);
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
                this.PopScope();

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
        private Expression CreateSelectForEnumerable(PlanNode actualNode, Expression incomingObservable, Expression expProjection)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(((LambdaExpression)expProjection).ReturnType), "SelectForEnumerable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception), "SelectForEnumerableException");

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
                this.PopScope();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE23, actualNode.NodeText), e);
            }
        }

        #endregion Enumerable extensions

        #region Methods to call the lock method to the incoming events

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

                Expression lockBlock =
                    Expression.Block(
                        new[] { lambdaResult },
                        Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start call event lock method."))),
                                        Expression.Assign(lambdaResult, Expression.Call(this.actualScope.GetFirstParameter(), lockMethod, Expression.Constant(this.context.QueryName))),
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

                Type delegateType = typeof(Func<,>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0], lockBlock.Type);
                LambdaExpression lambdaSelect = Expression.Lambda(delegateType, lockBlock, new ParameterExpression[] { this.actualScope.GetFirstParameter() });

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
                this.PopScope();

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE58, actualNode.NodeText), e);
            }
        }

        #endregion Methods to apply the lock method to the incoming events

        #region Projections

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
            List<FieldNode> listOfFields = new List<FieldNode>();

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
                    int incidencias = 0;
                    if (plan.Children[1].Properties.ContainsKey("IncidenciasEnOn"))
                    {
                        incidencias = int.Parse(plan.Children[1].Properties["IncidenciasEnOn"].ToString());
                    }

                    listOfFields.Add(new FieldNode(key.Value.ToString(), value.Type, incidencias));
                }
            }

            // creo el tipo a retornar para la proyección, si es una proyección para el select el tipo debe heredar de EventResult
            Type myType = default(Type);
            ParameterExpression y = null;
            if (((PlanNodeTypeEnum)plans.Properties["ProjectionType"]).Equals(PlanNodeTypeEnum.ObservableSelect))
            {
                myType = LanguageTypeBuilder.CompileResultType(listOfFields);
                y = Expression.Variable(myType);
                expressionList.Add(Expression.Assign(y, Expression.New(myType.GetConstructor(new Type[] { }))));
            }
            else if (((PlanNodeTypeEnum)plans.Properties["ProjectionType"]).Equals(PlanNodeTypeEnum.ObservableExtractedEventData))
            {
                myType = LanguageTypeBuilder.CompileExtractedEventDataSpecificTypeForJoin(listOfFields);
                y = Expression.Variable(myType);
                expressionList.Add(Expression.Assign(y, Expression.New(myType.GetConstructor(new Type[] { }))));
            }
            else if (((PlanNodeTypeEnum)plans.Properties["ProjectionType"]).Equals(PlanNodeTypeEnum.ObservableExtractedEventDataComparer))
            {
                Type leftSourceExtractedEventDataType = (Type)plans.Properties["ParentType"];
                Type rightSourceExtractedEventDataType = (Type)plans.Properties["OtherSourceType"];
                Type delegateTypeOnCondition = null;

                if (!this.isSecondSource)
                {
                    this.onNode.Children.RemoveAt(0);
                    ScopeParameter[] newScopeParameters = new ScopeParameter[] { new ScopeParameter(0, leftSourceExtractedEventDataType), new ScopeParameter(1, rightSourceExtractedEventDataType) };
                    this.onNode.Properties.Add("ScopeParameters", newScopeParameters);

                    delegateTypeOnCondition = typeof(Func<,,>).MakeGenericType(leftSourceExtractedEventDataType, rightSourceExtractedEventDataType, typeof(bool));
                }
                else
                {
                    ScopeParameter[] newScopeParameters = new ScopeParameter[] { new ScopeParameter(0, rightSourceExtractedEventDataType), new ScopeParameter(1, leftSourceExtractedEventDataType) };
                    this.onNode.Properties["ScopeParameters"] = newScopeParameters;

                    delegateTypeOnCondition = typeof(Func<,,>).MakeGenericType(rightSourceExtractedEventDataType, leftSourceExtractedEventDataType, typeof(bool));
                }

                this.CreateNewScope(this.onNode, null, null);

                this.isInConditionOn = true;
                Expression onCondition = this.GenerateExpressionTree(this.onNode.Children.First());
                this.isInConditionOn = false;

                ParameterExpression resultOnCondition = Expression.Parameter(typeof(bool), "onConditionResult");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                Expression tryCatchExpr =
                    Expression.Block(
                            new[] { resultOnCondition },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(true), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("*********************************************************************************** ENTRÓ"))),
                                    Expression.Assign(resultOnCondition, onCondition),
                                    Expression.IfThen(Expression.Constant(true), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("*********************************************************************************** SALIÓ")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(0, 0, RUNTIME_ERRORS.RE57, string.Empty), typeof(string)), paramException))
                                    )
                                )
                            ),
                        resultOnCondition
                        );
                
                Expression block = Expression.Block(
                    new[] { resultOnCondition },
                    Expression.Assign(resultOnCondition, onCondition),
                    resultOnCondition);                    

                LambdaExpression lambdaOnCondition = Expression.Lambda(delegateTypeOnCondition, block, new ParameterExpression[] { this.actualScope.GetParameterByIndex(0), this.actualScope.GetParameterByIndex(1) });

                this.PopScope();

                myType = LanguageTypeBuilder.CompileExtractedEventDataComparerTypeForJoin(listOfFields, leftSourceExtractedEventDataType, rightSourceExtractedEventDataType, this.isSecondSource, lambdaOnCondition);
                this.isSecondSource = true;
                y = Expression.Variable(myType);
                expressionList.Add(Expression.Assign(y, Expression.New(myType.GetConstructor(new Type[] { }))));
            }
            else
            {
                myType = LanguageTypeBuilder.CompileResultType(listOfFields, bool.Parse(plans.Properties["OverrideGetHashCodeMethod"].ToString()));
                y = Expression.Variable(myType);
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
            Expression expProjectionObject = Expression.Block(new[] { y }, expressionList);

            // creo la funcion Func<EventObject, "tipo creado en tiempo de ejecución"> del cual se creará el lambda
            var delegateType = typeof(Func<,>).MakeGenericType(this.actualScope.GetFirstParameter().Type, myType);

            // creo el lambda que será retornado como hijo derecho del nodo padre, por ejemplo un nodo select
            Expression lambda2 = Expression.Lambda(delegateType, expProjectionObject, new ParameterExpression[] { this.actualScope.GetFirstParameter() });

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
            List<FieldNode> listOfFields = new List<FieldNode>();

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
                    listOfFields.Add(new FieldNode(key.Value.ToString(), value.Type, 0));
                }
            }

            Type myType = LanguageTypeBuilder.CompileResultType(listOfFields, bool.Parse(plans.Properties["OverrideGetHashCodeMethod"].ToString()));
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
        /// Create the select that transforms the IEnumerable{EventResult} in a QueryResult{EventResult} object
        /// </summary>
        /// <param name="actualNode">Actual plan node</param>
        /// <returns>Expression result</returns>
        private Expression CreateProjectionForResult(PlanNode actualNode)
        {
            Expression incomingObservable = this.actualScope.GetFirstParameter();

            List<Expression> expressionList = new List<Expression>();

            // se agrega el print log que indica el inicio de la proyección
            expressionList.Add(Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the result projection"))));

            Type resultType = ResultTypeBuilder.CreateResultType(this.context.QueryName, incomingObservable.Type.GetGenericArguments()[0]);

            // inicializo el observer con el tipo creado a partir de la proyección
            if (this.observer == null)
            {
                this.observer = Expression.Parameter(typeof(IObserver<>).MakeGenericType(resultType));
            }

            MethodInfo toArray = typeof(System.Linq.Enumerable).GetMethods().Where(m =>
            {
                return m.Name == "ToArray" && m.GetParameters().Length == 1;
            })
            .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

            Expression arrayDeResultados = Expression.Call(toArray, incomingObservable);

            // inicializo el tipo creado, si es para un select hereda de EventResult e inicializo los parametros de su constructor
            ParameterExpression y = Expression.Variable(resultType);
            expressionList.Add(
                Expression.Assign(
                        y,
                        Expression.New(
                            resultType.GetConstructors()[0],
                            Expression.Constant(this.context.QueryName),
                            Expression.Property(null, typeof(DateTime).GetProperty("Now")),
                            arrayDeResultados
                        )
                    )
            );

            // se agrega el print log que indica la finalización de la proyección
            expressionList.Add(Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the result projection"))));

            // se agrega el objeto de la proyección resultante
            expressionList.Add(y);

            // se crea el bloque que establece los valores a retornar en el proyección
            Expression expProjectionObject = Expression.Block(
                                                        new[] { y },
                                                        expressionList
                                                );

            // creo la funcion Func<EventObject, "tipo creado en tiempo de ejecución"> del cual se creará el lambda
            var delegateType = typeof(Func<,>).MakeGenericType(this.actualScope.GetFirstParameter().Type, resultType);

            // creo el lambda que será retornado como hijo derecho del nodo padre, por ejemplo un nodo select
            return Expression.Lambda(delegateType, expProjectionObject, new ParameterExpression[] { this.actualScope.GetFirstParameter() });
        }

        /// <summary>
        /// Join projection, create the tuple to return by the observer.
        /// </summary>
        /// <param name="actualNode">Actual node.</param>
        /// <returns>Tuple expression.</returns>
        private Expression CreateJoinProjection(PlanNode actualNode)
        {
            try
            {
                ParameterExpression leftItem = null;
                ParameterExpression rightItem = null;

                ParameterExpression result = null;
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo methodCreate = typeof(Tuple).GetMethods()
                                                .Where(m => m.Name == "Create" && m.GetParameters().Length == 2)
                                                .Single();
                Type leftType = null;
                Type rightType = null;
                Expression valueToReturn = null;
                if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinLeftDuration))
                {
                    leftItem = this.actualScope.GetParameterByIndex(0);
                    rightItem = this.actualScope.ParentScope.ParentScope.GetParameterByIndex(1);
                    leftType = leftItem.Type;
                    rightType = rightItem.Type.GetGenericArguments()[0];
                    methodCreate = methodCreate.MakeGenericMethod(leftType, rightType);
                    valueToReturn = Expression.Call(methodCreate, leftItem, Expression.Constant(null, rightType));
                }
                else if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinRightDuration))
                {
                    leftItem = this.actualScope.ParentScope.ParentScope.GetParameterByIndex(0);
                    rightItem = this.actualScope.GetParameterByIndex(0);
                    leftType = leftItem.Type.GetGenericArguments()[0];
                    rightType = rightItem.Type;
                    methodCreate = methodCreate.MakeGenericMethod(leftType, rightType);
                    valueToReturn = Expression.Call(methodCreate, Expression.Constant(null, leftType), rightItem);
                }
                else if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinResultSelector))
                {
                    leftItem = this.actualScope.GetParameterByIndex(0);
                    rightItem = this.actualScope.GetParameterByIndex(1);
                    leftType = leftItem.Type;
                    rightType = rightItem.Type;
                    methodCreate = methodCreate.MakeGenericMethod(leftType, rightType);
                    valueToReturn = Expression.Call(methodCreate, Expression.Constant(leftItem, leftType), Expression.Constant(rightItem, rightType));
                }

                result = Expression.Variable(typeof(Tuple<,>).MakeGenericType(leftType, rightType), "JoinProjection");

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { result },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start the select for enumerable"))),
                                    Expression.Assign(result, valueToReturn),
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

                Type delegateType = null;
                LambdaExpression lambda = null;
                if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinLeftDuration))
                {
                    delegateType = typeof(Func<,>).MakeGenericType(leftType, result.Type);
                    lambda = Expression.Lambda(delegateType, tryCatchExpr, leftItem);
                }
                else if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinRightDuration))
                {
                    delegateType = typeof(Func<,>).MakeGenericType(rightType, result.Type);
                    lambda = Expression.Lambda(delegateType, tryCatchExpr, rightItem);
                }
                else if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinResultSelector))
                {
                    delegateType = typeof(Func<,,>).MakeGenericType(leftType, rightType, result.Type);
                    lambda = Expression.Lambda(delegateType, tryCatchExpr, leftItem, rightItem);
                }

                return lambda;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE61, actualNode.NodeText), e);
            }
        }

        #endregion Projections

        #region Dispose events

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

            if (this.actualScope.GetFirstParameter().Type.IsGenericType)
            {
                try
                {
                    // se hace el ToList del observable entrante para poder hacer ForEach
                    Expression toList = this.EnumerableToList(actualNode, this.actualScope.GetFirstParameter());

                    // new scope para el foreach
                    this.CreateNewScope(actualNode, toList, null);

                    // obtenemos el metodo foreach
                    MethodInfo methodForEach = typeof(System.Collections.Generic.List<>).MakeGenericType(toList.Type.GetGenericArguments()[0]).GetMethods().Where(m =>
                    {
                        return m.Name == "ForEach" && m.GetParameters().Length == 1;
                    })
                    .Single();

                    // este pop es del CreateNewScope al inicio de este bloque
                    ParameterExpression evento = this.actualScope.GetFirstParameter();
                    this.PopScope();

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
                    ParameterExpression evento = this.actualScope.GetFirstParameter();
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

        #endregion Dispose events

        #region Access to event values

        /// <summary>
        /// Create a expression tree.
        /// </summary>
        /// <param name="actualNode">plan node to convert.</param>
        /// <param name="sourceId">Name of the source, same at the from, join or with statement.</param>
        /// <returns>expression tree of actual plan.</returns>
        private Expression GenerateEvent(PlanNode actualNode, ConstantExpression sourceId)
        {
            return this.actualScope.GetFirstParameter();

            /*
            return param;

            if (this.sources.ContainsValue(sourceId.Value.ToString()))
            {
                int paramToTake = this.sources.First(x => x.Value == sourceId.Value.ToString()).Key;

                ParameterExpression param = this.actualScope.GetFirstParameter();

                if (param.Type.IsGenericType)
                {
                    if (param.Type.GetGenericTypeDefinition().Equals(typeof(Tuple<,>)))
                    {
                        if (paramToTake == LEFTSOURCE)
                        {
                            return Expression.Property(param, "Item1");
                        }
                        else if (paramToTake == RIGHTSOURCE)
                        {
                            return Expression.Property(param, "Item2");
                        }
                    }
                }

                return param;
            }
            else
            {
                throw new Exceptions.CompilationException(string.Format("Invalid source {0}.", sourceId.Value));
            }
            */
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

        #region Access to event properties

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
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateProperty(PlanNode actualNode, Expression leftNode)
        {
            string propiedad = actualNode.Properties["Property"].ToString();
            MethodInfo getItemNMethod = null;
            Expression tupleItem = leftNode;
            MethodInfo aux1 = null;

            if (leftNode.Type.IsGenericType && leftNode.Type.GetGenericTypeDefinition().Equals(typeof(Tuple<,>)))
            {
                int paramToTake = int.Parse(actualNode.Children[0].Properties["ParameterPosition"].ToString());

                if (this.sources.ContainsKey(paramToTake))
                {
                    if (paramToTake == LEFTSOURCE)
                    {
                        // tupleItem = Expression.Property(leftNode, "Item1");
                        getItemNMethod = leftNode.Type.GetMethod("get_Item1");
                        aux1 = leftNode.Type.GetMethod("get_Item1").ReturnType.GetProperty(propiedad).GetGetMethod();

                        tupleItem = Expression.Call(leftNode, getItemNMethod);
                        /*tupleItem = Expression.Call(tupleItem, aux1);*/
                    }
                    else if (paramToTake == RIGHTSOURCE)
                    {
                        // tupleItem = Expression.Property(leftNode, "Item2");
                        getItemNMethod = leftNode.Type.GetMethod("get_Item2");
                        aux1 = leftNode.Type.GetMethod("get_Item2").ReturnType.GetProperty(propiedad).GetGetMethod();

                        tupleItem = Expression.Call(leftNode, getItemNMethod);
                        /*tupleItem = Expression.Call(tupleItem, aux1);*/
                    }
                }
                else
                {
                    throw new Exceptions.CompilationException("Invalid source.");
                }
            }
            else
            {
                aux1 = leftNode.Type.GetProperty(propiedad).GetGetMethod();
            }

            MethodCallExpression expGetProperty = Expression.Call(tupleItem, aux1);
            Type tipoDeLaPropiedad = expGetProperty.Type;

            try
            {
                ParameterExpression param = Expression.Variable(tipoDeLaPropiedad, "PropertyValue");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                bool propExists = tupleItem.Type.GetProperties().Any(x => x.Name == propiedad);

                Expression aux =
                    Expression.IfThenElse(
                            Expression.Equal(Expression.Constant(propExists), Expression.Constant(false)),
                            Expression.Assign(param, Expression.Default(tipoDeLaPropiedad)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the get property operation: " + propiedad))),
                                    Expression.Assign(param, expGetProperty),                                    
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the get property operation: " + propiedad)))
                                ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la propiedad " + propiedad + ", error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE5, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            )
                        );

                Expression tryCatchExpr = null;
                if (leftNode.Type.IsGenericType && leftNode.Type.GetGenericTypeDefinition().Equals(typeof(Tuple<,>)))
                {
                    tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThen(
                                Expression.AndAlso(Expression.NotEqual(leftNode, Expression.Constant(null, leftNode.Type)), Expression.NotEqual(Expression.Call(leftNode, getItemNMethod), Expression.Constant(null, tupleItem.Type))),
                                aux
                            ),
                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("The value of the property is: "))),
                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Convert(param, typeof(object)))),
                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant(string.Empty))),
                            param);
                }
                else
                {
                    tryCatchExpr =
                        Expression.Block(
                            new[] { param },
                            Expression.IfThen(
                                Expression.NotEqual(leftNode, Expression.Constant(null, leftNode.Type)),
                                aux
                            ),
                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("The value of the property is: "))),
                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Convert(param, typeof(object)))),
                            Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant(string.Empty))),
                            param);
                }

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Resources.COMPILATION_ERRORS.CE49(propiedad), actualNode.NodeText), e);
            }
        }

        #region Access to event message properties

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

        #endregion Access to event message properties

        #endregion Access to event properties

        #endregion Access to event values

        #region Cast

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

        #endregion Cast

        #region Constant

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

        #endregion Constant

        #region Identifier

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

        #endregion Identifier

        #region Operations

        #region Comparative operations

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

        #endregion Comparative operations

        #region Arithmetic operations

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

        #endregion Arithmetic operations

        #region Unary operations

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

        #endregion Unary operations

        #region Logic operations

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
        private Expression GenerarNot(PlanNode actualNode, Expression leftNode)
        {
            try
            {
                ParameterExpression param = Expression.Variable(typeof(bool), "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                ParameterExpression aux = Expression.Variable(leftNode.Type, "ValorParaElNot");

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { aux, param },
                        Expression.Assign(aux, leftNode),
                        Expression.IfThenElse(
                            Expression.TypeEqual(aux, typeof(bool)),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'not' operation: "))),
                                    Expression.Assign(param, Expression.Not(aux)),
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

        #endregion Logic operations

        #endregion Operations

        #region Access to group key object

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
                            Expression.Assign(param, Expression.Default(groupKeyProperty.Type)),
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

        #endregion Access to group key object

        #region Helpers

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

        #endregion Helpers
    }
}