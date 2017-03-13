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
    using Common;

    /// <summary>
    /// Fake pipeline class.
    /// </summary>
    internal class FakePipeline
    {
        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="config">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <param name="schedulerFactory">Scheduler factory.</param>
        /// <returns>The assembly created.</returns>
        public Assembly ProcessWithQueryParser(CodeGeneratorConfiguration config, string script, IQuerySchedulerFactory schedulerFactory)
        {
            QueryParser parser = new QueryParser(script);
            ParseContextBase<Tuple<PlanNode, CommandObject>> parseContext = parser.Evaluate();
            PlanNode executionPlan = parseContext.Payload.Item1;

            if (parseContext.HasErrors())
            {
                throw new Exception();
            }

            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(config.AsmBuilder);
            modBuilder.CreateModuleBuilder();

            TreeTransformations tt = new TreeTransformations(config.AsmBuilder, executionPlan, config.Kernel.Get<ISourceTypeFactory>());
            tt.Transform();
                        
            CodeGenerator te = new CodeGenerator(config);

            return te.Compile(executionPlan);
        }
        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="config">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <param name="schedulerFactory">Scheduler factory.</param>
        /// <returns>The assembly created.</returns>
        public ParseContextBase<Tuple<PlanNode, CommandObject>> ProcessWithQueryParser2(CodeGeneratorConfiguration config, string script, IQuerySchedulerFactory schedulerFactory)
        {
            QueryParser parser = new QueryParser(script);
            return parser.Evaluate();
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="config">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <param name="schedulerFactory">Scheduler factory.</param>
        /// <returns>The assembly created.</returns>
        public Assembly ProcessWithExpressionParser(CodeGeneratorConfiguration config, string script, IQuerySchedulerFactory schedulerFactory)
        {
            ExpressionParser parser = new ExpressionParser(script);
            ParseContextBase<PlanNode> parseContext = parser.Evaluate();

            if (parseContext.HasErrors())
            {
                throw new Exception();
            }

            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(config.AsmBuilder);
            modBuilder.CreateModuleBuilder();
            
            CodeGenerator te = new CodeGenerator(config);

            return te.Compile(parseContext.Payload);
        }

        /// <summary>
        /// Proccess only one command.
        /// </summary>
        /// <param name="config">Compilation context.</param>
        /// <param name="script">EQL query.</param>
        /// <returns>The assembly created.</returns>
        public Delegate ProcessWithCommandParser(CodeGeneratorConfiguration config, string script, IGrammarRuleValidator ruleValidator)
        {
            SpaceModuleBuilder modBuilder = new SpaceModuleBuilder(config.AsmBuilder);
            modBuilder.CreateModuleBuilder();
            CodeGenerator te = new CodeGenerator(config);

            CommandParser parser = new CommandParser(script, ruleValidator);
            ParseContext parseContext = parser.Evaluate();
            SystemCommand command = null;
            if (parseContext.HasErrors())
            {
                throw new Exception();
            }
            else
            {
                foreach (var batch in parseContext.Payload)
                {
                    if (batch.HasErrors())
                    {
                        throw new Exception();
                    }
                    else
                    {
                        command = batch.Commands.First();
                    }
                }
            }

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
