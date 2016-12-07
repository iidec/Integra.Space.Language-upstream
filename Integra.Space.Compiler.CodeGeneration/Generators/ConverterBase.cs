//-----------------------------------------------------------------------
// <copyright file="ConverterBase.cs" company="Integra.Space.Language">
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
    using System.Reflection;
    using Integra.Space.Language.Exceptions;
    using Language;

    /// <summary>
    /// Node compiler base.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:ClosingParenthesisMustBeSpacedCorrectly", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1111:ClosingParenthesisMustBeOnLineOfLastParameter", Justification = "Reviewed.")]
    internal class ConverterBase
    {
        /// <summary>
        /// Compiler configuration.
        /// </summary>
        private CodeGeneratorConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConverterBase"/> class.
        /// </summary>
        /// <param name="config">Compiler configuration.</param>
        public ConverterBase(CodeGeneratorConfiguration config)
        {
            this.config = config;
        }
        
        /// <summary>
        /// Executes the transformation of the plan node.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        /// <returns>The result expression of the execution plan node.</returns>
        public CodeGeneratorContext Transform(CodeGeneratorContext context)
        {
            this.PreTransformation(context);
            context.ResultExpression = this.TranformNode(context);
            this.PostTransformation(context);

            return context;
        }

        /// <summary>
        /// Actions before the execution plan node transformation.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        protected virtual void PreTransformation(CodeGeneratorContext context)
        {
        }

        /// <summary>
        /// Actions after the execution plan node transformation.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        protected virtual void PostTransformation(CodeGeneratorContext context)
        {
        }

        /// <summary>
        /// Transform the execution plan node to an expression.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        /// <returns>The result expression of the execution plan node.</returns>
        protected virtual Expression TranformNode(CodeGeneratorContext context)
        {
            throw new NotImplementedException("Implementation for the node transormation is required.");
        }

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
        protected Expression GenerateEventBlock(PlanNode actualNode, IEnumerable<ParameterExpression> parameters, Expression mainAction, List<Expression> actionsBeforeMainAction, ParameterExpression resultParameter, bool assignable, string startMessage, string endMessage, string runtimeError)
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
    }
}
