using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class ArithmeticExpressionNodeTests
    {
        [TestMethod]
        public void RestaPositivosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 - 1 == 0 select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaNegativosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where -1 - -1 == 0 select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaPositivoNegativoIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 - -1 == 2 select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaNegativoPositivoIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where -1 - 1 == -2 select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaHourFunctionIgualdadWithoutHour()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where hour('02/01/2014') - hour('01/01/2014') == 0 select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaHourFunctionIgualdadWithHour()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where hour('02/01/2014 12:11:10') - hour('01/01/2014 11:11:10') == 1 select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaYearFunctionIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where year('02/03/2015') - year('01/01/2014') == 1 select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
    }
}
