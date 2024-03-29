﻿//-----------------------------------------------------------------------
// <copyright file="CodeGenerator.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reflection;
    using Language.Exceptions;
    using Language;
    using System.Reflection.Emit;
    using Ninject;

    /// <summary>
    /// Observable constructor class
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1111:ClosingParenthesisMustBeOnLineOfLastParameter", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")] // se debe quitar
    internal class CodeGenerator
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
        /// Compilation context.
        /// </summary>
        private CodeGeneratorConfiguration config;

        /// <summary>
        /// Observer of the ObservableCreate node.
        /// </summary>
        private ParameterExpression observer;

        /// <summary>
        /// Indicates whether the compiler is generating the condition of the query section 'ON'
        /// </summary>
        private bool isInConditionOn;

        /// <summary>
        /// Dictionary of lag variables for join query.
        /// </summary>
        private Dictionary<int, ParameterExpression> lagVariables;

        /// <summary>
        /// Buffer size of join sources expression.
        /// </summary>
        private ParameterExpression bufferSizeOfJoinSourcesExpression;

        /// <summary>
        /// Scheduler defined in the compile context.
        /// </summary>
        private ParameterExpression schedulerExpression;

        #endregion Field definitions

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGenerator"/> class
        /// </summary>
        /// <param name="config">Compilation context.</param>
        public CodeGenerator(CodeGeneratorConfiguration config)
        {
            if (config.SchedulerFactory == null)
            {
                throw new CompilationException(COMPILATION_ERRORS.CE65);
            }

            this.config = config;

            this.scopeLevel = 0;
            this.parameterList = new List<ParameterExpression>();
            this.sources = new Dictionary<int, string>();
            this.isInConditionOn = false;
            this.IsSecondSource = false;
            this.lagVariables = new Dictionary<int, ParameterExpression>();

            this.bufferSizeOfJoinSourcesExpression = Expression.Variable(typeof(TimeSpan), "bufferSizeOfJoinSources");
            this.schedulerExpression = Expression.Variable(typeof(System.Reactive.Concurrency.IScheduler), "Scheduler");
        }

        #endregion Constructors

        #region Compile methods

        /// <summary>
        /// Gets or sets the ON node.
        /// </summary>
        public PlanNode OnNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the projection for extract event data is for the second source
        /// </summary>
        public bool IsSecondSource { get; set; }

        /// <summary>
        /// Gets the dictionary of sources
        /// </summary>
        public Dictionary<int, string> Sources
        {
            get
            {
                return this.sources;
            }
        }

        /// <summary>
        /// Gets the parameter list.
        /// </summary>
        public List<ParameterExpression> ParameterList
        {
            get
            {
                return this.parameterList;
            }
        }

        /// <summary>
        /// Creates a delegate
        /// </summary>
        /// <param name="plan">Execution plan</param>
        /// <returns>Result delegate.</returns>
        public Assembly Compile(PlanNode plan)
        {
            ConstructorInfo ctrTimeSpan = typeof(TimeSpan).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) });
            int bufferSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]);
            this.lagVariables.Add(0, Expression.Variable(typeof(TimeSpan), "lagIzq"));
            this.lagVariables.Add(1, Expression.Variable(typeof(TimeSpan), "lagDer"));

            Expression rootExpression = this.GenerateExpressionTree(plan);

            Expression setSchedulerExp = null;
            if (this.config.IsTestMode)
            {
                ParameterExpression schedulerParam = Expression.Parameter(typeof(System.Reactive.Concurrency.IScheduler), "SchedulerGlobalParam");
                setSchedulerExp = Expression.Assign(this.schedulerExpression, schedulerParam);
                this.parameterList.Add(schedulerParam);
            }
            else
            {
                setSchedulerExp = Expression.Assign(this.schedulerExpression, this.config.SchedulerFactory.GetScheduler());
            }

            Expression rootBlock = Expression.Block(
                                        new[] { this.lagVariables[0], this.lagVariables[1], this.bufferSizeOfJoinSourcesExpression, this.schedulerExpression },
                                        setSchedulerExp,
                                        Expression.Assign(this.bufferSizeOfJoinSourcesExpression, Expression.New(ctrTimeSpan, Expression.Constant(0), Expression.Constant(0), Expression.Constant(0), Expression.Constant(0), Expression.Constant(bufferSize))),
                                        Expression.Assign(this.lagVariables[0], Expression.Default(this.lagVariables[0].Type)),
                                        Expression.Assign(this.lagVariables[1], Expression.Default(this.lagVariables[1].Type)),
                                        rootExpression
                                        );

            LambdaExpression lambda = Expression.Lambda(rootBlock, this.parameterList.ToArray());

            SpaceQueryTypeBuilder sqtb = new SpaceQueryTypeBuilder(this.config.AsmBuilder, this.config.QueryName, lambda);
            Assembly assembly = sqtb.CreateNewAssembly();

            this.actualScope = null;
            this.parameterList.Clear();
            this.sources.Clear();
            this.scopeLevel = 0;
            this.isInConditionOn = false;
            this.IsSecondSource = false;

            Console.WriteLine("El assembly fue creado exitosamente.");
            return assembly;
        }

        /// <summary>
        /// Compile the result function.
        /// </summary>
        /// <param name="plan">Execution plan.</param>
        /// <returns>Result function.</returns>
        public Delegate CompileDelegate(PlanNode plan)
        {
            Delegate funcResult = this.CreateLambda(plan).Compile();

            this.actualScope = null;
            this.parameterList.Clear();
            this.sources.Clear();
            this.scopeLevel = 0;
            this.isInConditionOn = false;
            this.IsSecondSource = false;

            Console.WriteLine("La función fue compilada exitosamente.");
            return funcResult;
        }

        #endregion Compile methods

        #region CreateLambda methods

        /// <summary>
        /// Creates a lambda expression
        /// </summary>
        /// <param name="plan">Execution plan</param>
        /// <returns>Expression lambda</returns>
        public LambdaExpression CreateLambda(PlanNode plan)
        {
            ConstructorInfo ctrTimeSpan = typeof(TimeSpan).GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) });
            int bufferSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]);
            this.lagVariables.Add(0, Expression.Variable(typeof(TimeSpan), "lagIzq"));
            this.lagVariables.Add(1, Expression.Variable(typeof(TimeSpan), "lagDer"));

            Expression rootExpression = this.GenerateExpressionTree(plan);

            Expression setSchedulerExp = null;
            if (this.config.IsTestMode)
            {
                ParameterExpression schedulerParam = Expression.Parameter(typeof(System.Reactive.Concurrency.IScheduler), "SchedulerGlobalParam");
                setSchedulerExp = Expression.Assign(this.schedulerExpression, schedulerParam);
                this.parameterList.Add(schedulerParam);
            }
            else
            {
                setSchedulerExp = Expression.Assign(this.schedulerExpression, this.config.SchedulerFactory.GetScheduler());
            }

            Expression rootBlock = Expression.Block(
                                        new[] { this.lagVariables[0], this.lagVariables[1], this.bufferSizeOfJoinSourcesExpression, this.schedulerExpression },
                                        setSchedulerExp,
                                        Expression.Assign(this.bufferSizeOfJoinSourcesExpression, Expression.New(ctrTimeSpan, Expression.Constant(0), Expression.Constant(0), Expression.Constant(0), Expression.Constant(0), Expression.Constant(bufferSize))),
                                        Expression.Assign(this.lagVariables[0], Expression.Default(this.lagVariables[0].Type)),
                                        Expression.Assign(this.lagVariables[1], Expression.Default(this.lagVariables[1].Type)),
                                        rootExpression
                                        );

            LambdaExpression result = Expression.Lambda(rootBlock, this.parameterList.ToArray());
            return result;
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

            Expression rootBlock = Expression.Block(
                                        new[] { this.schedulerExpression },
                                        Expression.Assign(this.schedulerExpression, this.config.SchedulerFactory.GetScheduler()),
                                        rootExpression
                                        );

            Expression<Func<In, Out>> result = Expression.Lambda<Func<In, Out>>(rootBlock, this.parameterList.ToArray());
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
                case PlanNodeTypeEnum.MathFunctionWithOneParameter:
                    expResult = this.CreateMathFunctionWithOneParameter(actualNode, leftNode);
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
                case PlanNodeTypeEnum.ApplyDuration:
                case PlanNodeTypeEnum.ApplyRepetition:
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
                    this.CreateNewScope(actualNode, leftNode);
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
                    // expResult = new ObservableNeverConverter(this.config).Transform(new CompilationContext() { ActualNode = actualNode }).ResultExpression;
                    expResult = this.CreateObservableNever(actualNode);
                    break;
                case PlanNodeTypeEnum.ObservableEmpty:
                    expResult = this.CreateObservableEmpty(actualNode);
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
                case PlanNodeTypeEnum.ObservableConcat:
                    expResult = this.ObservableConcat(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.IsNullFunction:
                    expResult = this.CreateIsNullFunction(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.BufferSizeForJoin:
                    expResult = this.CreateBufferSizeForJoin(actualNode, leftNode);
                    break;
                /********************************************************************************************************************************************************************************************************************/
                case PlanNodeTypeEnum.Cast:
                    return this.GenerateCast(actualNode, leftNode);
                case PlanNodeTypeEnum.Equal:
                    return this.GenerarEqual(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.NotEqual:
                    expResult = this.GenerarNotEqual(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.LessThan:
                    expResult = this.GenerarLessThan(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.LessThanOrEqual:
                    expResult = this.GenerarLessThanOrEqual(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.GreaterThan:
                    expResult = this.GenerarGreaterThan(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.GreaterThanOrEqual:
                    expResult = this.GenerarGreaterThanOrEqual(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.Not:
                    expResult = this.GenerarNot(actualNode, leftNode);
                    break;
                case PlanNodeTypeEnum.Like:
                    expResult = this.GenerarLike(actualNode, leftNode, rightNode);
                    break;
                case PlanNodeTypeEnum.Or:
                    return this.GenerateOr(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.And:
                    return this.GenerateAnd(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.DateTimeFunction:
                    return this.GenerateDateFunction(actualNode, leftNode);
                case PlanNodeTypeEnum.Negate:
                    return this.GenerateNegate(actualNode, leftNode);
                case PlanNodeTypeEnum.Subtract:
                    return this.GenerateSubtract(actualNode, leftNode, rightNode);
                case PlanNodeTypeEnum.Property:
                case PlanNodeTypeEnum.EventProperty:
                    return this.GenerateProperty(actualNode, leftNode);
                case PlanNodeTypeEnum.ObservableToEnumerable:
                    return this.ObservableToEnumerable(actualNode, leftNode);
                default:
                    throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE1(nodeType.ToString()), actualNode.NodeText));
            }

            return expResult;
        }

        #endregion Depth-first search (post-order) and method selector

        #region code generator aux functions

        /// <summary>
        /// Generate event block if the compilation is in debug mode.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="parameters">Expression block parameters.</param>
        /// <param name="mainAction">Action with the main functionality of the expression block.</param>
        /// <param name="actionsBeforeMainAction">Expressions before main action.</param>
        /// <param name="resultParameter">Result parameter, this expression will be the last expression of the block, is the return value.</param>
        /// <param name="assignable">Indicates whether the result parameter will be assigned to the main action</param>
        /// <param name="startMessage">Message to print where an event enter to this block.</param>
        /// <param name="endMessage">Message to print where an event leave this block.</param>
        /// <param name="runtimeError">Runtime error to print if an error occurs.</param>
        /// <returns>Doc goes here.</returns>
        private Expression GenerateEventBlock(PlanNode actualNode, IEnumerable<ParameterExpression> parameters, Expression mainAction, List<Expression> actionsBeforeMainAction, ParameterExpression resultParameter, bool assignable, string startMessage, string endMessage, string runtimeError)
        {
            Contract.Requires(actualNode != null);
            Contract.Requires(parameters != null);
            Contract.Requires(mainAction != null);
            Contract.Requires(actionsBeforeMainAction != null);
            Contract.Requires(startMessage != null && startMessage != string.Empty);
            Contract.Requires(endMessage != null && endMessage != string.Empty);
            Contract.Requires(runtimeError != null && runtimeError != string.Empty);

            Expression aux = null;
            if (this.config.DebugMode)
            {
                if (assignable)
                {
                    aux = Expression.Assign(resultParameter, mainAction);
                }
                else
                {
                    aux = mainAction;
                }

                actionsBeforeMainAction.Add(aux);
            }
            else
            {
                if ((actionsBeforeMainAction != null && actionsBeforeMainAction.Count > 0) || this.config.MeasureElapsedTime)
                {
                    if (assignable)
                    {
                        aux = Expression.Assign(resultParameter, mainAction);
                    }
                    else
                    {
                        aux = mainAction;
                    }

                    actionsBeforeMainAction.Add(aux);
                }
                else
                {
                    if (mainAction.Type.Equals(typeof(void)) && resultParameter != null)
                    {
                        return Expression.Block(new[] { resultParameter }, mainAction, resultParameter);
                    }
                    else
                    {
                        return mainAction;
                    }
                }
            }

            List<ParameterExpression> parametersAux = parameters.ToList();
            Expression[] body = this.GenerateExpressionBlockBody(actualNode, actionsBeforeMainAction, parametersAux, resultParameter, startMessage, endMessage, runtimeError);

            if (parametersAux.Count() > 0)
            {
                return Expression.Block(parametersAux, body);
            }
            else
            {
                return Expression.Block(body);
            }
        }

        /// <summary>
        /// Creates the body of the expression block
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="actionsBeforeResultExpression">Expressions before the result expression.</param>
        /// <param name="parameters">Expression block parameters.</param>
        /// <param name="resultParameter">Result parameter, this expression will be the last expression of the block, is the return value.</param>
        /// <param name="startMessage">Message to print where an event enter to this block.</param>
        /// <param name="endMessage">Message to print where an event leave this block.</param>
        /// <param name="runtimeError">Runtime error to print if an error occurs.</param>
        /// <returns>Doc goes here.</returns>
        private Expression[] GenerateExpressionBlockBody(PlanNode actualNode, List<Expression> actionsBeforeResultExpression, List<ParameterExpression> parameters, ParameterExpression resultParameter, string startMessage, string endMessage, string runtimeError)
        {
            if (this.config.MeasureElapsedTime)
            {
                ParameterExpression watcher = Expression.Variable(typeof(System.Diagnostics.Stopwatch), "Watcher_" + actualNode.NodeType.ToString());
                parameters.Add(watcher);
                Expression startNew = Expression.Assign(watcher, Expression.Call(null, typeof(System.Diagnostics.Stopwatch).GetMethod("StartNew")));
                actionsBeforeResultExpression.Insert(0, startNew);
                actionsBeforeResultExpression.Insert(0, watcher);

                Expression stop = Expression.Call(watcher, typeof(System.Diagnostics.Stopwatch).GetMethod("Stop"));
                MethodInfo formatMethod = typeof(string).GetMethods().First(x => x.Name == "Format" && x.GetParameters().Count() == 3 && x.GetParameters()[1].ParameterType == typeof(object) && x.GetParameters()[2].ParameterType == typeof(object));
                Expression valueToPrint = Expression.Call(formatMethod, Expression.Constant("{0},{1}", typeof(string)), Expression.Convert(Expression.Constant("Node_" + actualNode.NodeType.ToString(), typeof(string)), typeof(object)), Expression.Convert(Expression.Property(Expression.Property(watcher, "Elapsed"), "Ticks"), typeof(object)));
                Expression printValue = Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(string) }), valueToPrint);
                actionsBeforeResultExpression.Insert(actionsBeforeResultExpression.Count, stop);
                actionsBeforeResultExpression.Insert(actionsBeforeResultExpression.Count, printValue);
            }

            if (this.config.PrintLog)
            {
                Expression start = Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant(startMessage));
                Expression end = Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant(startMessage));
                actionsBeforeResultExpression.Insert(0, start);
                actionsBeforeResultExpression.Insert(actionsBeforeResultExpression.Count, end);
            }

            if (this.config.DebugMode)
            {
                actionsBeforeResultExpression.Add(Expression.Empty());
                Expression tryCatch = this.WrapWithTryCatch(actualNode, runtimeError, actionsBeforeResultExpression);
                List<Expression> result = new List<Expression>();
                result.Add(tryCatch);
                if (resultParameter != null)
                {
                    result.Add(resultParameter);
                }

                return result.ToArray();
            }
            else
            {
                if (resultParameter != null)
                {
                    actionsBeforeResultExpression.Add(resultParameter);
                }

                return actionsBeforeResultExpression.ToArray();
            }
        }

        /// <summary>
        /// Wrap the expression block body in a try catch expression.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="runtimeError">Runtime error to print if an error occurs.</param>
        /// <param name="actions">Expressions of the try catch expression body.</param>
        /// <returns>Doc goes here.</returns>
        private Expression WrapWithTryCatch(PlanNode actualNode, string runtimeError, List<Expression> actions)
        {
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            Expression tryCatch = Expression.TryCatch(
                            Expression.Block(actions),
                            Expression.Catch(
                                paramException,
                                 Expression.Block(
                                    Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, runtimeError, actualNode.NodeText), typeof(string)), paramException))
                                )
                            )
                        );

            return tryCatch;
        }

        /// <summary>
        /// Creates the expression to get the specify property based in the compilation mode.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="property">Property name.</param>
        /// <param name="expGetProperty">Expression to get the specify property.</param>
        /// <param name="tipoDeLaPropiedad">Property type.</param>
        /// <param name="leftNodeType">Left node type.</param>
        /// <returns>Result expression to get the specify property.</returns>
        private Expression GenerateExpressionForGetPropertyValue(PlanNode actualNode, string property, MethodCallExpression expGetProperty, Type tipoDeLaPropiedad, Type leftNodeType)
        {
            ParameterExpression paramValorObtenido = Expression.Variable(this.ConvertToNullable(tipoDeLaPropiedad), "PropertyValue1");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            Expression aux = null;
            Expression aux2 = null;
            if (this.config.PrintLog)
            {
                if (this.config.DebugMode)
                {
                    aux2 = Expression.Block(
                             Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the get property operation: " + property))),
                             Expression.Assign(paramValorObtenido, this.StandardizeType(expGetProperty, expGetProperty.Type)),
                             Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the get property operation: " + property)))
                          );
                }
                else
                {
                    aux2 = Expression.Block(
                        new[] { paramValorObtenido },
                             Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the get property operation: " + property))),
                             Expression.Assign(paramValorObtenido, this.StandardizeType(expGetProperty, expGetProperty.Type)),
                             Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the get property operation: " + property))),
                             paramValorObtenido
                          );
                }
            }
            else
            {
                if (this.config.DebugMode)
                {
                    aux2 = Expression.Block(
                        Expression.Assign(paramValorObtenido, this.StandardizeType(expGetProperty, expGetProperty.Type)),
                        Expression.Empty()
                        );
                }
                else
                {
                    aux2 = this.StandardizeType(expGetProperty, expGetProperty.Type);
                }
            }

            if (this.config.DebugMode)
            {
                aux =
                    Expression.Block(
                        new[] { paramValorObtenido },
                        Expression.TryCatch(
                                    aux2,
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible obtener la propiedad " + property + ", error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE5, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                    )
                                ),
                        paramValorObtenido
                    );
            }
            else
            {
                aux = aux2;
            }

            return aux;
        }

        #endregion code generator aux functions

        #region Join

        /// <summary>
        /// Creates the buffer size for sources of join queries.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="bufferSizeOfJoinSources">Buffer size of join sources specified in the configuration file.</param>
        /// <returns>A expression to invoke to define the boundaries of the produced buffers.</returns>
        private Expression CreateBufferSizeForJoin(PlanNode actualNode, Expression bufferSizeOfJoinSources)
        {
            ParameterExpression result = Expression.Variable(typeof(IObservable<long>), "BufferSizeForJoin");

            MethodInfo methodTimer = typeof(Observable).GetMethods()
                                            .Where(m => m.Name == "Timer" && m.GetParameters().Length == 2 && m.GetParameters()[0].ParameterType == typeof(TimeSpan) && m.GetParameters()[1].ParameterType == typeof(System.Reactive.Concurrency.IScheduler))
                                            .Single();

            ParameterExpression lagVariable = null;
            string source = string.Empty;
            if (this.IsSecondSource)
            {
                lagVariable = this.lagVariables[1];
                source = "right source";
            }
            else
            {
                lagVariable = this.lagVariables[0];
                source = "left source";
            }

            // ConstantExpression bufferSizeOfJoinSources = Expression.Constant(double.Parse(System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]), typeof(double));
            Expression sum = Expression.Add(lagVariable, this.bufferSizeOfJoinSourcesExpression);

            // Expression bufferTime = Expression.Call(typeof(TimeSpan).GetMethod("FromMilliseconds"), sum);
            Expression timer = Expression.Call(methodTimer, sum, this.schedulerExpression);

            Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, timer, new List<Expression>(), result, true, string.Format("Start of the observable timer at {0}.", source), string.Format("End of the observable timer at {0}.", source), RUNTIME_ERRORS.RE82);

            LambdaExpression funcBufferSize = Expression.Lambda<Func<IObservable<long>>>(resultExpression);

            return funcBufferSize;
        }

        /// <summary>
        /// Creates an Observable.Never[Unit]() expression.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <returns>Result expression.</returns>
        private Expression CreateObservableNever(PlanNode actualNode)
        {
            Type genericType = typeof(Unit);
            ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(genericType), "ObservableNever");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            MethodInfo methodNever = typeof(Observable).GetMethods()
                                            .Where(m => m.Name == "Never" && m.GetParameters().Length == 0)
                                            .Single().MakeGenericMethod(genericType);

            Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(null, methodNever), new List<Expression>(), result, true, "Start of the observable never.", "End of the observable never.", RUNTIME_ERRORS.RE65);

            return resultExpression;
        }

        /// <summary>
        /// Creates an Observable.Empty[Unit]() expression.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <returns>Result expression.</returns>
        private Expression CreateObservableEmpty(PlanNode actualNode)
        {
            Type genericType = typeof(Unit);
            ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(genericType), "ObservableEmpty");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            MethodInfo methodEmpty = typeof(Observable).GetMethods()
                                            .Where(m => m.Name == "Empty" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler)))
                                            .Single().MakeGenericMethod(genericType);

            Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(null, methodEmpty, this.schedulerExpression), new List<Expression>(), result, true, "Start of the observable empty.", "End of the observable empty.", RUNTIME_ERRORS.RE66);

            return resultExpression;
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
                if (actualNode.Properties.ContainsKey("ReturnObservable") && bool.Parse(actualNode.Properties["ReturnObservable"].ToString()) == true)
                {
                    MethodInfo timeoutMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Timeout" && m.GetParameters().Length == 4 && m.GetParameters()[1].ParameterType.Equals(typeof(TimeSpan)) && m.GetParameters()[3].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler)); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                    valueResult = Expression.Call(timeoutMethod, incomingObservable, right, this.CreateObservableEmpty(actualNode), this.schedulerExpression);
                }
                else
                {
                    MethodInfo timeoutMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Timeout" && m.GetParameters().Length == 3 && m.GetParameters()[1].ParameterType.Equals(typeof(TimeSpan)) && m.GetParameters()[2].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler)); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                    valueResult = Expression.Call(timeoutMethod, incomingObservable, right, this.schedulerExpression);
                }

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, valueResult, new List<Expression>(), result, true, "Start of the observable timeout.", "End of the observable timeout.", RUNTIME_ERRORS.RE67);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE83, actualNode.NodeText), e);
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
                // para settear el atributo timeout a true
                ParameterExpression sideParam = null;

                if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinLeftDuration))
                {
                    sideParam = this.actualScope.ParentScope.GetParameterByIndex(0);
                }
                else if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinRightDuration))
                {
                    sideParam = this.actualScope.ParentScope.GetParameterByIndex(1);
                }

                Expression toList = this.EnumerableToList(actualNode, sideParam);

                // new scope para el foreach
                this.CreateNewScope(actualNode, toList);

                // obtenemos el metodo foreach
                MethodInfo methodForEach = typeof(System.Collections.Generic.List<>).MakeGenericType(toList.Type.GetGenericArguments()[0]).GetMethods().Where(m =>
                {
                    return m.Name == "ForEach" && m.GetParameters().Length == 1;
                })
                .Single();

                // este pop es del CreateNewScope al inicio de este bloque
                ParameterExpression singleEvent = this.actualScope.GetFirstParameter();

                MethodInfo setStateMethod = typeof(ExtractedEventData).GetMethod("SetState"); // typeof(InputBase).GetMethod("SetState"); // typeof(ExtractedEventData).GetMethod("SetState");

                Type delegateTypeForSetTimeout = typeof(Action<>).MakeGenericType(toList.Type.GetGenericArguments()[0]);
                LambdaExpression lambdaSetTimeout = Expression.Lambda(delegateTypeForSetTimeout, Expression.Call(singleEvent, setStateMethod, Expression.Constant(ExtractedEventDataStateEnum.Expired, typeof(ExtractedEventDataStateEnum))), new ParameterExpression[] { singleEvent });

                this.PopScope();

                Expression callToSetTimeoutMethod = this.GenerateEventBlock(actualNode, new ParameterExpression[] { }, Expression.Call(toList, methodForEach, lambdaSetTimeout), new List<Expression>(), null, false, "Start of the enumerable foreach for call SetTimeout method.", "End of the enumerable foreach for call SetTimeout method.", RUNTIME_ERRORS.RE63);

                MethodInfo onNextMethod = this.observer.Type.GetMethod("OnNext");

                Expression body = null;

                if (bool.Parse(actualNode.Properties["HasBody"].ToString()) == true)
                {
                    body = Expression.Block(
                        callToSetTimeoutMethod,
                        Expression.Call(this.observer, onNextMethod, right),
                        this.CreateObservableEmpty(actualNode)
                        );
                }
                else
                {
                    body = Expression.Block(
                        callToSetTimeoutMethod,
                        this.CreateObservableEmpty(actualNode)
                        );
                }

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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(catchMethod, incomingObservable, lambdaOfTheOnNext), new List<Expression>(), result, true, "Start of the observable catch.", "End of the observable catch.", RUNTIME_ERRORS.RE63);

                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE83, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="actualNode">Actual node.</param>
        /// <param name="source1">Left source.</param>
        /// <param name="source2">Right source.</param>
        /// <param name="leftDurationLambda">Left duration</param>
        /// <param name="rightDurationLambda">Right duration</param>
        /// <returns>Result expression.</returns>
        private Expression CreateObservableJoin(PlanNode actualNode, Expression source1, Expression source2, LambdaExpression leftDurationLambda, LambdaExpression rightDurationLambda)
        {
            /************************************** ENUMERABLE JOIN *****************************************/
            LambdaExpression lambdaEnumerableJoin = this.CreateEnumerableJoin(actualNode, source1.Type.GetGenericArguments()[0], source2.Type.GetGenericArguments()[0]);
            /************************************** OBSERVABLE JOIN *****************************************/

            MethodInfo methodObservableJoin = typeof(System.Reactive.Linq.Observable).GetMethods()
                                            .Where(m => m.Name == "Join")
                                            .Single().MakeGenericMethod(source1.Type.GetGenericArguments()[0], source2.Type.GetGenericArguments()[0], leftDurationLambda.ReturnType.GetGenericArguments()[0], rightDurationLambda.ReturnType.GetGenericArguments()[0], lambdaEnumerableJoin.ReturnType);

            ParameterExpression obvJoinResult = Expression.Variable(typeof(IObservable<>).MakeGenericType(lambdaEnumerableJoin.ReturnType), "ObservableJoinResult");
            ParameterExpression paramException2 = Expression.Variable(typeof(Exception), "ObservableJoinException");

            Expression tryCatchExprObvJoin = this.GenerateEventBlock(actualNode, new ParameterExpression[] { obvJoinResult }, Expression.Call(methodObservableJoin, source1, source2, leftDurationLambda, rightDurationLambda, lambdaEnumerableJoin), new List<Expression>(), obvJoinResult, true, "Start of the observable join.", "End of the observable join.", RUNTIME_ERRORS.RE70);

            return tryCatchExprObvJoin;
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="actualNode">Actual node.</param>
        /// <param name="source1Type">Left source type.</param>
        /// <param name="source2Type">Right source type.</param>
        /// <returns>Result expression.</returns>
        private LambdaExpression CreateEnumerableJoin(PlanNode actualNode, Type source1Type, Type source2Type)
        {
            try
            {
                // creates the scope for the join with two parameters
                ParameterExpression source1 = Expression.Parameter(source1Type, "Izquierda");
                ParameterExpression source2 = Expression.Parameter(source2Type, "Derecha");

                // left key
                ParameterExpression leftKeyParam = Expression.Parameter(source1Type.GetGenericArguments()[0]);
                Expression leftKey = leftKeyParam;

                Type delegateTypeLeftKey = typeof(Func<,>).MakeGenericType(leftKeyParam.Type, leftKeyParam.Type);
                LambdaExpression lambdaLeftKey = Expression.Lambda(delegateTypeLeftKey, leftKey, new ParameterExpression[] { leftKeyParam });

                // right key
                ParameterExpression rightKeyParam = Expression.Parameter(source2Type.GetGenericArguments()[0]);
                Expression rightKey = rightKeyParam;

                Type delegateTypeRightKey = typeof(Func<,>).MakeGenericType(rightKeyParam.Type, rightKeyParam.Type);
                LambdaExpression lambdaRightKey = Expression.Lambda(delegateTypeRightKey, rightKey, new ParameterExpression[] { rightKeyParam });

                // enumerable selector
                ParameterExpression enumerableSelectorLeftParam = Expression.Parameter(source1Type.GetGenericArguments()[0]);
                ParameterExpression enumerableSelectorRightParam = Expression.Parameter(source2Type.GetGenericArguments()[0]);
                MethodInfo methodCreate = typeof(Tuple).GetMethods()
                                                .Where(m => m.Name == "Create" && m.GetParameters().Length == 2)
                                                .Single().MakeGenericMethod(enumerableSelectorLeftParam.Type, enumerableSelectorRightParam.Type);

                MethodInfo setStateMethod = typeof(ExtractedEventData).GetMethod("SetState"); // typeof(InputBase).GetMethod("SetState"); // typeof(ExtractedEventData).GetMethod("SetState");
                ParameterExpression paramToReturnInSelector = Expression.Variable(methodCreate.ReturnType, "ValueToReturnInSelector");

                ParameterExpression nowVariable = Expression.Variable(typeof(DateTime), "NowVariable");
                Expression now = Expression.Property(null, typeof(DateTime).GetProperty("Now"));
                Expression getLagIzq = Expression.Assign(this.lagVariables[0], Expression.Subtract(nowVariable, Expression.Property(enumerableSelectorLeftParam, "JoinProcessingTimestamp")));
                Expression getLagDer = Expression.Assign(this.lagVariables[1], Expression.Subtract(nowVariable, Expression.Property(enumerableSelectorRightParam, "JoinProcessingTimestamp")));

                Expression enumerableSelector = Expression.Block(
                    new[] { paramToReturnInSelector, nowVariable },
                    Expression.Assign(paramToReturnInSelector, Expression.Constant(null, methodCreate.ReturnType)),
                    Expression.Assign(nowVariable, now),
                    getLagIzq,
                    getLagDer,
                    Expression.IfThen(
                        Expression.Call(enumerableSelectorLeftParam, setStateMethod, Expression.Constant(ExtractedEventDataStateEnum.Matched, typeof(ExtractedEventDataStateEnum))),
                        Expression.IfThen(
                            Expression.Call(enumerableSelectorRightParam, setStateMethod, Expression.Constant(ExtractedEventDataStateEnum.Matched, typeof(ExtractedEventDataStateEnum))),
                            Expression.Assign(paramToReturnInSelector, Expression.Call(methodCreate, enumerableSelectorLeftParam, enumerableSelectorRightParam))
                        )
                    ),
                    paramToReturnInSelector
                    );

                Type delegateTypeEnumerableSelector = typeof(Func<,,>).MakeGenericType(enumerableSelectorLeftParam.Type, enumerableSelectorRightParam.Type, methodCreate.ReturnType);
                LambdaExpression lambdaEnumerableSelector = Expression.Lambda(delegateTypeEnumerableSelector, enumerableSelector, new ParameterExpression[] { enumerableSelectorLeftParam, enumerableSelectorRightParam });

                // enumerable join
                MethodInfo methodEnumerableJoin = typeof(System.Linq.Enumerable).GetMethods()
                                                .Where(m => m.Name == "Join" && m.GetParameters().Length == 5)
                                                .Single().MakeGenericMethod(enumerableSelectorLeftParam.Type, enumerableSelectorRightParam.Type, typeof(ExtractedEventData) /* typeof(InputBase) /*typeof(ExtractedEventData)*/, methodCreate.ReturnType);

                ParameterExpression enumerableJoinResult = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(methodCreate.ReturnType), "EnumerableJoinSelectorResult");
                ParameterExpression paramExp = Expression.Variable(typeof(Exception), "EnumerableJoinException");

                Expression tryCatchExprEnumJoin = this.GenerateEventBlock(actualNode, new ParameterExpression[] { enumerableJoinResult }, Expression.Call(methodEnumerableJoin, source1, source2, lambdaLeftKey, lambdaRightKey, lambdaEnumerableSelector), new List<Expression>(), enumerableJoinResult, true, "Start of the select for enumerable join.", "End of the select for enumerable join.", RUNTIME_ERRORS.RE69);

                this.CreateNewScope(actualNode, tryCatchExprEnumJoin);
                ParameterExpression projectionResult = this.actualScope.GetParameterByIndex(0);
                Expression whereBlock = Expression.Block(
                    Expression.NotEqual(
                        projectionResult,
                        Expression.Constant(null, projectionResult.Type)
                        )
                    );

                Expression tryCatchExprJoin = this.CreateEnumerableWhere(actualNode, tryCatchExprEnumJoin, whereBlock);
                Expression tryCatchExprToObservable = this.EnumerableToObservable(actualNode, tryCatchExprJoin);

                ParameterExpression nowVariableJoinProcessing = Expression.Variable(typeof(DateTime), "NowVariable");
                Expression nowJoinProcessing = Expression.Property(null, typeof(DateTime).GetProperty("Now"));

                /*** start left set timestamp before join processing ***/

                Expression leftToList = this.EnumerableToList(actualNode, source1);

                // new scope para el foreach
                this.CreateNewScope(actualNode, leftToList);

                // obtenemos el metodo foreach
                MethodInfo leftMethodForEach = typeof(System.Collections.Generic.List<>).MakeGenericType(leftToList.Type.GetGenericArguments()[0]).GetMethods().Where(m =>
                {
                    return m.Name == "ForEach" && m.GetParameters().Length == 1;
                })
                .Single();

                ParameterExpression leftSingleEvent = this.actualScope.GetFirstParameter();
                LambdaExpression leftLambdaSetDateTimeLeftJoinProcessing = Expression.Lambda(typeof(Action<>).MakeGenericType(leftToList.Type.GetGenericArguments()[0]), Expression.Assign(Expression.Property(leftSingleEvent, "JoinProcessingTimestamp"), nowVariableJoinProcessing), new ParameterExpression[] { leftSingleEvent });

                // este pop es del CreateNewScope al inicio de este bloque
                this.PopScope();
                Expression leftCallSetDateTimeLeftJoinProcessing = this.GenerateEventBlock(actualNode, new ParameterExpression[] { }, Expression.Call(leftToList, leftMethodForEach, leftLambdaSetDateTimeLeftJoinProcessing), new List<Expression>(), null, false, "Start of the enumerable foreach for set left processing timespan.", "End of the enumerable foreach for set left processing timespan.", RUNTIME_ERRORS.RE83);

                /*** end left set timestamp before join processing ***/

                /*** start right set timestamp before join processing ***/

                Expression rightToList = this.EnumerableToList(actualNode, source2);

                // new scope para el foreach
                this.CreateNewScope(actualNode, rightToList);

                // obtenemos el metodo foreach
                MethodInfo rightMethodForEach = typeof(System.Collections.Generic.List<>).MakeGenericType(rightToList.Type.GetGenericArguments()[0]).GetMethods().Where(m =>
                {
                    return m.Name == "ForEach" && m.GetParameters().Length == 1;
                })
                .Single();

                ParameterExpression rightSingleEvent = this.actualScope.GetFirstParameter();
                LambdaExpression rightLambdaSetDateTimeLeftJoinProcessing = Expression.Lambda(typeof(Action<>).MakeGenericType(rightToList.Type.GetGenericArguments()[0]), Expression.Assign(Expression.Property(rightSingleEvent, "JoinProcessingTimestamp"), nowVariableJoinProcessing), new ParameterExpression[] { rightSingleEvent });

                // este pop es del CreateNewScope al inicio de este bloque
                this.PopScope();
                Expression rightCallSetDateTimeLeftJoinProcessing = this.GenerateEventBlock(actualNode, new ParameterExpression[] { }, Expression.Call(rightToList, rightMethodForEach, rightLambdaSetDateTimeLeftJoinProcessing), new List<Expression>(), null, false, "Start of the enumerable foreach for set right processing timespan.", "End of the enumerable foreach for set right processing timespan.", RUNTIME_ERRORS.RE83);

                /*** end right set timestamp before join processing ***/

                Expression blockJoinSelector = Expression.Block(
                    new[] { nowVariableJoinProcessing },
                    Expression.Assign(nowVariableJoinProcessing, nowJoinProcessing),
                    leftCallSetDateTimeLeftJoinProcessing,
                    rightCallSetDateTimeLeftJoinProcessing,
                    tryCatchExprToObservable
                    );

                LambdaExpression lambdaEnumerableJoin = Expression.Lambda(typeof(Func<,,>).MakeGenericType(source1Type, source2Type, blockJoinSelector.Type), blockJoinSelector, new ParameterExpression[] { source1, source2 });

                return lambdaEnumerableJoin;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE63, actualNode.NodeText), e);
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
                this.OnNode = actualNode.Children[1];

                // get the sources of the join node
                this.IsSecondSource = false;
                Expression source1 = this.GenerateExpressionTree(actualNode.Children[0].Children[0]);

                this.IsSecondSource = true;
                Expression source2 = this.GenerateExpressionTree(actualNode.Children[0].Children[1]);

                this.IsSecondSource = false;

                // se se inicializa el observer
                this.CreateObserver(typeof(IObservable<>).MakeGenericType(typeof(Tuple<,>).MakeGenericType(source1.Type.GetGenericArguments()[0].GetGenericArguments()[0], source2.Type.GetGenericArguments()[0].GetGenericArguments()[0])));

                // creates the scope for the join with two parameters
                ScopeParameter[] newScopeParameters = new ScopeParameter[] { new ScopeParameter(0, source1.Type.GetGenericArguments()[0]), new ScopeParameter(1, source2.Type.GetGenericArguments()[0]) };
                actualNode.Children[0].Properties["ScopeParameters"] = newScopeParameters;
                this.CreateNewScope(actualNode.Children[0], null);

                // get the durations of the observable join
                InternalPlanNodes ipn = new InternalPlanNodes();
                Expression leftDuration = null;

                PlanNode eventLifeTimeNode = actualNode.FindNode(PlanNodeTypeEnum.EventLifeTime).FirstOrDefault();
                PlanNode timeoutConstant = actualNode.FindNode(PlanNodeTypeEnum.ObservableTimeout).First().Children.First();

                if (actualNode.NodeType.Equals(PlanNodeTypeEnum.LeftJoin) || actualNode.NodeType.Equals(PlanNodeTypeEnum.CrossJoin))
                {
                    leftDuration = this.GenerateExpressionTree(ipn.DurationWithSendingResults(timeoutConstant, 0, PlanNodeTypeEnum.JoinLeftDuration));
                }
                else if (actualNode.NodeType.Equals(PlanNodeTypeEnum.RightJoin) || actualNode.NodeType.Equals(PlanNodeTypeEnum.InnerJoin))
                {
                    leftDuration = this.GenerateExpressionTree(ipn.DurationWithoutSendingResults(timeoutConstant, PlanNodeTypeEnum.JoinLeftDuration));
                }
                else
                {
                    throw new CompilationException(string.Empty);
                }

                Type delegateTypeLeftDuration = typeof(Func<,>).MakeGenericType(newScopeParameters[0].Type, typeof(IObservable<>).MakeGenericType(typeof(Unit)));
                LambdaExpression lambdaLeftDuration = Expression.Lambda(delegateTypeLeftDuration, leftDuration, new ParameterExpression[] { this.actualScope.GetParameterByIndex(0) });

                Expression rightDuration = null;
                ipn = new InternalPlanNodes();
                if (actualNode.NodeType.Equals(PlanNodeTypeEnum.LeftJoin) || actualNode.NodeType.Equals(PlanNodeTypeEnum.InnerJoin))
                {
                    rightDuration = this.GenerateExpressionTree(ipn.DurationWithoutSendingResults(timeoutConstant, PlanNodeTypeEnum.JoinRightDuration));
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

                Expression r = this.CreateObservableJoin(actualNode, source1, source2, lambdaLeftDuration, lambdaRightDuration);

                return r;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE73, actualNode.NodeText), e);
            }
        }

        #endregion Join

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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodSelect, incomingObservable, expSelect), new List<Expression>(), result, true, "Start of the observable select for result.", "End of the observable select for result.", RUNTIME_ERRORS.RE77);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE20, actualNode.NodeText), e);
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

                /* aqui se debe obtener el tipo de entrada del objeto */
                Common.CommandObject sourceObject = (Common.CommandObject)actualNode.Properties["Source"];
                ISourceTypeFactory sourceTypeFactory = this.config.Kernel.Get<ISourceTypeFactory>();
                currentParameter = Expression.Parameter(typeof(IObservable<>).MakeGenericType(sourceTypeFactory.GetSourceType(sourceObject)), parameterName);

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
                if (actualNode.Properties.ContainsKey("ParameterType") && this.isInConditionOn)
                {
                    // a esta condición se entra cuando creando el código para la condición 'on' de la consulta
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
                else if (actualNode.Properties.ContainsKey("SourceName"))
                {
                    KeyValuePair<int, string> sourceParam = this.sources.FirstOrDefault(x => x.Value == actualNode.Properties["SourceName"].ToString());

                    if (sourceParam.Equals(default(KeyValuePair<int, string>)))
                    {
                        throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE77(actualNode.Properties["SourceName"].ToString()), actualNode.NodeText));
                    }

                    if (this.IsSecondSource)
                    {
                        // si es la segunda fuente el parámetro se encuentra en la primera posicion
                        if (sourceParam.Key == 1)
                        {
                            return this.actualScope.GetParameterByIndex(0);
                        }
                        else
                        {
                            throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE77(actualNode.Properties["SourceName"].ToString()), actualNode.NodeText));
                        }
                    }
                    else
                    {
                        return this.actualScope.GetParameterByIndex(sourceParam.Key);
                    }
                }
                else
                {
                    return this.actualScope.GetParameterByIndex(0);
                }
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE2, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Add a new scope variable to the scope variables stack.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="leftObservable">Left observable expression.</param>
        public void CreateNewScope(PlanNode actualNode, Expression leftObservable)
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
                    string paramName = string.Format("NewScopeParameter_{0}_{1}", this.actualScope.Level, sp.Position);
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
                string paramName = string.Format("NewScopeParameter_{0}_{1}", this.actualScope.Level, 0);

                // si tiene parametros genéricos, creo una nueva variable global para el nuevo ambiente
                // de lo contrario se utilizará la que se tiene actualmente al tope de la pila
                if (incomingObservable.Type.IsGenericType)
                {
                    Type gt1 = incomingObservable.Type.GetGenericTypeDefinition();
                    if (gt1.Equals(typeof(IGroupedObservable<,>)) || gt1.Equals(typeof(IGrouping<,>)))
                    {
                        this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type.GetGenericArguments()[1]));
                    }
                    else
                    {
                        this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type.GetGenericArguments()[0]));
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
                    this.actualScope.AddParameter(Expression.Parameter(incomingObservable.Type));
                }
            }
        }

        /// <summary>
        /// Removes the actual scope and makes the parent scope the actual scope
        /// </summary>
        public void PopScope()
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
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE3, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(typeof(string), "resultFunctionLeft");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression cadenaResultante = Expression.Variable(incomingObservable.Type, "CadenaResultante");

            int cutNumber = int.Parse(((PlanNode)actualNode.Properties["Number"]).Properties["Value"].ToString());
            Expression numberExp = Expression.Constant(cutNumber, typeof(int));

            try
            {
                MethodInfo method = typeof(string).GetMethods().Where(m => m.Name == "Substring" && m.GetParameters().Length == 2).Single();

                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(cadenaResultante, Expression.Constant(null, cadenaResultante.Type)),
                                            Expression.Assign(param, Expression.Default(typeof(string))),
                                            Expression.Assign(param, Expression.Call(cadenaResultante, method, Expression.Constant(0), numberExp))
                                        );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(cadenaResultante, incomingObservable));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { cadenaResultante, param }, mainAction, actionsBeforeMainAction, param, false, "Start of the 'left' function", "End of the 'left' function", RUNTIME_ERRORS.RE1);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE4, actualNode.NodeText), e);
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
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE5, actualNode.NodeText));
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

                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(cadenaResultante, Expression.Constant(null, cadenaResultante.Type)),
                                            Expression.Assign(param, Expression.Default(typeof(string))),
                                            Expression.Assign(param, Expression.Call(cadenaResultante, method, substringExp))
                                        );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(cadenaResultante, incomingObservable));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { cadenaResultante, param }, mainAction, actionsBeforeMainAction, param, false, "Start of the 'right' function", "End of the 'right' function", RUNTIME_ERRORS.RE10);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE6, actualNode.NodeText), e);
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
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE7, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(typeof(string), "resultFunctionLeft");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression cadenaResultante = Expression.Variable(incomingObservable.Type, "CadenaResultante");

            try
            {
                MethodInfo method = typeof(string).GetMethods().Where(m => m.Name == "ToUpper" && m.GetParameters().Length == 0).Single();

                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(cadenaResultante, Expression.Constant(null, cadenaResultante.Type)),
                                            Expression.Assign(param, Expression.Default(typeof(string))),
                                            Expression.Assign(param, Expression.Call(cadenaResultante, method))
                                        );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(cadenaResultante, incomingObservable));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { cadenaResultante, param }, mainAction, actionsBeforeMainAction, param, false, "Start of the 'upper' function", "End of the 'upper' function", RUNTIME_ERRORS.RE11);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE8, actualNode.NodeText), e);
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
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE9, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(typeof(string), "resultFunctionLeft");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression cadenaResultante = Expression.Variable(incomingObservable.Type, "CadenaResultante");

            try
            {
                MethodInfo method = typeof(string).GetMethods().Where(m => m.Name == "ToLower" && m.GetParameters().Length == 0).Single();

                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(cadenaResultante, Expression.Constant(null, cadenaResultante.Type)),
                                            Expression.Assign(param, Expression.Default(typeof(string))),
                                            Expression.Assign(param, Expression.Call(cadenaResultante, method))
                                        );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(cadenaResultante, incomingObservable));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { cadenaResultante, param }, mainAction, actionsBeforeMainAction, param, false, "Start of the 'lower' function", "End of the 'lower' function", RUNTIME_ERRORS.RE12);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE10, actualNode.NodeText), e);
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

            ParameterExpression param = Expression.Variable(this.ConvertToNullable(tipo), "variable");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression auxL = Expression.Parameter(this.ConvertToNullable(leftNode.Type), "AuxParameterDateFunction");

            try
            {
                // se evalua las propiedades que terminan con 's' y sin 's' ya que pueden venir dos tipos de valores timespan y datetime, 
                // por ejemplo la funcion hour('<>'), si el valor es de tipo datetime tengo que obtener la propiedad Hour,
                // pero si el valor de tipo timespman tengo que obtener la propiedad Hours.
                PropertyInfo p = leftNode.Type.GetProperties().Where(x => x.Name.Equals(propiedad) || x.Name.Equals(propiedad + "s")).Single();

                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(auxL, Expression.Constant(null, auxL.Type)),
                                            Expression.Assign(param, Expression.Default(param.Type)),
                                            Expression.Assign(param, this.StandardizeType(Expression.Call(Expression.Property(auxL, "Value"), p.GetMethod), p.GetMethod.ReturnType))
                                        );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(auxL, this.StandardizeType(leftNode, leftNode.Type)));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { param, auxL }, mainAction, actionsBeforeMainAction, param, false, "Start of the get property of a date type operation: " + propiedad, "End of the get property of a date type operation: " + propiedad, RUNTIME_ERRORS.RE49);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE48(propiedad), actualNode.NodeText), e);
            }
        }

        #endregion Timespan or date and time functions 

        #region Math functions

        /// <summary>
        /// Create a expression for Math Absolute function.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Expression to convert to absolute.</param>
        /// <returns>Absolute value of the expression.</returns>
        private Expression CreateMathFunctionWithOneParameter(PlanNode actualNode, Expression incomingObservable)
        {
            if (!incomingObservable.Type.IsNumericType())
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE71, actualNode.NodeText));
            }

            ParameterExpression param = Expression.Variable(incomingObservable.Type, "resultFunctionAbs");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));
            ParameterExpression valorResultante = Expression.Variable(incomingObservable.Type, "ValorResultante");

            try
            {
                MethodInfo method = typeof(Math).GetMethods().Where(m => m.Name == actualNode.Properties["Function"].ToString() && m.GetParameters()[0].ParameterType.Equals(incomingObservable.Type)).Single();

                Expression mainAction = Expression.IfThenElse(
                            Expression.Equal(valorResultante, Expression.Constant(null, valorResultante.Type)),
                                Expression.Assign(param, Expression.Default(incomingObservable.Type)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'Abs' function"))),
                                        Expression.Assign(param, Expression.Call(method, valorResultante)),
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'Abs' function"))),
                                        Expression.Empty()
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "Message")),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE64, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                    )
                                )
                            );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(valorResultante, incomingObservable));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { valorResultante, param }, mainAction, actionsBeforeMainAction, param, false, "Start of the 'Abs' function", "End of the 'Abs' function", RUNTIME_ERRORS.RE64);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE10, actualNode.NodeText), e);
            }
        }

        #endregion Math functions

        #region Other functions

        /// <summary>
        /// Generates the 'IsNull' function.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="checkExpression">Check expression.</param>
        /// <param name="valueToReplace">Value to replace.</param>
        /// <returns>Left or right value of the function.</returns>
        private Expression CreateIsNullFunction(PlanNode actualNode, Expression checkExpression, Expression valueToReplace)
        {
            Type returnValueType = null;
            Expression leftAux = this.StandardizeType(checkExpression, checkExpression.Type);
            Expression rightAux = this.StandardizeType(valueToReplace, valueToReplace.Type);
            Expression auxCheckExpression = leftAux;
            Expression auxValueToReplace = rightAux;

            if (leftAux.Type.Equals(rightAux.Type))
            {
                returnValueType = leftAux.Type;
            }
            else
            {
                if (leftAux.Type.Equals(typeof(object)) && !rightAux.Type.Equals(typeof(object)))
                {
                    auxCheckExpression = Expression.Convert(leftAux, rightAux.Type);
                    returnValueType = auxCheckExpression.Type;
                }
                else if (!leftAux.Type.Equals(typeof(object)) && rightAux.Type.Equals(typeof(object)))
                {
                    auxValueToReplace = Expression.Convert(rightAux, leftAux.Type);
                    returnValueType = auxValueToReplace.Type;
                }
                else
                {
                    throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE78(checkExpression.Type, valueToReplace.Type), actualNode.NodeText));
                }
            }

            ParameterExpression result = Expression.Variable(this.ConvertToNullable(returnValueType), "resultFunctionAbs");
            ParameterExpression isNullFunctionLeftParameterAux = Expression.Variable(auxCheckExpression.Type, "isNullFunctionLeftParameterAux");
            ParameterExpression isNullFunctionRightParameterAux = Expression.Variable(auxValueToReplace.Type, "isNullFunctionRightParameterAux");

            try
            {
                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(this.StandardizeType(isNullFunctionLeftParameterAux, isNullFunctionLeftParameterAux.Type), Expression.Constant(null, this.ConvertToNullable(isNullFunctionLeftParameterAux.Type))),
                                            Expression.Assign(result, this.StandardizeType(isNullFunctionRightParameterAux, isNullFunctionRightParameterAux.Type)),
                                            Expression.Assign(result, this.StandardizeType(isNullFunctionLeftParameterAux, isNullFunctionLeftParameterAux.Type))
                                        );
                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(isNullFunctionLeftParameterAux, auxCheckExpression));
                actionsBeforeMainAction.Add(Expression.Assign(isNullFunctionRightParameterAux, auxValueToReplace));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result, isNullFunctionLeftParameterAux, isNullFunctionRightParameterAux }, mainAction, actionsBeforeMainAction, result, false, "Start of the 'isnull' function", "End of the 'isnull' function", RUNTIME_ERRORS.RE81);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE79, actualNode.NodeText), e);
            }
        }

        #endregion Other functions

        #endregion Functions

        #region Observable extensions

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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(publishMethod, incomingObservable), new List<Expression>(), result, true, "Start of the observable publish.", "End of the observable publish.", RUNTIME_ERRORS.RE71);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE72, actualNode.NodeText), e);
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

                string sourceName = string.Empty;
                if (this.IsSecondSource)
                {
                    sourceName = "---> RIGHTSOURCE ";
                }
                else
                {
                    sourceName = "---> LEFTSOURCE ";
                }

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(refCountMethod, incomingObservable), new List<Expression>(), result, true, string.Format("{0}. Start of the observable RefCount", sourceName), string.Format("{0}. End of the observable RefCount", sourceName), RUNTIME_ERRORS.RE72);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE63, actualNode.NodeText), e);
            }
        }

        #endregion Publish and RefCount

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

                LambdaExpression lambdaOfTheSubscribe = Expression.Lambda(delegateType, incomingObservable, this.observer);

                MethodInfo methodCreate = typeof(System.Reactive.Linq.Observable).GetMethods()
                                            .Where(x => x.Name == "Create" && x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.ToString().Equals("System.Func`2[System.IObserver`1[TResult],System.IDisposable]"))
                                            .Single().MakeGenericMethod(this.observer.Type.GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(methodCreate, lambdaOfTheSubscribe), new List<Expression>(), result, true, "Start of the observable create.", "End of the observable create.", RUNTIME_ERRORS.RE73);

                return resultExpression; // this.CreateObserveOn(actualNode, tryCatchExpr);
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE81, actualNode.NodeText), e);
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
                    // this.observer = Expression.Parameter(this.context.ObserverType.GetObserverType().MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ObserverDePrueba");
                    this.CreateObserver(incomingObservable.Type.GetGenericArguments()[0]);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(subscribeMethod, incomingObservable, lambdaOfTheOnNext, lambdaOfTheOnCompleted), new List<Expression>(), result, true, "Start of subscribe.", "End of subscribe.", RUNTIME_ERRORS.RE74);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE82, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates the ObserverOn expression.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Incoming expression.</param>
        /// <returns>ObserveOn expression.</returns>
        private Expression CreateObserveOn(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ResultObserveOn");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo observeOnMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                .Where(m => { return m.Name == "ObserveOn" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler)); })
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(observeOnMethod, incomingObservable, this.schedulerExpression), new List<Expression>(), result, true, "Start of ObserveOn.", "End of ObserveOn.", RUNTIME_ERRORS.RE75);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE67, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates the SubscribeOn expression.
        /// </summary>
        /// <param name="actualNode">Actual plan node.</param>
        /// <param name="incomingObservable">Incoming expression.</param>
        /// <returns>SubscribeOn expression.</returns>
        private Expression CreateSubscribeOn(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ResultSubscribeOn");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                MethodInfo subscribeOnMethod = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                .Where(m => { return m.Name == "SubscribeOn" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler)); })
                                                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(subscribeOnMethod, incomingObservable, this.schedulerExpression), new List<Expression>(), result, true, "Start of SubscribeOn.", "End of Subscribe.", RUNTIME_ERRORS.RE76);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE67, actualNode.NodeText), e);
            }
        }

        #endregion Observable Create and Subscription

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
            /*
             * public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, int count);
             * public static IObservable<IList<TSource>> Buffer<TSource>(this IObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler);     
             * 
             * Crea:
             * incomingObservable.Buffer(bufferTimeOrSize)
             * 
             * Ej.        
             * incomingObservable.Buffer(<numero>)
             * incomingObservable.Buffer(<timeSpan>, <scheduler>)
             */

            ParameterExpression result = Expression.Variable(typeof(IObservable<IList<I>>), "ResultObservableBuffer");
            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            try
            {
                ConstructionValidator.Validate(actualNode.NodeType, actualNode, incomingObservable, bufferTimeOrSize);

                MethodInfo methodBuffer = null;
                if (bufferTimeOrSize.Type.IsGenericType)
                {
                    methodBuffer = typeof(System.Reactive.Linq.Observable)
                        .GetMethods()
                        .Where(m => m.Name == "Buffer" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.IsGenericType && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition().Equals(typeof(Func<>)))
                        .Single()
                        .MakeGenericMethod(typeof(I), bufferTimeOrSize.Type.GetGenericArguments()[0].GetGenericArguments()[0]);
                }
                else
                {
                    methodBuffer = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
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
                }

                Expression function = Expression.Throw(Expression.New(typeof(Exception).GetConstructor(new Type[] { typeof(string), typeof(RuntimeException) }), Expression.Constant(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, "Invalid 'apply window of'.", actualNode.NodeText), typeof(string)), paramException));
                if (bufferTimeOrSize.Type.Equals(typeof(TimeSpan)))
                {
                    function = Expression.Call(methodBuffer, incomingObservable, bufferTimeOrSize, this.schedulerExpression);
                }
                else if (bufferTimeOrSize.Type.Equals(typeof(int)) || (bufferTimeOrSize.Type.IsGenericType && bufferTimeOrSize.Type.GetGenericTypeDefinition() == typeof(Func<>)))
                {
                    function = Expression.Call(methodBuffer, incomingObservable, bufferTimeOrSize);
                }

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, function, new List<Expression>(), result, true, "Start of the 'observable buffer' function.", "End of the 'observable buffer' function.", RUNTIME_ERRORS.RE15);

                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE15, actualNode.NodeText), e);
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
                    return m.Name == "Take" && m.GetParameters().Length == 3 && m.GetParameters()[1].ParameterType.Equals(takeSize.Type) && m.GetParameters()[2].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler));
                })
                .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodTake, incomingObservable, takeSize, this.schedulerExpression), new List<Expression>(), result, true, "Start of the observable 'top' function", "End of the observable 'top' function", RUNTIME_ERRORS.RE16);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE16, actualNode.NodeText), e);
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

                MethodInfo methodBuffer = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "Buffer" && m.GetParameters().Length == 4 && m.GetParameters()[1].ParameterType.Equals(bufferTimeAndSize.Type.GetProperty("TimeSpanValue").PropertyType) && m.GetParameters()[2].ParameterType.Equals(bufferTimeAndSize.Type.GetProperty("IntegerValue").PropertyType) && m.GetParameters()[3].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler));
                })
                .Single().MakeGenericMethod(typeof(I));

                MethodInfo get1 = bufferTimeAndSize.Type.GetProperty("TimeSpanValue").GetMethod;
                MethodInfo get2 = bufferTimeAndSize.Type.GetProperty("IntegerValue").GetMethod;

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodBuffer, incomingObservable, Expression.Call(bufferTimeAndSize, get1), Expression.Call(bufferTimeAndSize, get2), this.schedulerExpression), new List<Expression>(), result, true, "Start of the expression observable buffer with time and size", "End of the expression observable buffer with time and size", RUNTIME_ERRORS.RE18);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE18, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(methodWhere, incomingObservable, selectorLambda), new List<Expression>(), result, true, "Start of the expression observable where", "End of the expression observable where", RUNTIME_ERRORS.RE19);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE19, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodGroupBy, incomingObservable, keySelector, Expression.New(typeof(GroupByKeyComparer<O>))), new List<Expression>(), result, true, "Start of the observable group by", "End of the observable group by", RUNTIME_ERRORS.RE24);

                // pop del ámbito
                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE26, actualNode.NodeText), e);
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

                MethodInfo methodMerge = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "Merge" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.ToString().Equals("System.IObservable`1[System.IObservable`1[TSource]]") && m.GetParameters()[0].ParameterType.Equals(typeof(System.Reactive.Concurrency.IScheduler));
                })
                .Single().MakeGenericMethod(typeof(I).GetGenericArguments()[0].GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodMerge, incomingObservable), new List<Expression>(), result, true, "Start of the observable merge", "End of the observable merge", RUNTIME_ERRORS.RE27);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE28, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodSelectMany, incomingObservable, expSelect), new List<Expression>(), result, true, "Start of the select for observable group by", "End of the select for observable group by", RUNTIME_ERRORS.RE2);

                // pop del ámbito
                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE85, actualNode.NodeText), e);
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

                    MethodInfo methodSelect = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], ((LambdaExpression)expProjection).ReturnType);

                    Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(methodSelect, incomingObservable, expProjection), new List<Expression>(), result, true, "Start of the select for observable buffer con lambda", "End of the select for observable buffer con lambda", RUNTIME_ERRORS.RE21);

                    // pop del ámbito
                    this.PopScope();

                    return resultExpression;
                }
                else
                {
                    Type delegateType = typeof(Func<,>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0], expProjection.Type);
                    LambdaExpression selectorLambda = Expression.Lambda(delegateType, expProjection, new ParameterExpression[] { this.actualScope.GetFirstParameter() });

                    ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(expProjection.Type), "SelectForObservableBufferOrSourceNOLambda");
                    ParameterExpression paramException = Expression.Variable(typeof(Exception));

                    MethodInfo methodSelect = typeof(System.Reactive.Linq.Observable).GetMethods()
                                                    .Where(m => { return m.Name == "Select" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource,TResult]"); })
                                                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0], expProjection.Type);

                    Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(methodSelect, incomingObservable, selectorLambda), new List<Expression>(), result, true, "Start of the select for observable buffer sin lambda", "End of the select for observable buffer sin lambda", RUNTIME_ERRORS.RE22);

                    // pop del ámbito
                    this.PopScope();

                    return resultExpression;
                }
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE22, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Call Observable.Switch extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression switch.</returns>
        private Expression ObservableSwitch(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(incomingObservable.Type.GetGenericArguments()[0], "SwitchParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                // se usa Last() porque ya que aun no ha sido posible diferenciarlos por el tipo de parametro
                MethodInfo methodSwitch = typeof(System.Reactive.Linq.Observable).GetMethods().First(m =>
                {
                    return m.Name == "Switch" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IObservable<>);
                }).MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0].GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodSwitch, incomingObservable), new List<Expression>(), result, true, "Start of the observable switch.", "End of the observable switch.", RUNTIME_ERRORS.RE59);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE64, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Call observable concatenate extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression concatenate.</returns>
        private Expression ObservableConcat(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(incomingObservable.Type.GetGenericArguments()[0], "ConcatParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                // se usa Last() porque ya que aun no ha sido posible diferenciarlos por el tipo de parametro
                MethodInfo methodConcat = typeof(System.Reactive.Linq.Observable).GetMethods().First(m =>
                {
                    return m.Name == "Concat" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType.IsGenericType && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IObservable<>);
                }).MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0].GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodConcat, incomingObservable), new List<Expression>(), result, true, "Start of the observable concat.", "End of the observable concat.", RUNTIME_ERRORS.RE62);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE69, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Call enumerable ToList extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression ToList.</returns>
        private Expression ObservableToEnumerable(PlanNode actualNode, Expression incomingObservable)
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

                ParameterExpression result = Expression.Variable(typeof(IEnumerable<>).MakeGenericType(new Type[] { paramGenericType }), "ToListParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                ParameterExpression objetoAConvertir = Expression.Variable(incomingObservable.Type, "ObjetoAConvertirALista");

                MethodInfo methodToEnumerable = typeof(System.Reactive.Linq.Observable).GetMethods().Where(m =>
                {
                    return m.Name == "ToEnumerable";
                })
                .Single().MakeGenericMethod(paramGenericType);

                Expression mainExpression = Expression.IfThen(
                                                Expression.NotEqual(objetoAConvertir, Expression.Constant(null, objetoAConvertir.Type)),
                                                Expression.Assign(result, Expression.Call(methodToEnumerable, objetoAConvertir))
                                            );

                List<Expression> otherActions = new List<Expression>();
                otherActions.Add(Expression.Assign(objetoAConvertir, incomingObservable));

                // aqui el IfThen que compara si el objeto entrante es nulo no se hace.
                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { objetoAConvertir, result }, mainExpression, otherActions, result, false, "Start of the enumerable ToList.", "End of the enumerable ToList.", RUNTIME_ERRORS.RE54);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE55, actualNode.NodeText), e);
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
                            throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE11, actualNode.NodeText), e);
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
                        throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE11, actualNode.NodeText), e);
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
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'count' function"))),
                                    Expression.Assign(param, Expression.Call(methodCount, incomingObservable)),
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'count' function"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "Message")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE13, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE12, actualNode.NodeText), e);
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
            string functionName = actualNode.Properties["FunctionName"].ToString();

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
                            throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE13, actualNode.NodeText), e);
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
                        throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE13, actualNode.NodeText), e);
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
                
                Type delegateType = typeof(Func<,>).MakeGenericType(incommingTypeForFunction, selector.Type);
                MethodInfo methodSum = typeof(System.Linq.Enumerable).GetMethods().Where(m => m.Name == functionName && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType.ToString().Equals("System.Func`2[TSource," + selector.Type.ToString() + "]")).Single().MakeGenericMethod(incommingTypeForFunction);

                Expression selectorLambda = Expression.Lambda(delegateType, selector, new ParameterExpression[] { this.actualScope.GetFirstParameter() });
                this.PopScope();

                Expression tryCatchExpr =
                    Expression.Block(
                        new[] { param },
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'sum' function"))),
                                    Expression.Assign(param, Expression.Call(methodSum, incomingObservable, selectorLambda)),
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'sum' function"))),
                                    Expression.Empty()
                                    ),
                                Expression.Catch(
                                    paramException,
                                     Expression.Block(
                                        Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Property(paramException, "Message")),
                                        Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE14, actualNode.NodeText), typeof(string)), paramException))
                                    )
                                )
                            ),
                        param
                        );

                return tryCatchExpr;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE14(functionName), actualNode.NodeText), e);
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

                Expression mainExpression = Expression.IfThen(
                                                Expression.NotEqual(objetoAConvertir, Expression.Constant(null, objetoAConvertir.Type)),
                                                Expression.Assign(result, Expression.Call(methodToList, objetoAConvertir))
                                            );

                List<Expression> otherActions = new List<Expression>();
                otherActions.Add(Expression.Assign(objetoAConvertir, incomingObservable));

                // aqui el IfThen que compara si el objeto entrante es nulo no se hace.
                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { objetoAConvertir, result }, mainExpression, otherActions, result, false, "Start of the enumerable ToList.", "End of the enumerable ToList.", RUNTIME_ERRORS.RE54);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE55, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(methodToArray, incomingObservable), new List<Expression>(), result, true, "Start of the enumerable to array.", "End of the enumerable to array.", RUNTIME_ERRORS.RE6);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE62, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Call enumerable ToList extension.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable.</param>
        /// <returns>Result expression ToList.</returns>
        public Expression EnumerableToObservable(PlanNode actualNode, Expression incomingObservable)
        {
            try
            {
                ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(incomingObservable.Type.GetGenericArguments()[0]), "ToObservableParam");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));
                ParameterExpression objetoAConvertir = Expression.Variable(incomingObservable.Type, "ObjetoAConvertirAObservable");

                MethodInfo methodToObservable = typeof(System.Reactive.Linq.Observable)
                    .GetMethods()
                    .Where(m => m.Name == "ToObservable" && m.GetParameters().Length == 2)
                    .Single().MakeGenericMethod(incomingObservable.Type.GetGenericArguments()[0]);

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { result }, Expression.Call(methodToObservable, incomingObservable, this.schedulerExpression), new List<Expression>(), result, true, "Start of the enumerable to observable", "End of the enumerable to observable", RUNTIME_ERRORS.RE61);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE62, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodGroupBy, incomingObservable, takeSize), new List<Expression>(), result, true, "Start of the enumerable 'enumerable take' function.", "End of the enumerable 'enumerable take' function.", RUNTIME_ERRORS.RE17);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE17, actualNode.NodeText), e);
            }
        }

        /// <summary>
        /// Creates a new Observable.Where expression.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="filter">Filter expression.</param>
        /// <returns>Observable where expression.</returns>
        public Expression CreateEnumerableWhere(PlanNode actualNode, Expression incomingObservable, Expression filter)
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodWhere, incomingObservable, selectorLambda), new List<Expression>(), result, true, "Start of the expression observable where", "End of the expression observable where", RUNTIME_ERRORS.RE79);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE59, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodGroupBy, incomingObservable, keySelector, Expression.New(typeof(GroupByKeyComparer<>).MakeGenericType(((LambdaExpression)keySelector).ReturnType))), new List<Expression>(), result, true, "Start of the enumerable group by", "End of the enumerable group by", RUNTIME_ERRORS.RE25);

                // pop del ámbito
                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE27, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodGroupBy, incomingObservable, keySelector, Expression.New(typeof(OrderByKeyComparer<>).MakeGenericType(((LambdaExpression)keySelector).ReturnType))), new List<Expression>(), result, true, "Start of the enumerable order by", "End of the enumerable order by", RUNTIME_ERRORS.RE26);

                // pop del ámbito
                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE54, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodSelect, incomingObservable, expSelect), new List<Expression>(), result, true, "Start of the select for enumerable group by", "End of the select for enumerable group by", RUNTIME_ERRORS.RE20);

                // pop del ámbito
                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE21, actualNode.NodeText), e);
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

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, Expression.Call(methodSelectMany, incomingObservable, expProjection), new List<Expression>(), result, true, "Start the select for enumerable", "End the select for enumerable", RUNTIME_ERRORS.RE23);

                // pop del ámbito
                this.PopScope();

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE23, actualNode.NodeText), e);
            }
        }

        #endregion Enumerable extensions

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
            if (this.config.PrintLog)
            {
                expressionList.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the projection")));
            }

            Dictionary<string, Tuple<ConstantExpression, Expression>> keyValueList = new Dictionary<string, Tuple<ConstantExpression, Expression>>();
            List<FieldNode> listOfFields = new List<FieldNode>();

            // obtengo la llave y el valor de cada columna, en forma de expresiones
            foreach (var plan in plans.Children)
            {
                ConstantExpression key = (ConstantExpression)this.GenerateExpressionTree(plan.Children[0]);
                Expression value = this.GenerateExpressionTree(plan.Children[1]);
                if (keyValueList.ContainsKey(key.Value.ToString()))
                {
                    throw new CompilationException(Language.Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE24, string.Empty));
                }
                else
                {
                    keyValueList.Add(key.Value.ToString(), new Tuple<ConstantExpression, Expression>(key, value));
                    int incidencias = 0;
                    if (plan.Children[1].Properties.ContainsKey("IncidenciasEnOn"))
                    {
                        incidencias = int.Parse(plan.Children[1].Properties["IncidenciasEnOn"].ToString());
                    }

                    listOfFields.Add(new FieldNode(key.Value.ToString(), this.ConvertToNullable(value.Type), incidencias));
                }
            }

            // establezco el valor cada propiedad del tipo creado al valor en expresiones de la columna con el nombre(alias) de propiedad(columna)
            List<Expression> parameterList = new List<Expression>();
            foreach (FieldNode p in listOfFields)
            {
                parameterList.Add(this.StandardizeType(keyValueList[p.FieldName].Item2, p.FieldType));
            }

            // creo el tipo a retornar para la proyección, si es una proyección para el select el tipo debe heredar de EventResult
            Type myType = default(Type);
            ParameterExpression y = null;
            if (((PlanNodeTypeEnum)plans.Properties["ProjectionType"]).Equals(PlanNodeTypeEnum.ObservableSelect))
            {
                // myType = LanguageTypeBuilder.CompileResultType(listOfFields);
                EventResultTypeBuilder ertb = new EventResultTypeBuilder(this.config.AsmBuilder, this.config.QueryName, listOfFields);
                myType = ertb.CreateNewType();
                y = Expression.Variable(myType);
                expressionList.Add(Expression.Assign(y, Expression.New(myType.GetConstructors().Single(), parameterList.ToArray())));
            }
            else if (((PlanNodeTypeEnum)plans.Properties["ProjectionType"]).Equals(PlanNodeTypeEnum.ObservableExtractedEventDataComparer))
            {
                Type leftSourceExtractedEventDataType = (Type)plans.Properties["ParentType"];
                Type rightSourceExtractedEventDataType = (Type)plans.Properties["OtherSourceType"];
                Type delegateTypeOnCondition = null;

                if (!this.IsSecondSource)
                {
                    this.OnNode.Children.RemoveAt(0);
                    ScopeParameter[] newScopeParameters = new ScopeParameter[] { new ScopeParameter(0, leftSourceExtractedEventDataType), new ScopeParameter(1, rightSourceExtractedEventDataType) };
                    this.OnNode.Properties.Add("ScopeParameters", newScopeParameters);

                    delegateTypeOnCondition = typeof(Func<,,>).MakeGenericType(leftSourceExtractedEventDataType, rightSourceExtractedEventDataType, typeof(bool));
                }
                else
                {
                    ScopeParameter[] newScopeParameters = new ScopeParameter[] { new ScopeParameter(0, rightSourceExtractedEventDataType), new ScopeParameter(1, leftSourceExtractedEventDataType) };
                    this.OnNode.Properties["ScopeParameters"] = newScopeParameters;

                    delegateTypeOnCondition = typeof(Func<,,>).MakeGenericType(rightSourceExtractedEventDataType, leftSourceExtractedEventDataType, typeof(bool));
                }

                this.CreateNewScope(this.OnNode, null);

                this.isInConditionOn = true;
                Expression onCondition = this.GenerateExpressionTree(this.OnNode.Children.First());
                this.isInConditionOn = false;

                ParameterExpression resultOnCondition = Expression.Parameter(onCondition.Type, "onConditionResult");

                Expression block = Expression.Block(
                    new[] { resultOnCondition },
                    Expression.Assign(resultOnCondition, onCondition),
                    resultOnCondition);

                LambdaExpression lambdaOnCondition = Expression.Lambda(delegateTypeOnCondition, block, new ParameterExpression[] { this.actualScope.GetParameterByIndex(0), this.actualScope.GetParameterByIndex(1) });

                this.PopScope();

                // myType = LanguageTypeBuilder.CompileExtractedEventDataComparerTypeForJoin(listOfFields, leftSourceExtractedEventDataType, rightSourceExtractedEventDataType, this.IsSecondSource, lambdaOnCondition);
                JoinSideObjectComparerTypeBuilder jsoctb = new JoinSideObjectComparerTypeBuilder(this.config.AsmBuilder, listOfFields, leftSourceExtractedEventDataType, rightSourceExtractedEventDataType, this.IsSecondSource, lambdaOnCondition);
                myType = jsoctb.CreateNewType();

                // this.isSecondSource = true;
                y = Expression.Variable(myType);
                expressionList.Add(Expression.Assign(y, Expression.New(myType.GetConstructors().Single(), parameterList.ToArray())));

                // establezco el valor cada propiedad del tipo creado al valor en expresiones de la columna con el nombre(alias) de propiedad(columna)
                foreach (PropertyInfo p in myType.GetProperties())
                {
                    if (keyValueList.ContainsKey(p.Name))
                    {
                        expressionList.Add(Expression.Call(y, p.GetSetMethod(), this.StandardizeType(keyValueList[p.Name].Item2, keyValueList[p.Name].Item2.Type)));
                    }
                }
            }
            else
            {
                // myType = LanguageTypeBuilder.CompileResultType(listOfFields, bool.Parse(plans.Properties["OverrideGetHashCodeMethod"].ToString()));
                DynamicObjectTypeBuilder dotb = new DynamicObjectTypeBuilder(this.config.AsmBuilder, this.config.QueryName, listOfFields);
                myType = dotb.CreateNewType();
                y = Expression.Variable(myType);
                expressionList.Add(Expression.Assign(y, Expression.New(myType)));

                // establezco el valor cada propiedad del tipo creado al valor en expresiones de la columna con el nombre(alias) de propiedad(columna)
                foreach (PropertyInfo p in myType.GetProperties())
                {
                    if (keyValueList.ContainsKey(p.Name))
                    {
                        expressionList.Add(Expression.Call(y, p.GetSetMethod(), this.StandardizeType(keyValueList[p.Name].Item2, keyValueList[p.Name].Item2.Type)));
                    }
                }
            }

            // se agrega el print log que indica la finalización de la proyección
            if (this.config.PrintLog)
            {
                expressionList.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the projection")));
            }

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
            if (this.config.PrintLog)
            {
                expressionList.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the constants projection")));
            }

            Dictionary<string, Tuple<ConstantExpression, Expression>> keyValueList = new Dictionary<string, Tuple<ConstantExpression, Expression>>();
            List<FieldNode> listOfFields = new List<FieldNode>();

            // obtengo la llave y el valor de cada columna, en forma de expresiones
            foreach (var plan in plans.Children)
            {
                ConstantExpression key = (ConstantExpression)this.GenerateExpressionTree(plan.Children[0]);
                Expression value = this.GenerateExpressionTree(plan.Children[1]);
                if (keyValueList.ContainsKey(key.Value.ToString()))
                {
                    throw new CompilationException(Language.Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE25, string.Empty));
                }
                else
                {
                    keyValueList.Add(key.Value.ToString(), new Tuple<ConstantExpression, Expression>(key, value));
                    listOfFields.Add(new FieldNode(key.Value.ToString(), value.Type, 0));
                }
            }

            // Type myType = LanguageTypeBuilder.CompileResultType(listOfFields, bool.Parse(plans.Properties["OverrideGetHashCodeMethod"].ToString()));
            DynamicObjectTypeBuilder dotb = new DynamicObjectTypeBuilder(this.config.AsmBuilder, this.config.QueryName, listOfFields);
            Type myType = dotb.CreateNewType();

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
            if (this.config.PrintLog)
            {
                expressionList.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the constants projection")));
            }

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
            if (this.config.PrintLog)
            {
                expressionList.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the result projection")));
            }

            QueryResultTypeBuilder qrtb = new QueryResultTypeBuilder(this.config.AsmBuilder, this.config.QueryName, incomingObservable.Type.GetGenericArguments()[0]);
            Type resultType = qrtb.CreateNewType();

            // inicializo el observer con el tipo creado a partir de la proyección
            if (this.observer == null)
            {
                // this.observer = Expression.Parameter(this.context.ObserverType.GetObserverType().MakeGenericType(resultType));
                this.CreateObserver(resultType);
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
                            Expression.Constant(this.config.QueryName),
                            Expression.Property(null, typeof(DateTime).GetProperty("Now")),
                            arrayDeResultados
                        )
                    )
            );

            // se agrega el print log que indica la finalización de la proyección
            if (this.config.PrintLog)
            {
                expressionList.Add(Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the result projection")));
            }

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
            return Expression.Lambda(delegateType, expProjectionObject, new ParameterExpression[] { this.actualScope.GetParameterByIndex(0) });
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

                result = Expression.Variable(typeof(Tuple<,>).MakeGenericType(leftType, rightType), "JoinProjection");

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { result }, valueToReturn, new List<Expression>(), result, true, "Start the join projection", "End the join projection", RUNTIME_ERRORS.RE80);

                Type delegateType = null;
                LambdaExpression lambda = null;
                if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinLeftDuration))
                {
                    delegateType = typeof(Func<,>).MakeGenericType(leftType, result.Type);
                    lambda = Expression.Lambda(delegateType, resultExpression, leftItem);
                }
                else if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinRightDuration))
                {
                    delegateType = typeof(Func<,>).MakeGenericType(rightType, result.Type);
                    lambda = Expression.Lambda(delegateType, resultExpression, rightItem);
                }
                else if (actualNode.Properties["ProjectionType"].Equals(PlanNodeTypeEnum.JoinResultSelector))
                {
                    delegateType = typeof(Func<,,>).MakeGenericType(leftType, rightType, result.Type);
                    lambda = Expression.Lambda(delegateType, resultExpression, leftItem, rightItem);
                }

                return lambda;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE61, actualNode.NodeText), e);
            }
        }

        #endregion Projections

        #region Access to event properties

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
            string position = string.Empty;
            Type tipo = null;
            ParameterExpression auxVar = Expression.Variable(leftNode.Type, "VariableParaAsignarLeftNode");

            MethodCallExpression expGetProperty;

            if (actualNode.Properties.ContainsKey("FromInterface"))
            {
                tipo = leftNode.Type.GetInterface(actualNode.Properties["FromInterface"].ToString());
            }
            else
            {
                tipo = leftNode.Type;
            }

            if (leftNode.Type.IsGenericType && leftNode.Type.GetGenericTypeDefinition().Equals(typeof(Tuple<,>)))
            {
                int paramToTake = this.sources.First(x => x.Value == actualNode.Children[0].Properties["SourceName"].ToString()).Key;

                if (this.sources.ContainsKey(paramToTake))
                {
                    MethodInfo aux1 = null;
                    if (paramToTake == LEFTSOURCE)
                    {
                        getItemNMethod = leftNode.Type.GetMethod("get_Item1");
                        aux1 = getItemNMethod.ReturnType.GetProperty(propiedad).GetGetMethod();

                        tupleItem = Expression.Call(auxVar, getItemNMethod);
                        position = "left";
                    }
                    else if (paramToTake == RIGHTSOURCE)
                    {
                        getItemNMethod = leftNode.Type.GetMethod("get_Item2");
                        aux1 = getItemNMethod.ReturnType.GetProperty(propiedad).GetGetMethod();

                        tupleItem = Expression.Call(auxVar, getItemNMethod);
                        position = "right";
                    }

                    expGetProperty = Expression.Call(tupleItem, aux1);
                }
                else
                {
                    throw new CompilationException("Invalid source.");
                }
                 
            }
            else if(tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(System.Linq.IGrouping<,>))
            {
                MethodInfo aux1 = tipo.GetProperty("Key").GetGetMethod();
                MethodInfo aux2 = tipo.GetProperty("Key").PropertyType.GetProperty(propiedad).GetGetMethod();

                expGetProperty = Expression.Call(Expression.Call(tupleItem, aux1), aux2);
            }
            else
            {
                MethodInfo aux1 = tipo.GetProperty(propiedad).GetGetMethod();
                expGetProperty = Expression.Call(tupleItem, aux1);
            }

            Type tipoDeLaPropiedad = expGetProperty.Type;

            try
            {
                ParameterExpression param = Expression.Variable(this.ConvertToNullable(tipoDeLaPropiedad), "PropertyValue");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(param, Expression.Default(param.Type)));
                actionsBeforeMainAction.Add(Expression.Assign(auxVar, leftNode));

                Expression mainAction = null;
                if (tipo.IsGenericType && tipo.GetGenericTypeDefinition().Equals(typeof(Tuple<,>)))
                {
                    if (this.config.PrintLog)
                    {
                        mainAction =
                            Expression.Block(
                                Expression.IfThen(
                                    Expression.AndAlso(Expression.NotEqual(auxVar, Expression.Constant(null, tipo)), Expression.NotEqual(Expression.Call(auxVar, getItemNMethod), Expression.Constant(null, tupleItem.Type))),
                                    Expression.Assign(param, this.StandardizeType(expGetProperty, expGetProperty.Type))
                                ),
                                Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("Tupla " + position + ": " + propiedad + " Valor: "))),
                                Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Convert(param, typeof(object))))
                                );
                    }
                    else
                    {
                        mainAction =
                            Expression.IfThen(
                                    Expression.AndAlso(Expression.NotEqual(auxVar, Expression.Constant(null, tipo)), Expression.NotEqual(Expression.Call(auxVar, getItemNMethod), Expression.Constant(null, tupleItem.Type))),
                                    Expression.Assign(param, this.StandardizeType(expGetProperty, expGetProperty.Type))
                                );
                    }
                }
                else
                {
                    if (this.config.PrintLog)
                    {
                        mainAction =
                                Expression.Block(
                                    Expression.IfThen(
                                        Expression.NotEqual(auxVar, Expression.Constant(null, tipo)),
                                        Expression.Assign(param, this.StandardizeType(expGetProperty, expGetProperty.Type))
                                    ),
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("The value of the property is: "))),
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Convert(param, typeof(object))))
                                );
                    }
                    else
                    {
                        mainAction =
                                Expression.IfThen(
                                    Expression.NotEqual(auxVar, Expression.Constant(null, tipo)),
                                    Expression.Assign(param, this.StandardizeType(expGetProperty, expGetProperty.Type))
                                );
                    }
                }

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { param, auxVar }, mainAction, actionsBeforeMainAction, param, false, "Start of the get property operation: " + propiedad, "End of the get property operation: " + propiedad, RUNTIME_ERRORS.RE5);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE49(propiedad), actualNode.NodeText), e);
            }
        }

        #endregion Access to event properties

        #region Cast

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="actualNode">actual plan</param>
        /// <param name="leftNode">left child expression</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateCast(PlanNode actualNode, Expression leftNode)
        {
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
            ParameterExpression retorno = Expression.Parameter(this.ConvertToNullable(typeToCast));

            try
            {
                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(valorACastear, leftNode));
                Expression mainAction = null;

                if (leftNode.Type.IsGenericType && leftNode.Type.GetGenericArguments()[0].Equals(typeof(System.Guid)))
                {
                    MethodInfo toStringMethod = typeof(object).GetMethod("ToString");
                    mainAction = Expression.IfThen(
                                                Expression.NotEqual(Expression.Convert(valorACastear, this.ConvertToNullable(leftNode.Type)), Expression.Constant(null)),
                                                Expression.Assign(retorno, Expression.Call(valorACastear, toStringMethod))
                                            );
                }
                else
                {
                    mainAction = Expression.IfThen(
                                                Expression.NotEqual(Expression.Convert(valorACastear, this.ConvertToNullable(leftNode.Type)), Expression.Constant(null)),
                                                Expression.Assign(retorno, Expression.Convert(valorACastear, this.ConvertToNullable(typeToCast)))
                                            );
                }

                Expression resultExpression = this.GenerateEventBlock(actualNode, new ParameterExpression[] { valorACastear, retorno }, mainAction, actionsBeforeMainAction, retorno, false, "Start of the convert operation to type " + typeToCast, "End of the 'Convert' operation to type " + typeToCast, RUNTIME_ERRORS.RE33);

                return resultExpression;
            }
            catch (Exception ex)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE34, actualNode.NodeText), ex);
            }
        }

        #endregion Cast

        #region Constant

        /// <summary>
        /// Create a expression tree
        /// </summary>
        /// <param name="plan">plan node to convert</param>
        /// <returns>expression tree of actual plan</returns>
        private Expression GenerateConstant(PlanNode plan)
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

                Type tipo = (Type)plan.Properties["DataType"];

                if (tipo.Equals(typeof(TimeSpan)) || tipo.Equals(typeof(DateTime)))
                {
                    return Expression.Call(null, tipo.GetMethod("Parse", new Type[] { typeof(string) }), Expression.Constant(plan.Properties["Value"].ToString()));
                }

                return Expression.Constant(plan.Properties["Value"], tipo);
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(plan.Line, plan.Column, COMPILATION_ERRORS.CE86, plan.NodeText), e);
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
                ParameterExpression auxl = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");

                if (cadenaLike.StartsWith("%") && cadenaLike.EndsWith("%"))
                {
                    Expression tryCatchExpr =
                        Expression.Block(
                            new[] { param, auxl },
                            Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)),
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation with two wildcards: "))),
                                        Expression.Assign(param, Expression.Call(auxl, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation with two wildcards")))
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like '%...%', error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE34, actualNode.NodeText), typeof(string)), paramException))
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
                            new[] { param, auxl },
                            Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)),
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation with left wildcard: "))),
                                        Expression.Assign(param, Expression.Call(auxl, typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation with left wildcard")))
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like '%..., error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE35, actualNode.NodeText), typeof(string)), paramException))
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
                            new[] { param, auxl },
                            Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)),
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation with right wildcard: "))),
                                        Expression.Assign(param, Expression.Call(auxl, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation with right wildcard")))
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like '...%, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE36, actualNode.NodeText), typeof(string)), paramException))
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
                            new[] { param, auxl },
                            Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)),
                            Expression.IfThenElse(
                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(this.StandardizeType(rightNode, rightNode.Type), Expression.Constant(null, this.ConvertToNullable(rightNode.Type)))),
                                Expression.Assign(param, Expression.Constant(false)),
                                Expression.TryCatch(
                                    Expression.Block(
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the 'like' operation without wildcards: "))),
                                        Expression.Assign(param, Expression.Call(auxl, typeof(string).GetMethod("Equals", new Type[] { typeof(string) }), Expression.Constant(cadenaAComparar))),
                                        Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the 'like' operation without wildcards")))
                                        ),
                                    Expression.Catch(
                                        paramException,
                                        Expression.Block(
                                                Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("No fue posible realizar la operación like sin comodines, error en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                                Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE37, actualNode.NodeText), typeof(string)), paramException))
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
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE35, actualNode.NodeText), e);
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
                ParameterExpression auxl = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");
                ParameterExpression auxr = Expression.Variable(this.ConvertToNullable(rightNode.Type), "VariableAuxRight");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)));
                actionsBeforeMainAction.Add(Expression.Assign(auxr, this.StandardizeType(rightNode, rightNode.Type)));

                Expression mainExpression = Expression.IfThenElse(
                                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(auxr, Expression.Constant(null, auxr.Type))),
                                                Expression.Assign(param, Expression.Constant(false)),
                                                Expression.Assign(param, Expression.GreaterThanOrEqual(auxl, auxr))
                                            );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, auxl, auxr }, mainExpression, actionsBeforeMainAction, param, false, "Start of the greater than or equal operation '>='.", "End of the greater than or equal operation '>='.", RUNTIME_ERRORS.RE39);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE37, actualNode.NodeText), e);
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
                ParameterExpression auxl = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");
                ParameterExpression auxr = Expression.Variable(this.ConvertToNullable(rightNode.Type), "VariableAuxRight");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)));
                actionsBeforeMainAction.Add(Expression.Assign(auxr, this.StandardizeType(rightNode, rightNode.Type)));

                Expression mainExpression = Expression.IfThenElse(
                                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(auxr, Expression.Constant(null, auxr.Type))),
                                                Expression.Assign(param, Expression.Constant(false)),
                                                Expression.Assign(param, Expression.GreaterThan(auxl, auxr))
                                            );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, auxl, auxr }, mainExpression, actionsBeforeMainAction, param, false, "Start of the greater than operation '>'.", "End of the greater than operation '>'.", RUNTIME_ERRORS.RE4);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE38, actualNode.NodeText), e);
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
                ParameterExpression auxl = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");
                ParameterExpression auxr = Expression.Variable(this.ConvertToNullable(rightNode.Type), "VariableAuxRight");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)));
                actionsBeforeMainAction.Add(Expression.Assign(auxr, this.StandardizeType(rightNode, rightNode.Type)));

                Expression mainExpression = Expression.IfThenElse(
                                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(auxr, Expression.Constant(null, auxr.Type))),
                                                Expression.Assign(param, Expression.Constant(false)),
                                                Expression.Assign(param, Expression.LessThanOrEqual(auxl, auxr))
                                            );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, auxl, auxr }, mainExpression, actionsBeforeMainAction, param, false, "Start of the less than or equal operation '<='.", "End of the less than or equal operation '<='.", RUNTIME_ERRORS.RE40);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE39, actualNode.NodeText), e);
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
                ParameterExpression auxl = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");
                ParameterExpression auxr = Expression.Variable(this.ConvertToNullable(rightNode.Type), "VariableAuxRight");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)));
                actionsBeforeMainAction.Add(Expression.Assign(auxr, this.StandardizeType(rightNode, rightNode.Type)));

                Expression mainExpression = Expression.IfThenElse(
                                                Expression.Or(Expression.Equal(auxl, Expression.Constant(null, auxl.Type)), Expression.Equal(auxr, Expression.Constant(null, auxr.Type))),
                                                Expression.Assign(param, Expression.Constant(false)),
                                                Expression.Assign(param, Expression.LessThan(auxl, auxr))
                                            );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, auxl, auxr }, mainExpression, actionsBeforeMainAction, param, false, "Start of the less than operation '<'.", "End of the less than operation '<'.", RUNTIME_ERRORS.RE41);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE40, actualNode.NodeText), e);
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

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param }, Expression.NotEqual(this.StandardizeType(leftNode, leftNode.Type), this.StandardizeType(rightNode, rightNode.Type)), new List<Expression>(), param, true, "Start of the not equal operation '!='.", "End of the not equal operation '!='.", RUNTIME_ERRORS.RE42);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE41, actualNode.NodeText), e);
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

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param }, Expression.Equal(this.StandardizeType(leftNode, leftNode.Type), this.StandardizeType(rightNode, rightNode.Type)), new List<Expression>(), param, true, "Start of the equal operation '=='.", "End of the equal operation '=='.", RUNTIME_ERRORS.RE43);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE42, actualNode.NodeText), e);
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
            Type tipo = null;

            if (leftNode.Type.Equals(rightNode.Type))
            {
                if ((leftNode.Type.Equals(typeof(DateTime)) || leftNode.Type.Equals(typeof(DateTime?))) && (rightNode.Type.Equals(typeof(DateTime)) || rightNode.Type.Equals(typeof(DateTime?))))
                {
                    tipo = typeof(TimeSpan);
                }
                else
                {
                    tipo = leftNode.Type;
                }
            }
            else
            {
                tipo = Type.GetType(actualNode.Properties["DataType"].ToString());
            }

            ParameterExpression leftAux = Expression.Variable(leftNode.Type, "leftArgumentSubsctractAux");
            ParameterExpression rightAux = Expression.Variable(rightNode.Type, "rightArgumentSubsctractAux");

            ParameterExpression paramException = Expression.Variable(typeof(Exception));

            if (tipo.Equals(typeof(TimeSpan)))
            {
                try
                {
                    ParameterExpression param = Expression.Variable(tipo, "variable");

                    Expression getLeftValue = Expression.Call(leftAux, leftNode.Type.GetMethod("GetValueOrDefault", Type.EmptyTypes));
                    Expression getRightValue = Expression.Call(rightAux, rightNode.Type.GetMethod("GetValueOrDefault", Type.EmptyTypes));

                    List<Expression> actionsBeforeMainAction = new List<Expression>();
                    actionsBeforeMainAction.Add(Expression.Assign(leftAux, leftNode));
                    actionsBeforeMainAction.Add(Expression.Assign(rightAux, rightNode));

                    Expression mainExpression = Expression.IfThen(
                                                    Expression.And(Expression.NotEqual(this.StandardizeType(leftAux, leftAux.Type), Expression.Constant(null, this.ConvertToNullable(leftAux.Type))), Expression.NotEqual(this.StandardizeType(rightAux, rightAux.Type), Expression.Constant(null, this.ConvertToNullable(rightAux.Type)))),
                                                    Expression.Assign(param, Expression.Call(getLeftValue, leftAux.Type.GetMethod("GetValueOrDefault", Type.EmptyTypes).ReturnType.GetMethod("Subtract", new Type[] { rightAux.Type.GetMethod("GetValueOrDefault", Type.EmptyTypes).ReturnType }), getRightValue))
                                                );

                    Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, leftAux, rightAux }, mainExpression, actionsBeforeMainAction, param, false, "Start of the substract operation '-' for timespan.", "End of the substract operation '-' for timespan.", RUNTIME_ERRORS.RE44);

                    return expresionResult;
                }
                catch (Exception e)
                {
                    throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE43, actualNode.NodeText), e);
                }
            }
            else
            {
                try
                {
                    ParameterExpression param = Expression.Variable(tipo, "variable");

                    List<Expression> actionsBeforeMainAction = new List<Expression>();
                    actionsBeforeMainAction.Add(Expression.Assign(leftAux, leftNode));
                    actionsBeforeMainAction.Add(Expression.Assign(rightAux, rightNode));

                    Expression mainExpression = Expression.IfThen(
                                                    Expression.And(Expression.NotEqual(this.StandardizeType(leftAux, leftAux.Type), Expression.Constant(null, this.ConvertToNullable(leftAux.Type))), Expression.NotEqual(this.StandardizeType(rightAux, rightAux.Type), Expression.Constant(null, this.ConvertToNullable(rightAux.Type)))),
                                                    Expression.Assign(param, Expression.Subtract(leftAux, rightAux))
                                                );

                    Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, leftAux, rightAux }, mainExpression, actionsBeforeMainAction, param, false, "Start of the substract operation '-'.", "End of the substract operation '-'.", RUNTIME_ERRORS.RE45);

                    return expresionResult;
                }
                catch (Exception e)
                {
                    throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE44, actualNode.NodeText), e);
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

                Expression mainExpression = Expression.IfThen(
                                                Expression.NotEqual(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))),
                                                Expression.Assign(param, Expression.Negate(leftNode))
                                            );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param }, mainExpression, new List<Expression>(), param, false, "Start of the negate operation '-'.", "End of the negate operation '-'.", RUNTIME_ERRORS.RE46);

                /*Expression expresionResult =
                    Expression.Block(
                        new[] { param },
                        Expression.IfThen(
                            Expression.NotEqual(this.StandardizeType(leftNode, leftNode.Type), Expression.Constant(null, this.ConvertToNullable(leftNode.Type))),
                            Expression.TryCatch(
                                Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Start of the negate operation '-': "))),
                                    Expression.Assign(param, Expression.Negate(leftNode)),
                                    Expression.IfThen(Expression.Constant(this.context.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("End of the negate operation '-': ")))
                                    ),
                                Expression.Catch(
                                    paramException,
                                    Expression.Block(
                                            Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("Error con la expresion aritmetica unaria de negacion '-' en la linea: " + actualNode.Line + " columna: " + actualNode.Column + " con " + actualNode.NodeText)),
                                            Expression.Throw(Expression.New(typeof(RuntimeException).GetConstructor(new Type[] { typeof(string), typeof(Exception) }), Expression.Constant(Language.Resources.SR.RuntimeError(actualNode.Line, actualNode.Column, RUNTIME_ERRORS.RE46, actualNode.NodeText), typeof(string)), paramException))
                                        )
                                )
                            )
                        ),
                        param
                        );*/

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE45, actualNode.NodeText), e);
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
                ParameterExpression auxl = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");
                ParameterExpression auxr = Expression.Variable(this.ConvertToNullable(rightNode.Type), "VariableAuxRight");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)));
                actionsBeforeMainAction.Add(Expression.Assign(auxr, this.StandardizeType(rightNode, rightNode.Type)));

                Expression mainExpression = Expression.IfThenElse(
                                                Expression.And(Expression.TypeEqual(auxl, typeof(bool?)), Expression.TypeEqual(auxr, typeof(bool?))),
                                                Expression.IfThenElse(
                                                    Expression.Equal(auxl, Expression.Constant(null, auxl.Type)),
                                                    Expression.Assign(param, Expression.Constant(false, typeof(bool))),
                                                    Expression.IfThenElse(
                                                        Expression.Equal(auxr, Expression.Constant(null, auxr.Type)),
                                                        Expression.Assign(param, Expression.Constant(false, typeof(bool))),
                                                        Expression.Assign(param, Expression.AndAlso(Expression.Convert(Expression.Property(auxl, "Value"), typeof(bool)), Expression.Convert(Expression.Property(auxr, "Value"), typeof(bool))))
                                                    )
                                                ),
                                                Expression.Assign(param, Expression.Constant(false, typeof(bool)))
                                            );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, auxl, auxr }, mainExpression, actionsBeforeMainAction, param, false, "Start of the and operation 'and'.", "End of the and operation 'and'.", RUNTIME_ERRORS.RE47);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE46, actualNode.NodeText), e);
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
                ParameterExpression auxl = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");
                ParameterExpression auxr = Expression.Variable(this.ConvertToNullable(rightNode.Type), "VariableAuxRight");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(auxl, this.StandardizeType(leftNode, leftNode.Type)));
                actionsBeforeMainAction.Add(Expression.Assign(auxr, this.StandardizeType(rightNode, rightNode.Type)));

                Expression mainExpression = Expression.IfThenElse(
                                                Expression.And(Expression.TypeEqual(auxl, typeof(bool?)), Expression.TypeEqual(auxr, typeof(bool?))),
                                                Expression.IfThenElse(
                                                    Expression.Equal(auxl, Expression.Constant(null, auxl.Type)),
                                                    Expression.Assign(param, Expression.Constant(false, typeof(bool))),
                                                    Expression.IfThenElse(
                                                        Expression.Equal(auxr, Expression.Constant(null, auxr.Type)),
                                                        Expression.Assign(param, Expression.Constant(false, typeof(bool))),
                                                        Expression.Assign(param, Expression.OrElse(Expression.Convert(Expression.Property(auxl, "Value"), typeof(bool)), Expression.Convert(Expression.Property(auxr, "Value"), typeof(bool))))
                                                    )
                                                ),
                                                Expression.Assign(param, Expression.Constant(false, typeof(bool)))
                                            );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { param, auxl, auxr }, mainExpression, actionsBeforeMainAction, param, false, "Start of the and operation 'or'.", "End of the and operation 'or'.", RUNTIME_ERRORS.RE48);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE47, actualNode.NodeText), e);
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

                ParameterExpression aux = Expression.Variable(this.ConvertToNullable(leftNode.Type), "VariableAuxLeft");

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(aux, this.StandardizeType(leftNode, leftNode.Type)));

                Expression mainExpression = Expression.IfThenElse(
                                                            Expression.TypeEqual(aux, typeof(bool?)),
                                                            Expression.IfThenElse(
                                                                Expression.Equal(aux, Expression.Constant(null, aux.Type)),
                                                                Expression.Assign(param, Expression.Constant(false, typeof(bool))),
                                                                Expression.Assign(param, Expression.Not(Expression.Convert(Expression.Property(aux, "Value"), typeof(bool))))
                                                            ),
                                                            Expression.Assign(param, Expression.Constant(false))
                                                        );

                Expression expresionResult = this.GenerateEventBlock(actualNode, new ParameterExpression[] { aux, param }, mainExpression, actionsBeforeMainAction, param, false, "Start of the 'not' operation.", "End of the 'not' operation.", RUNTIME_ERRORS.RE38);

                return expresionResult;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE36, actualNode.NodeText), e);
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
                throw new CompilationException(Language.Resources.SR.CompilationError(string.Empty, string.Empty, COMPILATION_ERRORS.CE50, string.Empty));
            }

            Type tipo = this.groupExpression.Type;
            string propiedad = actualNode.Properties["Value"].ToString();

            try
            {
                ParameterExpression param = Expression.Variable(tipo.GetGenericArguments()[0], "variable");
                ParameterExpression paramException = Expression.Variable(typeof(Exception));

                Expression mainAction = Expression.IfThenElse(
                                                Expression.Equal(Expression.Call(this.groupExpression, this.groupExpression.Type.GetMethod("get_Key")), Expression.Constant(null)),
                                                Expression.Assign(param, Expression.Default(this.groupExpression.Type.GetProperty(propiedad).PropertyType)),
                                                Expression.Assign(param, Expression.Property(this.groupExpression, propiedad))
                                            );

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { param }, mainAction, new List<Expression>(), param, false, "Start of the get group key operation: " + propiedad, "End of the get group key operation: " + propiedad, RUNTIME_ERRORS.RE50);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE51(propiedad), actualNode.NodeText), e);
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

                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(Expression.Call(g, g.Type.GetProperty(propiedad).GetGetMethod()), Expression.Constant(null)),
                                            Expression.Assign(param, Expression.Default(tipo.GetProperty(propiedad).PropertyType)),
                                            Expression.Assign(param, Expression.Property(g, propiedad))
                                        );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(g, leftNode));

                Expression resultExpression = this.GenerateEventBlock(actualNode, new[] { g, param }, mainAction, actionsBeforeMainAction, param, false, "Start of the get group key property operation: " + propiedad, "End of the get group key property operation: " + propiedad, RUNTIME_ERRORS.RE51);

                return resultExpression;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, Language.Resources.COMPILATION_ERRORS.CE52(propiedad), actualNode.NodeText), e);
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

                Expression aux = null;
                if (this.config.PrintLog)
                {
                    aux = Expression.Block(
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), Expression.Constant("You will get the property value of " + actualNode.NodeText))),
                                    Expression.Assign(param, gkp),
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) }), Expression.Constant("The group key property value of " + actualNode.NodeText + " is: "))),
                                    Expression.IfThen(Expression.Constant(this.config.PrintLog), Expression.Call(typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) }), param))
                                    );
                }
                else
                {
                    aux = Expression.Assign(param, gkp);
                }

                Expression mainAction = Expression.IfThenElse(
                                            Expression.Equal(gkp, Expression.Constant(null, gkp.Type)),
                                            Expression.Assign(param, Expression.Default(groupKeyProperty.Type)),
                                            aux
                                        );

                List<Expression> actionsBeforeMainAction = new List<Expression>();
                actionsBeforeMainAction.Add(Expression.Assign(gkp, groupKeyProperty));

                Expression resultAction = this.GenerateEventBlock(actualNode, new[] { gkp, param }, mainAction, actionsBeforeMainAction, param, false, "Start of the get group property value method.", "End of the get group property value method.", RUNTIME_ERRORS.RE52);

                return resultAction;
            }
            catch (Exception e)
            {
                throw new CompilationException(Language.Resources.SR.CompilationError(actualNode.Line, actualNode.Column, COMPILATION_ERRORS.CE53, actualNode.NodeText), e);
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
            if (Nullable.GetUnderlyingType(exp.Type) != null)
            {
                return exp;
            }
            else
            {
                if (type.Equals(typeof(TimeSpan)) || type.Equals(typeof(Guid)))
                {
                    return Expression.Convert(exp, this.ConvertToNullable(type));
                }

                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.DateTime:
                        return Expression.Convert(exp, this.ConvertToNullable(type));
                    case TypeCode.Object:
                    case TypeCode.String:
                        return Expression.Convert(exp, type);
                    default:
                        throw new CompilationException(string.Format("Invalid cast. Can't convert {0} to {1}", exp.Type.Name, type.Name));
                }
            }
        }

        /// <summary>
        /// Creates the observer for the observable create method.
        /// </summary>
        /// <param name="genericType">Generic type of the observer</param>
        public void CreateObserver(Type genericType)
        {
            this.observer = Expression.Parameter(typeof(IObserver<>).MakeGenericType(genericType), "ObserverDePrueba");
        }

        #endregion Helpers
    }
}