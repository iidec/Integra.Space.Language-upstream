using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class LeftJoinTests : ReactiveTest
    {
        #region On condition true

        [TestMethod]
        public void LeftJoinTest_OnTrue_1()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(new TimeSpan(40000001).Ticks, Notification.CreateOnNext((object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnTrue_2()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(30000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnTrue_3()
        {
            string eql = "Left " +
                                 "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                 "WITH SpaceObservable1 as t2 " +
                                 "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                 "TIMEOUT '00:00:02' " +
                                 "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(30000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnTrue_4()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:20' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
               () =>
               {
                   return result(input1, input2)
                   .Select(x =>
                   {
                       var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                       var b1 = a.GetType().GetProperty("c1");
                       var b2 = a.GetType().GetProperty("c2");
                       var c = b1.GetValue(a);
                       return (object)(new
                       {
                           c1 = b1.GetValue(a),
                           c2 = b2.GetValue(a)
                       });
                   });
               }
               , TimeSpan.FromSeconds(1).Ticks
               , TimeSpan.FromSeconds(1).Ticks
               , TimeSpan.FromSeconds(15).Ticks
               );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(30000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnTrue_5()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1.AsObservable(), input2.AsObservable())
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(40000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinMultipleEventsTest_OnTrue()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 " + //WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 " + //WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(6).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(8).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(10).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(12).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(6).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(8).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(10).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            var m = results.Messages;

            ReactiveAssert.AreElementsEqual(input1.Subscriptions, new Subscription[] {
                    new Subscription(TimeSpan.FromSeconds(1).Ticks, TimeSpan.FromSeconds(15).Ticks)
                });

            ReactiveAssert.AreElementsEqual(input2.Subscriptions, new Subscription[] {
                    new Subscription(TimeSpan.FromSeconds(1).Ticks, TimeSpan.FromSeconds(15).Ticks)
                });

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(40000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(60000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(80000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(100000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(130000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinMultipleEventsTest_OnTrue_2()
        {
            string eql = "left " +
                                "JOIN SpaceObservable1 as t1 " + //WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 " + //WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            var m = results.Messages;

            ReactiveAssert.AreElementsEqual(input1.Subscriptions, new Subscription[] {
                    new Subscription(TimeSpan.FromSeconds(1).Ticks, TimeSpan.FromSeconds(15).Ticks)
                });

            ReactiveAssert.AreElementsEqual(input2.Subscriptions, new Subscription[] {
                    new Subscription(TimeSpan.FromSeconds(1).Ticks, TimeSpan.FromSeconds(15).Ticks)
                });

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(40000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000002).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000003).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000004).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000005).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000006).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000007).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000008).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000009).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000010).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000011).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000012).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000013).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000014).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000015).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(40000016).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })),
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnTrue_6()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 and t1.@event.Message.#1.#35 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(new TimeSpan(40000001).Ticks, Notification.CreateOnNext((object)(new { c1 = (object)"9999941616073663_1", c2 = (object)"9999941616073663_2" })))
                }, results.Messages);
        }

        #endregion On condition true

        #region On condition false

        [TestMethod]
        public void LeftJoinTest_OnFalse_1()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(60000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnFalse_2()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(40000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnFalse_3()
        {
            string eql = "Left " +
                                 "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                 "WITH SpaceObservable1 as t2 " +
                                 "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                 "TIMEOUT '00:00:02' " +
                                 "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(40000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnFalse_4()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:01' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
               () =>
               {
                   return result(input1, input2)
                   .Select(x =>
                   {
                       var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                       var b1 = a.GetType().GetProperty("c1");
                       var b2 = a.GetType().GetProperty("c2");
                       var c = b1.GetValue(a);
                       return (object)(new
                       {
                           c1 = b1.GetValue(a),
                           c2 = b2.GetValue(a)
                       });
                   });
               }
               , TimeSpan.FromSeconds(1).Ticks
               , TimeSpan.FromSeconds(1).Ticks
               , TimeSpan.FromSeconds(15).Ticks
               );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(30000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnFalse_5()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1.AsObservable(), input2.AsObservable())
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinMultipleEventsTest_OnFalse()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 " + //WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 " + //WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(6).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(8).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(10).Ticks, TestObjects.CreateEventObjectTest1())
                , OnNext(TimeSpan.FromSeconds(12).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(6).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(8).Ticks, TestObjects.CreateEventObjectTest2())
                , OnNext(TimeSpan.FromSeconds(10).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            var m = results.Messages;

            ReactiveAssert.AreElementsEqual(input1.Subscriptions, new Subscription[] {
                    new Subscription(TimeSpan.FromSeconds(1).Ticks, TimeSpan.FromSeconds(15).Ticks)
                });

            ReactiveAssert.AreElementsEqual(input2.Subscriptions, new Subscription[] {
                    new Subscription(TimeSpan.FromSeconds(1).Ticks, TimeSpan.FromSeconds(15).Ticks)
                });

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(50000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(70000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(90000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(110000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(130000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void LeftJoinTest_OnFalse_6()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 and t1.@event.Message.#1.#35 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return result(input1, input2)
                    .Select(x =>
                    {
                        var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                        var b1 = a.GetType().GetProperty("c1");
                        var b2 = a.GetType().GetProperty("c2");
                        var c = b1.GetValue(a);
                        return (object)(new
                        {
                            c1 = b1.GetValue(a),
                            c2 = b2.GetValue(a)
                        });
                    });
                }
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(1).Ticks
                , TimeSpan.FromSeconds(15).Ticks
                );

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    OnNext(new TimeSpan(60000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        #endregion On condition false

        #region Errors

        #region Constants in on condition

        [TestMethod]
        public void LeftJoinTest_ConstantInOnCondition_1()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON \"constant\" == t1.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 172, "\"constant\"", Integra.Space.Language.COMPILATION_ERRORS.CE74));
                return;
            }

            Assert.Fail("Error: se permitió una constante en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_ConstantInOnCondition_2()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == \"constant\" " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 200, "\"constant\"", Integra.Space.Language.COMPILATION_ERRORS.CE74));
                return;
            }

            Assert.Fail("Error: se permitió un valor constante en la condición ON.");
        }

        #endregion Constants in on condition

        #region Invalid comparative operator

        [TestMethod]
        public void LeftJoinTest_NotEqualThanInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 != t2.@event.Message.#1.#32";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 197, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("not equal operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_LessThanInOnCondition()
        {
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 < t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 197, "t1.@event.Message.#1.#32 < t2.@event.Message.#1.#32", Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("less than operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_LessThanOrEqualsInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 <= t2.@event.Message.#1.#32";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 197, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("less than or equal operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_GreaterThanOrEqualsInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 >= t2.@event.Message.#1.#32";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 197, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("greater than or equal operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_GreaterThanInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 > t2.@event.Message.#1.#32";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 197, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("greater than operator")));
                return;
            }

            Assert.Fail("Error: se permitió una constante en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_LikeInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 like \"491381\"";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });

            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 202, "\"491381\"", Integra.Space.Language.COMPILATION_ERRORS.CE74));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_OrInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 or t1.@event.Message.#1.#35 == t2.@event.Message.#1.#35";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });

            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 225, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE76("logical disjunction")), e.InnerException.Message);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_NotInOnCondition()
        {
            string condition = "not(t1.@event.Message.#1.#35 == t2.@event.Message.#1.#35)";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                $"ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 and {condition} " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });

            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 229, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE76("logical negation")), e.InnerException.Message);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void LeftJoinTest_MixedSources_1()
        {
            string instruction = "t2.@event.Message.#1.#2";
            string eql = "Left " +
                                $"JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == {instruction} " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });

            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 88, instruction, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE77("t2")), e.InnerException.InnerException.Message);
                return;
            }

            Assert.Fail("Error: fuente invalida permitida en la condición WHERE de la primera fuente.");
        }

        [TestMethod]
        public void LeftJoinTest_MixedSources_2()
        {
            string instruction = "t1.@event.Message.#1.#2";
            string eql = "Left " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                $"WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == {instruction} " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1"; // , t2.@event.Message.#1.#2 as c2 

            EQLPublicParser parser = new EQLPublicParser(eql);
            PlanNode plan = parser.Evaluate().First();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });

            try
            {
                Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>> result = te.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<object>>(plan);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 170, instruction, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE77("t1")), e.InnerException.InnerException.Message);
                return;
            }

            Assert.Fail("Error: fuente invalida permitida en la condición WHERE de la segunda fuente.");
        }

        #endregion Invalid comparative operator

        #endregion Errors
    }
}
