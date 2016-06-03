﻿//-----------------------------------------------------------------------
// <copyright file="FakePipeline.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Fake pipeline class.
    /// </summary>
    internal class FakePipeline
    {
        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <param name="schedulerFactory">Scheduler factory.</param>
        /// <returns>The assembly created.</returns>
        public Assembly Process(CompileContext context, string script, IQuerySchedulerFactory schedulerFactory)
        {
            EQLPublicParser parser = new EQLPublicParser(script);
            PlanNode executionPlan = parser.Evaluate().First();

            SpaceAssemblyBuilder sasmBuilder = new SpaceAssemblyBuilder("SpaceQueryAssembly_" + context.QueryName);
            AssemblyBuilder asmBuilder = sasmBuilder.CreateAssemblyBuilder();
            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(asmBuilder);
            modBuilder.CreateModuleBuilder();

            TreeTransformations tf = new TreeTransformations(asmBuilder, executionPlan);
            tf.Transform();

            context.AsmBuilder = asmBuilder;
            ObservableConstructor te = new ObservableConstructor(context);

            return te.Compile(executionPlan);
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <param name="schedulerFactory">Scheduler factory.</param>
        /// <returns>The assembly created.</returns>
        public Assembly ProcessWithExpressionParser(CompileContext context, string script, IQuerySchedulerFactory schedulerFactory)
        {
            ExpressionParser parser = new ExpressionParser(script);
            PlanNode executionPlan = parser.Evaluate();

            SpaceAssemblyBuilder sasmBuilder = new SpaceAssemblyBuilder("SpaceQueryAssembly_" + context.QueryName);
            AssemblyBuilder asmBuilder = sasmBuilder.CreateAssemblyBuilder();
            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(asmBuilder);
            modBuilder.CreateModuleBuilder();

            TreeTransformations tf = new TreeTransformations(asmBuilder, executionPlan);
            tf.Transform();

            context.AsmBuilder = asmBuilder;
            ObservableConstructor te = new ObservableConstructor(context);

            return te.Compile(executionPlan);
        }
    }
}