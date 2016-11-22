using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive;
using Integra.Space.Language.Runtime;
using System.Collections.Generic;
using Integra.Space.Language.Exceptions;
using System.Reflection;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class TemporalStreamsTests
    {
        private IObservable<object> Process(string eql, DefaultSchedulerFactory dsf, ITestableObservable<EventObject> input)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            CompilerConfiguration context = new CompilerConfiguration() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = isTestMode };

            FakePipeline fp = new FakePipeline();
            Delegate d = fp.ProcessWithCommandParser<EventObject>(context, eql);

            if (isTestMode)
            {
                return (IObservable<object>)d.DynamicInvoke(input.AsObservable(), dsf.TestScheduler);
            }
            else
            {
                return (IObservable<object>)d.DynamicInvoke(input.AsObservable());
            }
        }

        [TestMethod]
        public void ConsultaProyeccionConWhereApplyDuration()
        {
            string eql = string.Format("from {0} where {1} select {2} as CampoNulo {3} into SourceXYZ",
                                                                "SpaceObservable1",
                                                                "@event.Message.#0.MessageType == \"0100\"",
                                                                "@event.Message.#0.MessageType",
                                                                "apply duration of '00:00:00:01'");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(TimeSpan.FromMilliseconds(100).Ticks, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(TimeSpan.FromSeconds(2).Ticks, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(TimeSpan.FromSeconds(3).Ticks, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(TimeSpan.FromSeconds(5).Ticks, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        CampoNulo = (string)((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("CampoNulo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0))
                    })
                )
                ,
                created: 10,
                subscribed: 50,
                disposed: TimeSpan.FromSeconds(7).Ticks);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(TimeSpan.FromMilliseconds(100).Ticks, Notification.CreateOnNext((object)(new { CampoNulo = "0100" }))),
                    new Recorded<Notification<object>>(TimeSpan.FromSeconds(1).Ticks + 50, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, TimeSpan.FromSeconds(1).Ticks + 50)
                });
        }



        [TestMethod]
        public void ConsultaProyeccionConWhereApplyRepetition()
        {
            string eql = string.Format("from {0} where {1} select {2} as CampoNulo {3} into SourceXYZ",
                                                                "SpaceObservable1",
                                                                "@event.Message.#0.MessageType == \"0100\"",
                                                                "@event.Message.#0.MessageType",
                                                                "apply repetition of 1");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(TimeSpan.FromMilliseconds(100).Ticks, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(TimeSpan.FromSeconds(2).Ticks, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(TimeSpan.FromSeconds(3).Ticks, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(TimeSpan.FromSeconds(5).Ticks, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        CampoNulo = (string)((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("CampoNulo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0))
                    })
                )
                ,
                created: 10,
                subscribed: 50,
                disposed: TimeSpan.FromSeconds(7).Ticks);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(TimeSpan.FromMilliseconds(100).Ticks, Notification.CreateOnNext((object)(new { CampoNulo = "0100" }))),
                    new Recorded<Notification<object>>(TimeSpan.FromMilliseconds(100).Ticks, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, TimeSpan.FromMilliseconds(100).Ticks)
                });
        }
    }
}
