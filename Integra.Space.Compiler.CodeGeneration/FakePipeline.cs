//-----------------------------------------------------------------------
// <copyright file="FakePipeline.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Scheduler;
    using Language;
    using Ninject;

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
        public Assembly Process(CodeGeneratorConfiguration context, string script, IQuerySchedulerFactory schedulerFactory)
        {
            QueryParser parser = new QueryParser(script);
            PlanNode executionPlan = parser.Evaluate().Item1;

            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(context.AsmBuilder);
            modBuilder.CreateModuleBuilder();

            TreeTransformations tt = new TreeTransformations(context.AsmBuilder, executionPlan, context.Kernel.Get<ISourceTypeFactory>());
            tt.Transform();
                        
            CodeGenerator te = new CodeGenerator(context);

            return te.Compile(executionPlan);
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <param name="schedulerFactory">Scheduler factory.</param>
        /// <returns>The assembly created.</returns>
        public Assembly ProcessWithExpressionParser(CodeGeneratorConfiguration context, string script, IQuerySchedulerFactory schedulerFactory)
        {
            ExpressionParser parser = new ExpressionParser(script);
            PlanNode executionPlan = parser.Evaluate();
            
            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(context.AsmBuilder);
            modBuilder.CreateModuleBuilder();
            
            CodeGenerator te = new CodeGenerator(context);

            return te.Compile(executionPlan);
        }

        /// <summary>
        /// Proccess only one command.
        /// </summary>
        /// <param name="context">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <returns>The assembly created.</returns>
        public Delegate ProcessWithCommandParser(CodeGeneratorConfiguration context, string script, IGrammarRuleValidator ruleValidator)
        {
            CommandParser parser = new CommandParser(script, ruleValidator);
            SystemCommand command = parser.Evaluate().First().Commands.First();
            
            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(context.AsmBuilder);
            modBuilder.CreateModuleBuilder();
                        
            CodeGenerator te = new CodeGenerator(context);

            PlanNode executionPlan = null;
            if(command is TemporalStreamNode)
            {
                executionPlan = ((TemporalStreamNode)command).ExecutionPlan;
            }
            else
            {
                executionPlan = ((QueryCommandForMetadataNode)command).ExecutionPlan;
            }

            return te.CompileDelegate(executionPlan);
        }
    }
}
