using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive;
using Integra.Space.Compiler;
using System.Reflection.Emit;
using System.Reflection;
using Ninject;
using Integra.Space.LanguageUnitTests.TestObject;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class TemporalStreamsTests
    {
        private CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGeneratorConfiguration config = new CodeGeneratorConfiguration(
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

        private IObservable<object> Process(string eql, DefaultSchedulerFactory dsf, ITestableObservable<TestObject1> input)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);

            FakePipeline fp = new FakePipeline();
            Delegate d = fp.ProcessWithCommandParser<TestObject1>(context, eql, new TestRuleValidator());

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
                                                                "SourceParaPruebas1",
                                                                "MessageType == \"0100\"",
                                                                "MessageType",
                                                                "apply duration of '00:00:00:01'");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(TimeSpan.FromMilliseconds(100).Ticks, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(TimeSpan.FromSeconds(2).Ticks, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(TimeSpan.FromSeconds(3).Ticks, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(TimeSpan.FromSeconds(5).Ticks, Notification.CreateOnCompleted<TestObject1>())
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
                                                                "SourceParaPruebas1",
                                                                "MessageType == \"0100\"",
                                                                "MessageType",
                                                                "apply repetition of 1");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(TimeSpan.FromMilliseconds(100).Ticks, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(TimeSpan.FromSeconds(2).Ticks, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(TimeSpan.FromSeconds(3).Ticks, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(TimeSpan.FromSeconds(5).Ticks, Notification.CreateOnCompleted<TestObject1>())
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
