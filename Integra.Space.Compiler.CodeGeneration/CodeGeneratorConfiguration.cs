//-----------------------------------------------------------------------
// <copyright file="CodeGeneratorConfiguration.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System.Linq;
    using System.Reflection.Emit;
    using Integra.Space.Language.Metadata;
    using Scheduler;
    using Language;
    using Ninject;

    /// <summary>
    /// Space compile context
    /// </summary>
    internal class CodeGeneratorConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorConfiguration"/> class.
        /// </summary>
        /// <param name="schedulerFactory">The scheduler that will be used in the observable .</param>
        public CodeGeneratorConfiguration(IQuerySchedulerFactory schedulerFactory, AssemblyBuilder assemblyBuilder, IKernel kernel
            , string queryName = "", bool debugMode = false, bool isTestMode = false, bool printLog = false, bool measureElapsedTime = false)
        {
            this.SchedulerFactory = schedulerFactory;
            this.AsmBuilder = assemblyBuilder;
            this.QueryName = queryName;
            this.DebugMode = debugMode;
            this.IsTestMode = isTestMode;
            this.PrintLog = printLog;
            this.MeasureElapsedTime = measureElapsedTime;
            this.Kernel = kernel;
        }

        /// <summary>
        /// Gets a value indicating whether the print log is activated.
        /// </summary>
        public bool PrintLog { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to measure the elapsed time of the compiled functions.
        /// </summary>
        public bool MeasureElapsedTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the compilation will be in debug mode.
        /// </summary>
        public bool DebugMode { get; private set; }

        /// <summary>
        /// Gets the query name.
        /// </summary>
        public string QueryName { get; private set; }

        /// <summary>
        /// Gets the space scheduler
        /// </summary>
        public IQuerySchedulerFactory SchedulerFactory { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the compilation is for test or not.
        /// </summary>
        public bool IsTestMode { get; private set; }
        
        /// <summary>
        /// Gets the assembly builder for this code generation.
        /// </summary>
        public AssemblyBuilder AsmBuilder { get; private set; }

        /// <summary>
        /// DI kernel.
        /// </summary>
        public IKernel Kernel { get; private set; }
    }
}
