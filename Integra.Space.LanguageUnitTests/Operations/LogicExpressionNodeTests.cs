using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class LogicExpressionNodeTests
    {
        private IObservable<object> Process(string eql, DefaultSchedulerFactory dsf, ITestableObservable<EventObject> input)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            CompileContext context = new CompileContext() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = true };

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.Process(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            object queryObject = Activator.CreateInstance(types.Last());
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            return ((IObservable<object>)result.Invoke(queryObject, new object[] { input.AsQbservable(), dsf.TestScheduler }));
        }

        [TestMethod]
        public void EventosIgualdadAndEventosIgualdad()
        {
            string eql = "from SpaceObservable1 where @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] and @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
            string eql = "from SpaceObservable1 where not (@event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] or @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"]) select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
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
            string eql = "from SpaceObservable1 where @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] or @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
            string eql = "from SpaceObservable1 where not (@event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] or @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"]) select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
            string eql = "from SpaceObservable1 where true and true select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );
           
            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
            string eql = "from SpaceObservable1 where false and false select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
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
            string eql = "from SpaceObservable1 where true and false select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
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
            string eql = "from SpaceObservable1 where false and true select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(x.GetType().GetProperty("resultado").GetValue(x).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void TrueOrTrue()
        {
            string eql = "from SpaceObservable1 where true or true select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(100, Notification.CreateOnNext<bool?>(true)),
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void TrueOrFalse()
        {
            string eql = "from SpaceObservable1 where true or false select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();


            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(100, Notification.CreateOnNext<bool?>(true)),
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void FalseOrTrue()
        {
            string eql = "from SpaceObservable1 where false or true select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(100, Notification.CreateOnNext<bool?>(true)),
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void FalseOrFalse()
        {
            string eql = "from SpaceObservable1 where false or false select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotFalseOrFalse()
        {
            string eql = "from SpaceObservable1 where not(false or false) select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(100, Notification.CreateOnNext<bool?>(true)),
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotTrueOrTrue()
        {
            string eql = "from SpaceObservable1 where not(true or true) select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotFalseAndFalse()
        {
            string eql = "from SpaceObservable1 where not(false and false) select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(100, Notification.CreateOnNext<bool?>(true)),
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotTrueAndTrue()
        {
            string eql = "from SpaceObservable1 where not(true and true) select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotTrueOrFalseAndTrue1()
        {
            string eql = "from SpaceObservable1 where not (true) or false and true select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotTrueOrFalseAndTrue2()
        {
            string eql = "from SpaceObservable1 where not (true or false and true) select true as resultado";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool?> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => (bool?)bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool?>>[] {
                    new Recorded<Notification<bool?>>(200, Notification.CreateOnCompleted<bool?>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
    }
}
