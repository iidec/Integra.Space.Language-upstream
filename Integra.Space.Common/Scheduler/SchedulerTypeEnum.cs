//-----------------------------------------------------------------------
// <copyright file="SchedulerTypeEnum.cs" company="Integra.Space.Services">
//     Copyright (c) Integra.Space.Services. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Scheduler
{
    /// <summary>
    /// Scheduler type enumerable
    /// </summary>
    internal enum SchedulerTypeEnum
    {
        /// <summary>
        /// CurrentThreadScheduler type
        /// </summary>
        CurrentThreadScheduler = 0,

        /// <summary>
        /// DefaultScheduler type
        /// </summary>
        DefaultScheduler = 1,

        /// <summary>
        /// EventLoopScheduler type
        /// </summary>
        EventLoopScheduler = 2,

        /// <summary>
        /// HistoricalScheduler type
        /// </summary>
        HistoricalScheduler = 3,

        /// <summary>
        /// ImmediateScheduler type
        /// </summary>
        ImmediateScheduler = 5,

        /// <summary>
        /// NewThreadScheduler type
        /// </summary>
        NewThreadScheduler = 7,

        /// <summary>
        /// Scheduler type
        /// </summary>
        Scheduler = 8,

        /// <summary>
        /// TaskPoolScheduler type
        /// </summary>
        TaskPoolScheduler = 9,

        /// <summary>
        /// ThreadPoolScheduler 10
        /// </summary>
        ThreadPoolScheduler = 10,

        /// <summary>
        /// TestScheduler 11
        /// </summary>
        TestScheduler = 11
    }
}
