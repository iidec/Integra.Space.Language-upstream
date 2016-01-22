using Integra.Space.Language.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Configuration;

namespace Integra.Space.LanguageUnitTests
{
    class TestSchedulerFactory : IQuerySchedulerFactory
    {
        public DateTimeOffset Now
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IScheduler GetScheduler()
        {
            int selectedScheduler = int.Parse(ConfigurationManager.AppSettings["Scheduler"]);

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

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            throw new NotImplementedException();
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            throw new NotImplementedException();
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            throw new NotImplementedException();
        }
    }
}
