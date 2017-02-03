//-----------------------------------------------------------------------
// <copyright file="IQuerySchedulerFactory.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    using System.Reactive.Concurrency;

    /// <summary>
    /// Space Scheduler interface
    /// </summary>
    public interface IQuerySchedulerFactory
    {
        /// <summary>
        /// Gets a custom scheduler
        /// </summary>
        /// <returns>Custom scheduler</returns>
        System.Linq.Expressions.Expression GetScheduler();
        
        /// <summary>
        /// Gets the test scheduler.
        /// </summary>
        /// <returns>The test scheduler.</returns>
        IScheduler GetTestScheduler();
    }
}
