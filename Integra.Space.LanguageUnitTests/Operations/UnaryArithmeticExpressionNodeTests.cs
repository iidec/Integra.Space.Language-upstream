using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using Integra.Space.Language.Runtime;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class UnaryArithmeticExpressionNodeTests
    {
        private IObservable<object> Process(string eql, DefaultSchedulerFactory dsf, ITestableObservable<EventObject> input)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            CompilerConfiguration context = new CompilerConfiguration() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = true };

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.Process(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            return ((IObservable<object>)result.Invoke(queryObject, new object[] { input.AsObservable(), dsf.TestScheduler }));
        }

        [TestMethod]
        public void UnaryNegativeInteger()
        {
            string eql = "from SpaceObservable1 select -1 as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<int> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<int>>[] {
                    new Recorded<Notification<int>>(100, Notification.CreateOnNext(-1)),
                    new Recorded<Notification<int>>(200, Notification.CreateOnCompleted<int>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void UnaryNegativeDouble()
        {
            string eql = "from SpaceObservable1 select -10.21 as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<double> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => double.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<double>>[] {
                    new Recorded<Notification<double>>(100, Notification.CreateOnNext(-10.21)),
                    new Recorded<Notification<double>>(200, Notification.CreateOnCompleted<double>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void UnaryNegativeDecimal()
        {
            string eql = "from SpaceObservable1 select -1m as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<decimal> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<decimal>>[] {
                    new Recorded<Notification<decimal>>(100, Notification.CreateOnNext(-1m)),
                    new Recorded<Notification<decimal>>(200, Notification.CreateOnCompleted<decimal>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
    }
}
