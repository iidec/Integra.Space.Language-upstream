using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using Integra.Space.Language.Runtime;
using System.Reactive;
using System.Reactive.Linq;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class UnaryArithmeticExpressionNodeTests
    {
        [TestMethod]
        public void UnaryNegativeInteger()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select -1 as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<int> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select -10.21 as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<double> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => double.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select -1m as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<decimal> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
