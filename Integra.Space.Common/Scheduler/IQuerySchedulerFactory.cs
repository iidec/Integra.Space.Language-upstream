//-----------------------------------------------------------------------
// <copyright file="IQuerySchedulerFactory.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Scheduler
{
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
    }
}
