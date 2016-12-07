//-----------------------------------------------------------------------
// <copyright file="ObservableNeverConverter.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using Language;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reflection;

    /// <summary>
    /// Observable never converter class.
    /// </summary>
    internal class ObservableNeverConverter : ConverterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableNeverConverter"/> class.
        /// </summary>
        /// <param name="config">Compiler configuration.</param>
        public ObservableNeverConverter(CodeGeneratorConfiguration config) : base(config)
        {
        }

        /// <inheritdoc />
        protected override Expression TranformNode(CodeGeneratorContext context)
        {
            Type genericType = typeof(Unit);
            ParameterExpression result = Expression.Variable(typeof(IObservable<>).MakeGenericType(genericType), "ObservableNever");

            MethodInfo methodNever = typeof(Observable).GetMethods()
                                            .Where(m => m.Name == "Never" && m.GetParameters().Length == 0)
                                            .Single().MakeGenericMethod(genericType);
            
            return this.GenerateEventBlock(context.ActualNode, new ParameterExpression[] { result }, Expression.Call(null, methodNever), new List<Expression>(), result, true, "Start of the observable never.", "End of the observable never.", RUNTIME_ERRORS.RE65);
        }
    }
}
