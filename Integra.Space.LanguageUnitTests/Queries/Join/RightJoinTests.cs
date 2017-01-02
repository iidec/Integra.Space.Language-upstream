using Integra.Space.Compiler;
using Integra.Space.Database;
using Integra.Space.Language;
using Integra.Space.LanguageUnitTests.TestObject;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class RightJoinTests : ReactiveTest
    {
        private CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            Login login = new SpaceDbContext().Logins.First();
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGeneratorConfiguration config = new CodeGeneratorConfiguration(
                login,
                dsf,
                AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Test"), AssemblyBuilderAccess.RunAndSave),
                kernel,
                printLog: printLog,
                debugMode: debugMode,
                measureElapsedTime: measureElapsedTime,
                isTestMode: isTestMode
                );

            return config;
        }

        private IObservable<object> Process(string eql, DefaultSchedulerFactory dsf, ITestableObservable<TestObject3> input1, ITestableObservable<TestObject3> input2, bool printLog = false, bool debugMode = false, bool measureElapsedTime = false)
        {
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.Process(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            return ((IObservable<object>)result.Invoke(queryObject, new object[] { input1.AsObservable(), input2.AsObservable(), dsf.TestScheduler }));
        }

        #region On condition true

        [TestMethod]
        public void RightJoinTest_OnTrue_1()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
        public void RightJoinTest_OnTrue_2()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(2).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
        public void RightJoinTest_OnTrue_3()
        {
            string eql = "Right " +
                                 "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                 "WITH SourceParaPruebas3 as t2 " +
                                 "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                 "TIMEOUT '00:00:02' " +
                                 "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(2).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
        public void RightJoinTest_OnTrue_4()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:20' " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(2).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
        public void RightJoinTest_OnTrue_5()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 " +
                                "WITH SourceParaPruebas3 as t2 " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(3).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
        public void RightJoinMultipleEventsTest_OnTrue()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 " + //WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 " + //WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(6).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(8).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(10).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(6).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(8).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(10).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(12).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(130000001).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void RightJoinTest_OnTrue_6()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode and t1.Track2Data == t2.Track2Data " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
        public void RightJoinMultipleEventsTest_OnTrue_2()
        {
            string eql = "right " +
                                "JOIN SourceParaPruebas3 as t1 " + //WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 " + //WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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

        #endregion On condition true

        #region On condition false

        [TestMethod]
        public void RightJoinTest_OnFalse_1()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.Track2Data " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void RightJoinTest_OnFalse_2()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.Track2Data " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(2).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void RightJoinTest_OnFalse_3()
        {
            string eql = "Right " +
                                 "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                 "WITH SourceParaPruebas3 as t2 " +
                                 "ON t1.AcquiringInstitutionIdentificationCode == t2.Track2Data " +
                                 "TIMEOUT '00:00:02' " +
                                 "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(2).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void RightJoinTest_OnFalse_4()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.Track2Data " +
                                "TIMEOUT '00:00:01' " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(2).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(40000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void RightJoinTest_OnFalse_5()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 " +
                                "WITH SourceParaPruebas3 as t2 " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.Track2Data " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(3).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(60000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        [TestMethod]
        public void RightJoinMultipleEventsTest_OnFalse()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 " + //WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 " + //WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                                                 //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.Track2Data " +
                                "TIMEOUT '00:00:01' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(6).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(8).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(10).Ticks, new TestObject3())
                , OnNext(TimeSpan.FromSeconds(12).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext(TimeSpan.FromSeconds(4).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(6).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(8).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
                , OnNext(TimeSpan.FromSeconds(10).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(50000001).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(70000001).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(90000001).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                    OnNext(new TimeSpan(110000001).Ticks,(object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" })),
                }, results.Messages);
        }

        [TestMethod]
        public void RightJoinTest_OnFalse_6()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.Track2Data and t1.Track2Data == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject3> input1 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(4).Ticks, new TestObject3())
                );

            ITestableObservable<TestObject3> input2 = dsf.TestScheduler.CreateHotObservable(
                OnNext<TestObject3>(TimeSpan.FromSeconds(3).Ticks, new TestObject3(primaryAccountNumber: "9999941616073663_2", processingCode: "302000", transactionAmount: 1000m))
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
                    OnNext(new TimeSpan(50000001).Ticks, (object)(new { c1 = (object)null, c2 = (object)"9999941616073663_2" }))
                }, results.Messages);
        }

        #endregion On condition false

        #region Errors

        #region Constants in on condition

        [TestMethod]
        public void RightJoinTest_ConstantInOnCondition_1()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON \"constant\" == t1.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió una constante en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_ConstantInOnCondition_2()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == \"constant\" " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un valor no constante en la condición del primer WHERE.");
        }

        #endregion Constants in on condition

        #region Invalid comparative operator

        [TestMethod]
        public void RightJoinTest_NotEqualThanInOnCondition()
        {
            string condition = "t1.AcquiringInstitutionIdentificationCode != t2.AcquiringInstitutionIdentificationCode";
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_LessThanInOnCondition()
        {
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode < t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_LessThanOrEqualsInOnCondition()
        {
            string condition = "t1.AcquiringInstitutionIdentificationCode <= t2.AcquiringInstitutionIdentificationCode";
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_GreaterThanOrEqualsInOnCondition()
        {
            string condition = "t1.AcquiringInstitutionIdentificationCode >= t2.AcquiringInstitutionIdentificationCode";
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_GreaterThanInOnCondition()
        {
            string condition = "t1.AcquiringInstitutionIdentificationCode > t2.AcquiringInstitutionIdentificationCode";
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_LikeInOnCondition()
        {
            string condition = "t1.AcquiringInstitutionIdentificationCode like \"491381\"";
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_OrInOnCondition()
        {
            string condition = "t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode or t1.Track2Data == t2.Track2Data";
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON " + condition + " " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_NotInOnCondition()
        {
            string condition = "not(t1.Track2Data == t2.Track2Data)";
            string eql = "Right " +
                                "JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                $"ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode and {condition} " +
                                "TIMEOUT '00:00:02' " +
                                //"WHERE  t1.@event.Message.#1.#43 == \"Shell El RodeoGUATEMALA    GT\" " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.IsTrue(e is Integra.Space.Language.Exceptions.SyntaxException);
                return;
            }

            Assert.Fail("Error: se permitió un operador de comparación inválido en la condición ON.");
        }

        [TestMethod]
        public void RightJoinTest_MixedSources_1()
        {
            string instruction = "t2.PrimaryAccountNumber";
            string eql = "Right " +
                                $"JOIN SourceParaPruebas3 as t1 WHERE t1.PrimaryAccountNumber == {instruction} " +
                                "WITH SourceParaPruebas3 as t2 WHERE t2.PrimaryAccountNumber == \"9999941616073663_2\" " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.PrimaryAccountNumber as c1, t2.PrimaryAccountNumber as c2 into SourceXYZ ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
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
        public void RightJoinTest_MixedSources_2()
        {
            string instruction = "t1.PrimaryAccountNumber";
            string eql = "Right " +
                                "JOIN SourceParaPruebas1 as t1 WHERE t1.PrimaryAccountNumber == \"9999941616073663_1\" " +
                                $"WITH SourceParaPruebas1 as t2 WHERE t2.PrimaryAccountNumber == {instruction} " +
                                "ON t1.AcquiringInstitutionIdentificationCode == t2.AcquiringInstitutionIdentificationCode " +
                                "TIMEOUT '00:00:02' " +
                                "SELECT t1.PrimaryAccountNumber as c1 into SourceXYZ"; // , t2.PrimaryAccountNumber as c2 

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            try
            {
                QueryParser parser = new QueryParser(eql);
                PlanNode plan = parser.Evaluate().Item1;
                CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
                FakePipeline fp = new FakePipeline();
                Assembly assembly = fp.Process(context, eql, dsf);
            }
            catch (Exception e)
            {
                Assert.AreEqual<string>(string.Format("CompilationException: Line: {0}, Column: {1}, Instruction: {2}, Error: {3}", 0, 153, instruction, Language.Resources.COMPILATION_ERRORS.CE77("t1")), e.InnerException.InnerException.Message);
                return;
            }

            Assert.Fail("Error: fuente invalida permitida en la condición WHERE de la segunda fuente.");
        }

        #endregion Invalid comparative operator

        #endregion Errors
    }
}
