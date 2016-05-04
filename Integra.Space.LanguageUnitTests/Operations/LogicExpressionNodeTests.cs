using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive;
using Integra.Space.Language.Runtime;
using System.Collections.Generic;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class LogicExpressionNodeTests
    {
        [TestMethod]
        public void EventosIgualdadAndEventosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] and @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void EventosIgualdadAndEventosIgualdadFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not (@event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] or @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"]) select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void EventosIgualdadOrEventosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] or @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void EventosIgualdadOrEventosIgualdadFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not (@event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] or @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"]) select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void TrueAndTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where true and true select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());
            
            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );
           
            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void FalseAndFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where false and false select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void TrueAndFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where true and false select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void FalseAndTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where false and true select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void TrueOrTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where true or true select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void TrueOrFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where true or false select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void FalseOrTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where false or true select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void FalseOrFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where false or false select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotFalseOrFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not(false or false) select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void NotTrueOrTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not(true or true) select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotFalseAndFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not(false and false) select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
        public void NotTrueAndTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not(true and true) select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotTrueOrFalseAndTrue1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not (true) or false and true select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotTrueOrFalseAndTrue2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not (true or false and true) select true as resultado");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
    }
}
