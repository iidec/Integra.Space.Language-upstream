using ET_Test;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using Integra.Space.LanguageUnitTests.Helpers;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class CrossJoinTests : ReactiveTest
    {
        private IObservable<object> Process(string eql, DefaultSchedulerFactory dsf, ITestableObservable<EventObject> input1, ITestableObservable<EventObject> input2, bool printLog = false, bool debugMode = false, bool measureElapsedTime = false)
        {
            CompileContext context = new CompileContext() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = true };

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.Process(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");
            
            return ((IObservable<object>)result.Invoke(queryObject, new object[] { input1.AsQbservable(), input2.AsQbservable(), dsf.TestScheduler }));
        }

        #region On condition true

        [TestMethod]
        public void CrossJoinTest_OnTrue_1()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinTest_OnTrue_2()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinTest_OnTrue_3()
        {
            string eql = "cross " +
                                 "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                 "WITH SpaceObservable1 as t2 " +
                                 "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                 "TIMEOUT '00:00:02' " +
                                 "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinTest_OnTrue_4()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:20' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
               () =>
               {
                   return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinTest_OnTrue_5()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinMultipleEventsTest_OnTrue()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 " + //WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 " + //WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
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
                    return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinMultipleEventsTest_OnTrue_2()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 " + //WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 " + //WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
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
                    return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinTest_OnTrue_6()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 and t1.@event.Message.#1.#35 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
        public void CrossJoinTest_OnFalse_1()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(60000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void CrossJoinTest_OnFalse_2()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
                    OnNext(new TimeSpan(40000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void CrossJoinTest_OnFalse_3()
        {
            string eql = "cross " +
                                 "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                 "WITH SpaceObservable1 as t2 " +
                                 "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                 "TIMEOUT '00:00:02' " +
                                 "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
                    OnNext(new TimeSpan(40000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void CrossJoinTest_OnFalse_4()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:01' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(2).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
               () =>
               {
                   return this.Process(eql, dsf, input1, input2)
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
                    OnNext(new TimeSpan(30000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(40000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void CrossJoinTest_OnFalse_5()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 " +
                                "WITH SpaceObservable1 as t2 " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(60000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void CrossJoinMultipleEventsTest_OnFalse()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 " + //WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 " + //WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
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
                    return this.Process(eql, dsf, input1, input2)
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
                    OnNext(new TimeSpan(50000003).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(70000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(70000003).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(90000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(90000003).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(110000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null })),
                    OnNext(new TimeSpan(110000003).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(130000001).Ticks,(object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        [TestMethod]
        public void CrossJoinTest_OnFalse_6()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#35 and t1.@event.Message.#1.#35 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
                        
            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(4).Ticks, TestObjects.CreateEventObjectTest1())
                );

            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<EventObject>(TimeSpan.FromSeconds(3).Ticks, TestObjects.CreateEventObjectTest2())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2)
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
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(60000001).Ticks, (object)(new { c1 = (object)"9999941616073663_1", c2 = (object)null }))
                }, results.Messages);
        }

        #endregion On condition false

        #region Errors

        #region Constants in on condition

        [TestMethod]
        public void CrossJoinTest_ConstantInOnCondition_1()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON \"constant\" == t1.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 173, "\"constant\"", Integra.Space.Language.COMPILATION_ERRORS.CE74));
                return;
            }

            Assert.Fail("Error: se permitió una constante en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_ConstantInOnCondition_2()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == \"constant\" " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 201, "\"constant\"", Integra.Space.Language.COMPILATION_ERRORS.CE74));
                return;
            }

            Assert.Fail("Error: se permitió un valor no constante en la condición del primer WHERE.");
        }

        #endregion Constants in on condition

        #region Invalid comparative operator

        [TestMethod]
        public void CrossJoinTest_NotEqualThanInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 != t2.@event.Message.#1.#32";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 198, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("not equal operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_LessThanInOnCondition()
        {
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 < t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 198, "t1.@event.Message.#1.#32 < t2.@event.Message.#1.#32", Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("less than operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_LessThanOrEqualsInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 <= t2.@event.Message.#1.#32";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 198, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("less than or equal operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_GreaterThanOrEqualsInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 >= t2.@event.Message.#1.#32";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 198, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("greater than or equal operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_GreaterThanInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 > t2.@event.Message.#1.#32";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 198, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE75("greater than operator")));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_LikeInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 like \"491381\"";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(e.InnerException.Message, string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 203, "\"491381\"", Integra.Space.Language.COMPILATION_ERRORS.CE74));
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_OrInOnCondition()
        {
            string condition = "t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 or t1.@event.Message.#1.#35 == t2.@event.Message.#1.#35";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 226, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE76("logical disjunction")), e.InnerException.Message);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_NotInOnCondition()
        {
            string condition = "not(t1.@event.Message.#1.#35 == t2.@event.Message.#1.#35)";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                $"ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 and {condition} " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 230, condition, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE76("logical negation")), e.InnerException.Message);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void CrossJoinTest_MixedSources_1()
        {
            string instruction = "t2.@event.Message.#1.#2";
            string eql = "cross " +
                                $"JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == {instruction} " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 89, instruction, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE77("t2")), e.InnerException.InnerException.Message);
                return;
            }

            Assert.Fail("Error: fuente invalida permitida en la condición WHERE de la primera fuente.");
        }

        [TestMethod]
        public void CrossJoinTest_MixedSources_2()
        {
            string instruction = "t1.@event.Message.#1.#2";
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                $"WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == {instruction} " +
                                "ON t1.@event.Message.#1.#32 == t2.@event.Message.#1.#32 " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.@event.Message.#1.#2 as c1"; // , t2.@event.Message.#1.#2 as c2 
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate();
                CompileContext context = new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf, DebugMode = true, IsTestMode = true, MeasureElapsedTime = false };
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 171, instruction, Integra.Space.Language.Resources.COMPILATION_ERRORS.CE77("t1")), e.InnerException.InnerException.Message);
                return;
            }

            Assert.Fail("Error: fuente invalida permitida en la condición WHERE de la segunda fuente.");
        }

        #endregion Invalid comparative operator

        #endregion Errors

        #region Custom load tests

        [TestMethod]
        public void CustomLoadTest1()
        {
            #region Compiler

            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#0.#0 == \"0100\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#0.#0 == \"0110\" " +
                                "ON t1.@event.Message.#1.#0 == t2.@event.Message.#1.#0 and t1.@event.Message.#1.#1 == t2.@event.Message.#1.#1 " +
                                "TIMEOUT '00:00:04' " +
                                "WHERE  isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') <= '00:00:01' " +
                                "SELECT  isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') as o1, " +
                                        "1 as o2, " +
                                        "isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(null, '01/01/2016') as o3, " +
                                        "t1.@event.Message.#1.#0 as c1, t1.@event.Message.#1.#1 as c2, isnull(t1.@event.SourceTimestamp, '01/01/2016') as ts1, " +
                                        "t2.@event.Message.#1.#0 as c3, t2.@event.Message.#1.#1 as c4, isnull(t2.@event.SourceTimestamp, '01/01/2017') as ts2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            #endregion Compiler
            
            #region parameters

            // contexto
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;

            // para la creación de eventos
            decimal tolerance = 0.5M;
            int eventNumber = 10;
            int limiteSuperiorOcurrenciaEventos = 10000;
            int timeoutPercentage = 0;
            int timeout = 4000;
            int whereDifference = 1000;
            bool evaluateMatchedEvents = true;

            #endregion parameters

            #region Creation of events

            LoadTestsHelper helper = new LoadTestsHelper(eventNumber, timeout, whereDifference, limiteSuperiorOcurrenciaEventos, timeoutPercentage, evaluateMatchedEvents);
            Tuple<Tuple<EventObject, long>[], Tuple<EventObject, long>[], Tuple<string, string, string, string, bool>[]> ltEvents = helper.CreateEvents(JoinTypeEnum.Cross);

            Tuple<EventObject, long>[] rqCreated = ltEvents.Item1;
            Tuple<EventObject, long>[] rsCreated = ltEvents.Item2;
            Tuple<string, string, string, string, bool>[] expectedResults = ltEvents.Item3;

            #endregion Creation of events

            #region Print created events

            int countLeft = 0;
            int countRight = 0;
            rqCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countLeft++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            System.Diagnostics.Debug.WriteLine("----------------------------------");

            rsCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countRight++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            #endregion Print created events

            #region Inputs creation

            List<Recorded<Notification<EventObject>>> rq = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rqCreated)
            {
                rq.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rq.Distinct().Count() < eventNumber)
            {
                throw new Exception("Solicitudes repetidas.");
            }

            List<Recorded<Notification<EventObject>>> rs = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rsCreated)
            {
                rs.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rs.Distinct().Count() < eventNumber)
            {
                throw new Exception("Respuestas repetidas.");
            }

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(rq.ToArray());
            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(rs.ToArray());

            #endregion Inputs creation

            #region Run test

            long maxTimeLeft = rqCreated.Max(x => x.Item2);
            long maxTimeRight = rsCreated.Max(x => x.Item2);
            long maxTime = maxTimeLeft > maxTimeRight ? maxTimeLeft : maxTimeRight;

            Stopwatch swJoin = Stopwatch.StartNew();

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2, printLog, debugMode, measureElapsedTime)
                            .Select(x =>
                            {
                                var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                                var b1 = a.GetType().GetProperty("c1");
                                var b2 = a.GetType().GetProperty("c2");
                                var b3 = a.GetType().GetProperty("c3");
                                var b4 = a.GetType().GetProperty("c4");
                                var b5 = a.GetType().GetProperty("ts1");
                                var b6 = a.GetType().GetProperty("ts2");
                                var b7 = a.GetType().GetProperty("o1");
                                var b8 = a.GetType().GetProperty("o2");
                                var b9 = a.GetType().GetProperty("o3");
                                return (object)(new
                                {
                                    o1 = b7.GetValue(a),
                                    o2 = b8.GetValue(a),
                                    o3 = b9.GetValue(a),
                                    ts1 = b5.GetValue(a),
                                    ts2 = b6.GetValue(a),
                                    c1 = b1.GetValue(a),
                                    c2 = b2.GetValue(a),
                                    c3 = b3.GetValue(a),
                                    c4 = b4.GetValue(a)
                                });
                            });
                }
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , maxTime + TimeSpan.FromSeconds(10).Ticks // tienen que ser mayor que el límite máximo definido para el envio de eventos, "maxLimitTimeTest" del constructor de la clase LoadTestsHelper
                );

            swJoin.Stop();
            TimeSpan tiempoDelJoin = swJoin.Elapsed;

            #endregion Run test

            #region Extract information from results

            Tuple<string, string, string, string, string, string, TimeSpan>[] actualResults = results.Messages
                .Select<Recorded<Notification<object>>, Tuple<string, string, string, string, string, string, TimeSpan>>(x =>
                {
                    dynamic rAux = ((dynamic)x.Value.Value);
                    return Tuple.Create<string, string, string, string, string, string, TimeSpan>(rAux.c1, rAux.c2, ((DateTime?)rAux.ts1).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.c3, rAux.c4, ((DateTime?)rAux.ts2).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.o1);
                })
                .ToArray();

            //helper.UpdateExpectedEventsMatchedFlag(actualResults);

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciasActualResults = new List<Tuple<string, string, string, string, bool>>();
            actualResults.ForEach(x =>
            {
                Tuple<string, string, string, string, bool> aux = expectedResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item3 == x.Item4 && y.Item4 == x.Item5);

                if (aux != null)
                {
                    expectedResultsUpdated.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciasActualResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item4, x.Item5, false));
                }
            });

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated2 = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciaExpectedResults = new List<Tuple<string, string, string, string, bool>>();
            expectedResults.ForEach(x =>
            {
                Tuple<string, string, string, string, string, string, TimeSpan> aux = actualResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item4 == x.Item3 && y.Item5 == x.Item4);

                if (aux != null)
                {
                    expectedResultsUpdated2.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciaExpectedResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4, false));
                }
            });

            decimal exactitudAlcanzada = ((decimal)(expectedResultsUpdated.Count() * 100)) / expectedResults.Count();

            #endregion Extract information from results

            #region Report generation from result information

            if (expectedResultsUpdated.Count != expectedResultsUpdated2.Count)
            {
                Assert.Fail("Falsos positivos entre los eventos resultantes obtenidos.");
            }

            string premisas = $"PrintLog: {printLog} \nDebugMode: {debugMode} \nMeasureElapsedTime: {measureElapsedTime} \nTolerancia: {tolerance} \nNumero de eventos: {eventNumber} eventos \nLimite superior ocurrencia de eventos: {limiteSuperiorOcurrenciaEventos} ms " +
                                $"\nPorcentaje de timeouts: {timeoutPercentage} % \nTimeout: {timeout} ms \nTimestamp condición en where: {whereDifference} ms \nEvaluar eventos coincidentes: {evaluateMatchedEvents} " +
                                $"\nBuffer actual: {System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]} " +
                                $"\nTamaño máximo del buffer: {System.Configuration.ConfigurationManager.AppSettings["MaxWindowSize"]}";

            string report = $"{premisas} " +
                                $"\n\nDuración de la prueba: {tiempoDelJoin} \n" +
                                $"Resultados actuales: {actualResults.Count()} eventos \n" +
                                $"Resultados esperados: {expectedResults.Count()} eventos \n" +
                                $"Coincidencias: {expectedResultsUpdated.Count()} eventos \n" +
                                $"Diferencias entre resultados esperados y actuales: {diferenciaExpectedResults.Count} eventos \n" +
                                $"Diferencias entre resultados actuales y esperados: {diferenciasActualResults.Count} eventos \n" +
                                $"Exactitud alcanzada: {exactitudAlcanzada} % \n";

            if (expectedResults.Where(x => x.Item5 == false).Count() > 0)
            {
                if (exactitudAlcanzada == 100)
                {
                    return;
                }
                if (exactitudAlcanzada < (100 - tolerance))
                {
                    Assert.Fail("Tolerancia no alcanzada.\n" + report);
                }
                else
                {
                    Assert.Inconclusive("Number of expected results is differ from number of actual results.\n" + report);
                }
            }

            #endregion  Report generation from result information
        }

        [TestMethod]
        public void CustomLoadTest2()
        {
            #region Compiler
            
            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#0.#0 == \"0100\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#0.#0 == \"0110\" " +
                                "ON (string)t1.@event.Message.#1.#0 == (string)t2.@event.Message.#1.#0 and (string)t1.@event.Message.#1.#1 == (string)t2.@event.Message.#1.#1 " +
                                "TIMEOUT '00:00:04' " +
                                "WHERE  isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') > '00:00:01' " +
                                "SELECT isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') as o1, " +
                                        "1 as o2, " +
                                        "isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(null, '01/01/2016') as o3, " +
                                        "t1.@event.Message.#1.#0 as c1, t1.@event.Message.#1.#1 as c2, isnull(t1.@event.SystemTimestamp, '01/01/2016') as ts1, " +
                                        "t2.@event.Message.#1.#0 as c3, t2.@event.Message.#1.#1 as c4, isnull(t2.@event.SourceTimestamp, '01/01/2017') as ts2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            #endregion Compiler

            #region parameters

            // contexto
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;

            // para la creación de eventos
            decimal tolerance = 0.5M;
            int eventNumber = 10000;
            int limiteSuperiorOcurrenciaEventos = 10000;
            int timeoutPercentage = 100;
            int timeout = 4000;
            int whereDifference = 1000;
            bool evaluateMatchedEvents = false;

            #endregion parameters
                        
            #region Creation of events

            LoadTestsHelper helper = new LoadTestsHelper(eventNumber, timeout, whereDifference, limiteSuperiorOcurrenciaEventos, timeoutPercentage, evaluateMatchedEvents);
            Tuple<Tuple<EventObject, long>[], Tuple<EventObject, long>[], Tuple<string, string, string, string, bool>[]> ltEvents = helper.CreateEvents(JoinTypeEnum.Cross);
            
            Tuple<EventObject, long>[] rqCreated = ltEvents.Item1;
            Tuple<EventObject, long>[] rsCreated = ltEvents.Item2;
            Tuple<string, string, string, string, bool>[] expectedResults = ltEvents.Item3;

            #endregion Creation of events

            #region Print created events

            int countLeft = 0;
            int countRight = 0;
            rqCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countLeft++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            System.Diagnostics.Debug.WriteLine("----------------------------------");

            rsCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countRight++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            #endregion Print created events

            #region Inputs creation

            List<Recorded<Notification<EventObject>>> rq = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rqCreated)
            {
                rq.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rq.Distinct().Count() < eventNumber)
            {
                throw new Exception("Solicitudes repetidas.");
            }

            List<Recorded<Notification<EventObject>>> rs = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rsCreated)
            {
                rs.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rs.Distinct().Count() < eventNumber)
            {
                throw new Exception("Respuestas repetidas.");
            }

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(rq.ToArray());
            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(rs.ToArray());

            #endregion Inputs creation

            #region Run test

            long maxTimeLeft = rqCreated.Max(x => x.Item2);
            long maxTimeRight = rsCreated.Max(x => x.Item2);
            long maxTime = maxTimeLeft > maxTimeRight ? maxTimeLeft : maxTimeRight;

            Stopwatch swJoin = Stopwatch.StartNew();

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2, printLog, debugMode, measureElapsedTime)
                            .Select(x =>
                            {
                                var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                                var b1 = a.GetType().GetProperty("c1");
                                var b2 = a.GetType().GetProperty("c2");
                                var b3 = a.GetType().GetProperty("c3");
                                var b4 = a.GetType().GetProperty("c4");
                                var b5 = a.GetType().GetProperty("ts1");
                                var b6 = a.GetType().GetProperty("ts2");
                                var b7 = a.GetType().GetProperty("o1");
                                var b8 = a.GetType().GetProperty("o2");
                                var b9 = a.GetType().GetProperty("o3");
                                return (object)(new
                                {
                                    o1 = b7.GetValue(a),
                                    o2 = b8.GetValue(a),
                                    o3 = b9.GetValue(a),
                                    ts1 = b5.GetValue(a),
                                    ts2 = b6.GetValue(a),
                                    c1 = b1.GetValue(a),
                                    c2 = b2.GetValue(a),
                                    c3 = b3.GetValue(a),
                                    c4 = b4.GetValue(a)
                                });
                            });
                }
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , maxTime + TimeSpan.FromSeconds(10).Ticks // tienen que ser mayor que el límite máximo definido para el envio de eventos, "maxLimitTimeTest" del constructor de la clase LoadTestsHelper
                );

            swJoin.Stop();
            TimeSpan tiempoDelJoin = swJoin.Elapsed;

            #endregion Run test

            #region Extract information from results

            Tuple<string, string, string, string, string, string, TimeSpan>[] actualResults = results.Messages
                .Select<Recorded<Notification<object>>, Tuple<string, string, string, string, string, string, TimeSpan>>(x =>
                {
                    dynamic rAux = ((dynamic)x.Value.Value);
                    return Tuple.Create<string, string, string, string, string, string, TimeSpan>(rAux.c1, rAux.c2, ((DateTime?)rAux.ts1).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.c3, rAux.c4, ((DateTime?)rAux.ts2).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.o1);
                })
                .ToArray();

            //helper.UpdateExpectedEventsMatchedFlag(actualResults);

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciasActualResults = new List<Tuple<string, string, string, string, bool>>();
            actualResults.ForEach(x =>
            {
                Tuple<string, string, string, string, bool> aux = expectedResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item3 == x.Item4 && y.Item4 == x.Item5);

                if (aux != null)
                {
                    expectedResultsUpdated.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciasActualResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item4, x.Item5, false));
                }
            });

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated2 = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciaExpectedResults = new List<Tuple<string, string, string, string, bool>>();
            expectedResults.ForEach(x =>
            {
                Tuple<string, string, string, string, string, string, TimeSpan> aux = actualResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item4 == x.Item3 && y.Item5 == x.Item4);

                if (aux != null)
                {
                    expectedResultsUpdated2.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciaExpectedResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4, false));
                }
            });

            decimal exactitudAlcanzada = ((decimal)(expectedResultsUpdated.Count() * 100)) / expectedResults.Count();

            #endregion Extract information from results

            #region Report generation from result information

            if (expectedResultsUpdated.Count != expectedResultsUpdated2.Count)
            {
                Assert.Fail("Falsos positivos entre los eventos resultantes obtenidos.");
            }
            
            string premisas = $"PrintLog: {printLog} \nDebugMode: {debugMode} \nMeasureElapsedTime: {measureElapsedTime} \nTolerancia: {tolerance} \nNumero de eventos: {eventNumber} eventos \nLimite superior ocurrencia de eventos: {limiteSuperiorOcurrenciaEventos} ms " +
                                $"\nPorcentaje de timeouts: {timeoutPercentage} % \nTimeout: {timeout} ms \nTimestamp condición en where: {whereDifference} ms \nEvaluar eventos coincidentes: {evaluateMatchedEvents} " +
                                $"\nBuffer actual: {System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]} " +
                                $"\nTamaño máximo del buffer: {System.Configuration.ConfigurationManager.AppSettings["MaxWindowSize"]}";

            string report = $"{premisas} " +
                                $"\n\nDuración de la prueba: {tiempoDelJoin} \n" +
                                $"Resultados actuales: {actualResults.Count()} eventos \n" +
                                $"Resultados esperados: {expectedResults.Count()} eventos \n" +
                                $"Coincidencias: {expectedResultsUpdated.Count()} eventos \n" +
                                $"Diferencias entre resultados esperados y actuales: {diferenciaExpectedResults.Count} eventos \n" +
                                $"Diferencias entre resultados actuales y esperados: {diferenciasActualResults.Count} eventos \n" +
                                $"Exactitud alcanzada: {exactitudAlcanzada} % \n";

            if (expectedResults.Where(x => x.Item5 == false).Count() > 0)
            {
                if (exactitudAlcanzada == 100)
                {
                    return;
                }
                if (exactitudAlcanzada < (100 - tolerance))
                {
                    Assert.Fail("Tolerancia no alcanzada.\n" + report);
                }
                else
                {
                    Assert.Inconclusive("Number of expected results is differ from number of actual results.\n" + report);
                }
            }

            #endregion  Report generation from result information
        }

        [TestMethod]
        public void CustomLoadTest3()
        {
            #region Compiler

            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#0.#0 == \"0100\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#0.#0 == \"0110\" " +
                                "ON t1.@event.Message.#1.#0 == t2.@event.Message.#1.#0 and t1.@event.Message.#1.#1 == t2.@event.Message.#1.#1 " +
                                "TIMEOUT '00:00:04' " +
                                "WHERE  isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') > '00:00:01' " +
                                "SELECT isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') as o1, " +
                                        "1 as o2, " +
                                        "isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(null, '01/01/2016') as o3, " +
                                        "t1.@event.Message.#1.#0 as c1, t1.@event.Message.#1.#1 as c2, isnull(t1.@event.SourceTimestamp, '01/01/2016') as ts1, " +
                                        "t2.@event.Message.#1.#0 as c3, t2.@event.Message.#1.#1 as c4, isnull(t2.@event.SourceTimestamp, '01/01/2017') as ts2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            #endregion Compiler
            
            #region parameters

            // contexto
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;

            // para la creación de eventos
            decimal tolerance = 0.5M;
            int eventNumber = 10;
            int limiteSuperiorOcurrenciaEventos = 10000;
            int timeoutPercentage = 15;
            int timeout = 4000;
            int whereDifference = 1000;
            bool evaluateMatchedEvents = false;

            #endregion parameters

            #region Creation of events

            LoadTestsHelper helper = new LoadTestsHelper(eventNumber, timeout, whereDifference, limiteSuperiorOcurrenciaEventos, timeoutPercentage, evaluateMatchedEvents);
            Tuple<Tuple<EventObject, long>[], Tuple<EventObject, long>[], Tuple<string, string, string, string, bool>[]> ltEvents = helper.CreateEvents(JoinTypeEnum.Cross);

            Tuple<EventObject, long>[] rqCreated = ltEvents.Item1;
            Tuple<EventObject, long>[] rsCreated = ltEvents.Item2;
            Tuple<string, string, string, string, bool>[] expectedResults = ltEvents.Item3;

            #endregion Creation of events

            #region Print created events

            int countLeft = 0;
            int countRight = 0;
            rqCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countLeft++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            System.Diagnostics.Debug.WriteLine("----------------------------------");

            rsCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countRight++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            #endregion Print created events

            #region Inputs creation

            List<Recorded<Notification<EventObject>>> rq = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rqCreated)
            {
                rq.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rq.Distinct().Count() < eventNumber)
            {
                throw new Exception("Solicitudes repetidas.");
            }

            List<Recorded<Notification<EventObject>>> rs = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rsCreated)
            {
                rs.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rs.Distinct().Count() < eventNumber)
            {
                throw new Exception("Respuestas repetidas.");
            }

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(rq.ToArray());
            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(rs.ToArray());

            #endregion Inputs creation

            #region Run test

            long maxTimeLeft = rqCreated.Max(x => x.Item2);
            long maxTimeRight = rsCreated.Max(x => x.Item2);
            long maxTime = maxTimeLeft > maxTimeRight ? maxTimeLeft : maxTimeRight;

            Stopwatch swJoin = Stopwatch.StartNew();

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2, printLog, debugMode, measureElapsedTime)
                            .Select(x =>
                            {
                                var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                                var b1 = a.GetType().GetProperty("c1");
                                var b2 = a.GetType().GetProperty("c2");
                                var b3 = a.GetType().GetProperty("c3");
                                var b4 = a.GetType().GetProperty("c4");
                                var b5 = a.GetType().GetProperty("ts1");
                                var b6 = a.GetType().GetProperty("ts2");
                                var b7 = a.GetType().GetProperty("o1");
                                var b8 = a.GetType().GetProperty("o2");
                                var b9 = a.GetType().GetProperty("o3");
                                return (object)(new
                                {
                                    o1 = b7.GetValue(a),
                                    o2 = b8.GetValue(a),
                                    o3 = b9.GetValue(a),
                                    ts1 = b5.GetValue(a),
                                    ts2 = b6.GetValue(a),
                                    c1 = b1.GetValue(a),
                                    c2 = b2.GetValue(a),
                                    c3 = b3.GetValue(a),
                                    c4 = b4.GetValue(a)
                                });
                            });
                }
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , maxTime + TimeSpan.FromSeconds(10).Ticks // tienen que ser mayor que el límite máximo definido para el envio de eventos, "maxLimitTimeTest" del constructor de la clase LoadTestsHelper
                );

            swJoin.Stop();
            TimeSpan tiempoDelJoin = swJoin.Elapsed;

            #endregion Run test

            #region Extract information from results

            Tuple<string, string, string, string, string, string, TimeSpan>[] actualResults = results.Messages
                .Select<Recorded<Notification<object>>, Tuple<string, string, string, string, string, string, TimeSpan>>(x =>
                {
                    dynamic rAux = ((dynamic)x.Value.Value);
                    return Tuple.Create<string, string, string, string, string, string, TimeSpan>(rAux.c1, rAux.c2, ((DateTime?)rAux.ts1).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.c3, rAux.c4, ((DateTime?)rAux.ts2).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.o1);
                })
                .ToArray();

            //helper.UpdateExpectedEventsMatchedFlag(actualResults);

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciasActualResults = new List<Tuple<string, string, string, string, bool>>();
            actualResults.ForEach(x =>
            {
                Tuple<string, string, string, string, bool> aux = expectedResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item3 == x.Item4 && y.Item4 == x.Item5);

                if (aux != null)
                {
                    expectedResultsUpdated.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciasActualResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item4, x.Item5, false));
                }
            });

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated2 = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciaExpectedResults = new List<Tuple<string, string, string, string, bool>>();
            expectedResults.ForEach(x =>
            {
                Tuple<string, string, string, string, string, string, TimeSpan> aux = actualResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item4 == x.Item3 && y.Item5 == x.Item4);

                if (aux != null)
                {
                    expectedResultsUpdated2.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciaExpectedResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4, false));
                }
            });

            decimal exactitudAlcanzada = ((decimal)(expectedResultsUpdated.Count() * 100)) / expectedResults.Count();

            #endregion Extract information from results

            #region Report generation from result information

            if (expectedResultsUpdated.Count != expectedResultsUpdated2.Count)
            {
                Assert.Fail("Falsos positivos entre los eventos resultantes obtenidos.");
            }

            string premisas = $"PrintLog: {printLog} \nDebugMode: {debugMode} \nMeasureElapsedTime: {measureElapsedTime} \nTolerancia: {tolerance} \nNumero de eventos: {eventNumber} eventos \nLimite superior ocurrencia de eventos: {limiteSuperiorOcurrenciaEventos} ms " +
                                $"\nPorcentaje de timeouts: {timeoutPercentage} % \nTimeout: {timeout} ms \nTimestamp condición en where: {whereDifference} ms \nEvaluar eventos coincidentes: {evaluateMatchedEvents} " +
                                $"\nBuffer actual: {System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]} " +
                                $"\nTamaño máximo del buffer: {System.Configuration.ConfigurationManager.AppSettings["MaxWindowSize"]}";

            string report = $"{premisas} " +
                                $"\n\nDuración de la prueba: {tiempoDelJoin} \n" +
                                $"Resultados actuales: {actualResults.Count()} eventos \n" +
                                $"Resultados esperados: {expectedResults.Count()} eventos \n" +
                                $"Coincidencias: {expectedResultsUpdated.Count()} eventos \n" +
                                $"Diferencias entre resultados esperados y actuales: {diferenciaExpectedResults.Count} eventos \n" +
                                $"Diferencias entre resultados actuales y esperados: {diferenciasActualResults.Count} eventos \n" +
                                $"Exactitud alcanzada: {exactitudAlcanzada} % \n";

            if (expectedResults.Where(x => x.Item5 == false).Count() > 0)
            {
                if (exactitudAlcanzada == 100)
                {
                    return;
                }
                if (exactitudAlcanzada < (100 - tolerance))
                {
                    Assert.Fail("Tolerancia no alcanzada.\n" + report);
                }
                else
                {
                    Assert.Inconclusive("Number of expected results is differ from number of actual results.\n" + report);
                }
            }

            #endregion  Report generation from result information
        }

        [TestMethod]
        public void CustomLoadTest4()
        {
            #region Compiler

            string eql = "cross " +
                                "JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#0.#0 == \"0100\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#0.#0 == \"0110\" " +
                                "ON t1.@event.Message.#1.#0 == t2.@event.Message.#1.#0 and t1.@event.Message.#1.#1 == t2.@event.Message.#1.#1 " +
                                "TIMEOUT '00:00:04' " +
                                "WHERE  isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') <= '00:00:01' " +
                                "SELECT isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(t1.@event.SourceTimestamp, '01/01/2016') as o1, " +
                                        "1 as o2, " +
                                        "isnull(t2.@event.SourceTimestamp, '01/01/2017') - isnull(null, '01/01/2016') as o3, " +
                                        "t1.@event.Message.#1.#0 as c1, t1.@event.Message.#1.#1 as c2, isnull(t1.@event.SourceTimestamp, '01/01/2016') as ts1, " +
                                        "t2.@event.Message.#1.#0 as c3, t2.@event.Message.#1.#1 as c4, isnull(t2.@event.SourceTimestamp, '01/01/2017') as ts2 ";
            
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            
            #endregion Compiler
                        
            #region parameters

            // contexto
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;

            // para la creación de eventos
            decimal tolerance = 0.5M;
            int eventNumber = 10;
            int limiteSuperiorOcurrenciaEventos = 10000;
            int timeoutPercentage = 15;
            int timeout = 4000;
            int whereDifference = 1000;
            bool evaluateMatchedEvents = true;

            #endregion parameters

            #region Creation of events

            LoadTestsHelper helper = new LoadTestsHelper(eventNumber, timeout, whereDifference, limiteSuperiorOcurrenciaEventos, timeoutPercentage, evaluateMatchedEvents);
            Tuple<Tuple<EventObject, long>[], Tuple<EventObject, long>[], Tuple<string, string, string, string, bool>[]> ltEvents = helper.CreateEvents(JoinTypeEnum.Cross);

            Tuple<EventObject, long>[] rqCreated = ltEvents.Item1;
            Tuple<EventObject, long>[] rsCreated = ltEvents.Item2;
            Tuple<string, string, string, string, bool>[] expectedResults = ltEvents.Item3;

            #endregion Creation of events

            #region Print created events

            int countLeft = 0;
            int countRight = 0;
            rqCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countLeft++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            System.Diagnostics.Debug.WriteLine("----------------------------------");

            rsCreated.ForEach(x =>
            {
                System.Diagnostics.Debug.WriteLine($"{countRight++} - {x.Item1.SourceTimestamp.ToString("hh:mm:ss.ffff")} [{x.Item1.Message[1][0].Value} - {x.Item1.Message[1][1].Value}] {TimeSpan.FromTicks(x.Item2)}");
            });

            #endregion Print created events

            #region Inputs creation

            List<Recorded<Notification<EventObject>>> rq = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rqCreated)
            {
                rq.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rq.Distinct().Count() < eventNumber)
            {
                throw new Exception("Solicitudes repetidas.");
            }

            List<Recorded<Notification<EventObject>>> rs = new List<Recorded<Notification<EventObject>>>();
            foreach (Tuple<EventObject, long> t in rsCreated)
            {
                rs.Add(OnNext<EventObject>(t.Item2, t.Item1));
            }

            if (rs.Distinct().Count() < eventNumber)
            {
                throw new Exception("Respuestas repetidas.");
            }

            ITestableObservable<EventObject> input1 = dsf.TestScheduler.CreateHotObservable(rq.ToArray());
            ITestableObservable<EventObject> input2 = dsf.TestScheduler.CreateHotObservable(rs.ToArray());

            #endregion Inputs creation

            #region Run test

            long maxTimeLeft = rqCreated.Max(x => x.Item2);
            long maxTimeRight = rsCreated.Max(x => x.Item2);
            long maxTime = maxTimeLeft > maxTimeRight ? maxTimeLeft : maxTimeRight;

            Stopwatch swJoin = Stopwatch.StartNew();

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () =>
                {
                    return this.Process(eql, dsf, input1, input2, printLog, debugMode, measureElapsedTime)
                            .Select(x =>
                            {
                                var a = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
                                var b1 = a.GetType().GetProperty("c1");
                                var b2 = a.GetType().GetProperty("c2");
                                var b3 = a.GetType().GetProperty("c3");
                                var b4 = a.GetType().GetProperty("c4");
                                var b5 = a.GetType().GetProperty("ts1");
                                var b6 = a.GetType().GetProperty("ts2");
                                var b7 = a.GetType().GetProperty("o1");
                                var b8 = a.GetType().GetProperty("o2");
                                var b9 = a.GetType().GetProperty("o3");
                                return (object)(new
                                {
                                    o1 = b7.GetValue(a),
                                    o2 = b8.GetValue(a),
                                    o3 = b9.GetValue(a),
                                    ts1 = b5.GetValue(a),
                                    ts2 = b6.GetValue(a),
                                    c1 = b1.GetValue(a),
                                    c2 = b2.GetValue(a),
                                    c3 = b3.GetValue(a),
                                    c4 = b4.GetValue(a)
                                });
                            });
                }
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , 0 // tienen que ser siempre 0 porque el límite inferior del random es 1
                , maxTime + TimeSpan.FromSeconds(10).Ticks // tienen que ser mayor que el límite máximo definido para el envio de eventos, "maxLimitTimeTest" del constructor de la clase LoadTestsHelper
                );

            swJoin.Stop();
            TimeSpan tiempoDelJoin = swJoin.Elapsed;

            #endregion Run test

            #region Extract information from results

            Tuple<string, string, string, string, string, string, TimeSpan>[] actualResults = results.Messages
                .Select<Recorded<Notification<object>>, Tuple<string, string, string, string, string, string, TimeSpan>>(x =>
                {
                    dynamic rAux = ((dynamic)x.Value.Value);
                    return Tuple.Create<string, string, string, string, string, string, TimeSpan>(rAux.c1, rAux.c2, ((DateTime?)rAux.ts1).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.c3, rAux.c4, ((DateTime?)rAux.ts2).Value.ToString("yyyy/MM/dd hh:mm:ss.ffff"), rAux.o1);
                })
                .ToArray();

            //helper.UpdateExpectedEventsMatchedFlag(actualResults);

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciasActualResults = new List<Tuple<string, string, string, string, bool>>();
            actualResults.ForEach(x =>
            {
                Tuple<string, string, string, string, bool> aux = expectedResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item3 == x.Item4 && y.Item4 == x.Item5);

                if (aux != null)
                {
                    expectedResultsUpdated.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciasActualResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item4, x.Item5, false));
                }
            });

            List<Tuple<string, string, string, string, bool>> expectedResultsUpdated2 = new List<Tuple<string, string, string, string, bool>>();
            List<Tuple<string, string, string, string, bool>> diferenciaExpectedResults = new List<Tuple<string, string, string, string, bool>>();
            expectedResults.ForEach(x =>
            {
                Tuple<string, string, string, string, string, string, TimeSpan> aux = actualResults.FirstOrDefault(y => y.Item1 == x.Item1 && y.Item2 == x.Item2 && y.Item4 == x.Item3 && y.Item5 == x.Item4);

                if (aux != null)
                {
                    expectedResultsUpdated2.Add(Tuple.Create(aux.Item1, aux.Item2, aux.Item3, aux.Item4, true));
                }
                else
                {
                    diferenciaExpectedResults.Add(Tuple.Create(x.Item1, x.Item2, x.Item3, x.Item4, false));
                }
            });

            decimal exactitudAlcanzada = ((decimal)(expectedResultsUpdated.Count() * 100)) / expectedResults.Count();

            #endregion Extract information from results

            #region Report generation from result information

            if (expectedResultsUpdated.Count != expectedResultsUpdated2.Count)
            {
                Assert.Fail("Falsos positivos entre los eventos resultantes obtenidos.");
            }

            string premisas = $"PrintLog: {printLog} \nDebugMode: {debugMode} \nMeasureElapsedTime: {measureElapsedTime} \nTolerancia: {tolerance} \nNumero de eventos: {eventNumber} eventos \nLimite superior ocurrencia de eventos: {limiteSuperiorOcurrenciaEventos} ms " +
                                $"\nPorcentaje de timeouts: {timeoutPercentage} % \nTimeout: {timeout} ms \nTimestamp condición en where: {whereDifference} ms \nEvaluar eventos coincidentes: {evaluateMatchedEvents} " +
                                $"\nBuffer actual: {System.Configuration.ConfigurationManager.AppSettings["bufferSizeOfJoinSources"]} " +
                                $"\nTamaño máximo del buffer: {System.Configuration.ConfigurationManager.AppSettings["MaxWindowSize"]}";

            string report = $"{premisas} " +
                                $"\n\nDuración de la prueba: {tiempoDelJoin} \n" +
                                $"Resultados actuales: {actualResults.Count()} eventos \n" +
                                $"Resultados esperados: {expectedResults.Count()} eventos \n" +
                                $"Coincidencias: {expectedResultsUpdated.Count()} eventos \n" +
                                $"Diferencias entre resultados esperados y actuales: {diferenciaExpectedResults.Count} eventos \n" +
                                $"Diferencias entre resultados actuales y esperados: {diferenciasActualResults.Count} eventos \n" +
                                $"Exactitud alcanzada: {exactitudAlcanzada} % \n";

            if (expectedResults.Where(x => x.Item5 == false).Count() > 0)
            {
                if (exactitudAlcanzada == 100)
                {
                    return;
                }
                if (exactitudAlcanzada < (100 - tolerance))
                {
                    Assert.Fail("Tolerancia no alcanzada.\n" + report);
                }
                else
                {
                    Assert.Inconclusive("Number of expected results is differ from number of actual results.\n" + report);
                }
            }

            #endregion  Report generation from result information
        }

        #endregion Custom load tests        
    }
}
