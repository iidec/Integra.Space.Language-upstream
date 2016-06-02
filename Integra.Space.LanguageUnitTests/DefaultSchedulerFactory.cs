using System;
using System.Reactive.Concurrency;
using System.Configuration;
using Microsoft.Reactive.Testing;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Integra.Space.LanguageUnitTests
{
    class DefaultSchedulerFactory : IQuerySchedulerFactory
    {
        private static DefaultSchedulerFactory factory = new DefaultSchedulerFactory();
        private TestScheduler testScheduler;
        private Expression testSchedulerExpression;
        
        public DefaultSchedulerFactory()
        {
            this.testScheduler = new TestScheduler();
        }

        public static DefaultSchedulerFactory Current
        {
            get
            {
                return factory;
            }
        }

        public TestScheduler TestScheduler
        {
            get
            {
                return this.testScheduler;
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
        public Expression GetScheduler()
        {
            int selectedScheduler = int.Parse(ConfigurationManager.AppSettings["QueryScheduler"]);

            switch (selectedScheduler)
            {
                case (int)SchedulerTypeEnum.CurrentThreadScheduler:
                    return Expression.Property(null, typeof(CurrentThreadScheduler).GetProperty("Instance"));
                case (int)SchedulerTypeEnum.DefaultScheduler:
                    return Expression.Property(null, typeof(DefaultScheduler).GetProperty("Instance"));
                case (int)SchedulerTypeEnum.EventLoopScheduler:
                    return Expression.New(typeof(EventLoopScheduler));
                case (int)SchedulerTypeEnum.HistoricalScheduler:
                    return Expression.New(typeof(HistoricalScheduler));
                case (int)SchedulerTypeEnum.ImmediateScheduler:
                    return Expression.Property(null, typeof(ImmediateScheduler).GetProperty("Instance"));
                case (int)SchedulerTypeEnum.NewThreadScheduler:
                    return Expression.Property(null, typeof(NewThreadScheduler).GetProperty("Default"));
                case (int)SchedulerTypeEnum.Scheduler:
                    return Expression.Property(null, typeof(Scheduler).GetProperty("Default"));
                case (int)SchedulerTypeEnum.TaskPoolScheduler:
                    return Expression.Property(null, typeof(TaskPoolScheduler).GetProperty("Default"));
                case (int)SchedulerTypeEnum.ThreadPoolScheduler:
                    return Expression.Property(null, typeof(ThreadPoolScheduler).GetProperty("Instance"));
                case (int)SchedulerTypeEnum.TestScheduler:
                    this.testSchedulerExpression = Expression.New(typeof(TestScheduler));
                    return testSchedulerExpression;
                default:
                    throw new Exception("Undefined or not suported scheduler.");
            }
        }

        public IScheduler GetTestScheduler()
        {
            return this.testScheduler;
        }
    }
}
