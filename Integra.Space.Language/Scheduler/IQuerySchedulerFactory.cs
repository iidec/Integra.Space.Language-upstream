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
        System.Reactive.Concurrency.IScheduler GetScheduler();
    }
}
