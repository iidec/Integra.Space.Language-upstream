//-----------------------------------------------------------------------
// <copyright file="CompileContext.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System.Reflection.Emit;
    using Integra.Space.Language.Metadata;

    /// <summary>
    /// Space compile context
    /// </summary>
    internal class CompileContext
    {
        /// <summary>
        /// Query name.
        /// </summary>
        private string queryName;

        /// <summary>
        /// Observable type for the compilation.
        /// </summary>
        private System.Type observerType;

        /// <summary>
        /// Indicates whether the compilation must be in debug mode.
        /// </summary>
        private bool debugMode = true;

        /// <summary>
        /// Value indicating whether the compilation is for test or not.
        /// </summary>
        private bool isTestMode = false;

        /// <summary>
        /// Gets or sets a value indicating whether the print log is activated.
        /// </summary>
        public bool PrintLog { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to measure the elapsed time of the compiled functions.
        /// </summary>
        public bool MeasureElapsedTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the compilation will be in debug mode.
        /// </summary>
        public bool DebugMode
        {
            get
            {
                return this.debugMode;
            }

            set
            {
                this.debugMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the query name.
        /// </summary>
        public string QueryName
        {
            get
            {
                if (this.queryName == null)
                {
                    return string.Empty;
                }

                return this.queryName;
            }

            set
            {
                this.queryName = value;
            }
        }

        /// <summary>
        /// Gets or sets the space scheduler
        /// </summary>
        public IQuerySchedulerFactory Scheduler { get; set; }

        /// <summary>
        /// Gets or sets the metadata of the query.
        /// </summary>
        public SpaceMetadataTreeNode Metadata { get; set; }

        /// <summary>
        /// Gets or sets the execution plan.
        /// </summary>
        public PlanNode ExecutionPlan { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the compilation is for test or not.
        /// </summary>
        public bool IsTestMode
        {
            get
            {
                return this.isTestMode;
            }

            set
            {
                this.isTestMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the assembly builder for this code generation.
        /// </summary>
        public AssemblyBuilder AsmBuilder { get; set; }
    }
}
