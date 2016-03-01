using System;
using System.Reactive.Concurrency;
using System.Configuration;

namespace Integra.Space.LanguageUnitTests
{
    class DefaultSchedulerFactory : IQuerySchedulerFactory
    {
        static DefaultSchedulerFactory factory = new DefaultSchedulerFactory();

        public static DefaultSchedulerFactory Current
        {
            get
            {
                return factory;
            }
        }

        public IScheduler GetObserverScheduler()
        {
            return System.Reactive.Concurrency.ThreadPoolScheduler.Instance;
        }

        public IScheduler GetSubscriberScheduler()
        {
            return System.Reactive.Concurrency.ThreadPoolScheduler.Instance;
        }

        /// <inheritdoc />
        public IScheduler GetScheduler()
        {
            int selectedScheduler = int.Parse(ConfigurationManager.AppSettings["QueryScheduler"]);

            switch (selectedScheduler)
            {
                case (int)SchedulerTypeEnum.CurrentThreadScheduler:
                    return CurrentThreadScheduler.Instance;
                case (int)SchedulerTypeEnum.DefaultScheduler:
                    return DefaultScheduler.Instance;
                case (int)SchedulerTypeEnum.EventLoopScheduler:
                    return new EventLoopScheduler();
                case (int)SchedulerTypeEnum.HistoricalScheduler:
                    return new HistoricalScheduler();
                case (int)SchedulerTypeEnum.ImmediateScheduler:
                    return ImmediateScheduler.Instance;
                case (int)SchedulerTypeEnum.NewThreadScheduler:
                    return NewThreadScheduler.Default;
                case (int)SchedulerTypeEnum.Scheduler:
                    return Scheduler.Default;
                case (int)SchedulerTypeEnum.TaskPoolScheduler:
                    return TaskPoolScheduler.Default;
                case (int)SchedulerTypeEnum.ThreadPoolScheduler:
                    return ThreadPoolScheduler.Instance;
                default:
                    throw new Exception("Undefined or not suported scheduler.");
            }
        }
    }
}
