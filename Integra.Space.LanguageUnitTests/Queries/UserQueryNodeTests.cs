using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Reactive.Linq;
using Integra.Space.Event;
using Microsoft.Reactive.Testing;
using System.Reactive;
using Integra.Space.Language.Runtime;
using System.Collections.Generic;
using System.Dynamic;
using Integra.Space.Language.Exceptions;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class UserQueryNodeTests
    {
        [TestMethod]
        public void ConsultaProyeccionCampoNuloConWhere()
        {
            EQLPublicParser parser = new EQLPublicParser(
                    string.Format("from {0} where {1} select {2} as CampoNulo",
                                                                "SpaceObservable1",
                                                                "@event.Message.#0.MessageType == \"0100\"",
                                                                "@event.Message.#0.[\"Campo que no existe\"]")
                                                                );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        CampoNulo = x.First().GetType().GetProperty("CampoNulo").GetValue(x.First())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(100, Notification.CreateOnNext((object)(new { CampoNulo = default(object) }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaProyeccionCampoNuloSinWhere()
        {
            EQLPublicParser parser = new EQLPublicParser(
                    string.Format("from {0} select {1} as CampoNulo",
                                                                "SpaceObservable1",
                                                                "@event.Message.#0.[\"Campo que no existe\"]")
                                                                );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        CampoNulo = x.First().GetType().GetProperty("CampoNulo").GetValue(x.First())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(100, Notification.CreateOnNext((object)(new { CampoNulo = default(object) }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByUnaLlaveYCount()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Contador",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1",
                                                                                            "key.grupo1",
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Contador = int.Parse(x.First().GetType().GetProperty("Contador").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "0100", Contador = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos1_0()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as Resultado",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(x.First().GetType().GetProperty("Resultado").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos1_1()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as Resultado",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "@event.Message.#0.MessageType")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = x.First().GetType().GetProperty("Resultado").GetValue(x.First()).ToString(),
                        Resultado2 = x.ElementAt(1).GetType().GetProperty("Resultado").GetValue(x.ElementAt(1)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = "0100", Resultado2 = "0100" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos1_2()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as Resultado1, {4} as Resultado2",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "@event.Message.#0.MessageType",
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = x.ElementAt(0).GetType().GetProperty("Resultado1").GetValue(x.ElementAt(0)).ToString(),
                        Resultado2 = int.Parse(x.ElementAt(0).GetType().GetProperty("Resultado2").GetValue(x.ElementAt(0)).ToString()),
                        Resultado3 = x.ElementAt(1).GetType().GetProperty("Resultado1").GetValue(x.ElementAt(1)).ToString(),
                        Resultado4 = int.Parse(x.ElementAt(1).GetType().GetProperty("Resultado2").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = "0100", Resultado2 = 2, Resultado3 = "0100", Resultado4 = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos1_4()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as Resultado",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "max((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(x.First().GetType().GetProperty("Resultado").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 1 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos1_5()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as Resultado",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "min((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(x.First().GetType().GetProperty("Resultado").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 1 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
        
        /**************************************************************************************************************************************************************/

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventosOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as monto order by desc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString()),
                        Resultado2 = decimal.Parse(x.ElementAt(1).GetType().GetProperty("monto").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 2m, Resultado2 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaSelectDiezEventosTop()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} select top 1 {3} as monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(100, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(100, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 100)
                });
        }

        [TestMethod]
        public void ConsultaWhereSelectDosEventosTop()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} select top 1 {3} as monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(100, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(100, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 100)
                });
        }
        
        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosTop()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select top 1 {3} as monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosTopOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select top 1 {3} as monto order by desc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosTopOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select top 1 {3} as monto order by asc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosTopOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select top 1 {3} as monto order by monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosTopOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto order by desc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosTopOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto order by asc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosTopOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto order by monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosTop()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumTopOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by desc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2GUATEMALA    GT", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumTopOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by asc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumTopOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumTop()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumTopOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by desc Sumatoria, Llave",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2GUATEMALA    GT", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumTopOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by asc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumTopOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectTopUnaLlaveYSum()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        /**************************************************************************************************************************************************************/
        
        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as monto order by desc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString()),
                        Resultado2 = decimal.Parse(x.ElementAt(1).GetType().GetProperty("monto").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 2m, Resultado2 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as monto order by asc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString()),
                        Resultado2 = decimal.Parse(x.ElementAt(1).GetType().GetProperty("monto").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m, Resultado2 = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as monto order by monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString()),
                        Resultado2 = decimal.Parse(x.ElementAt(1).GetType().GetProperty("monto").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m, Resultado2 = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select {3} as monto order by desc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString()),
                        Resultado2 = decimal.Parse(x.ElementAt(1).GetType().GetProperty("monto").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 2m, Resultado2 = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select {3} as monto order by asc monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString()),
                        Resultado2 = decimal.Parse(x.ElementAt(1).GetType().GetProperty("monto").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m, Resultado2 = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select {3} as monto order by monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(x.First().GetType().GetProperty("monto").GetValue(x.First()).ToString()),
                        Resultado2 = decimal.Parse(x.ElementAt(1).GetType().GetProperty("monto").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = 1m, Resultado2 = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by desc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2GUATEMALA    GT", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by asc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSum()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumOrderByDesc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by desc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2GUATEMALA    GT", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumOrderByAsc()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by asc Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumOrderBy()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSum()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#1.CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 1m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
        
        /**************************************************************************************************************************************************************/

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos2_0()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select {3} as Resultado",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(x.First().GetType().GetProperty("Resultado").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos2_1()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select {3} as Resultado",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "@event.Message.#0.MessageType")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = x.First().GetType().GetProperty("Resultado").GetValue(x.First()).ToString(),
                        Resultado2 = x.ElementAt(1).GetType().GetProperty("Resultado").GetValue(x.ElementAt(1)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = "0100", Resultado2 = "0100" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaSoloApplyWindowDosEventos2_2()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} select {3} as Resultado1, {4} as Resultado2",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "@event.Message.#0.MessageType",
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = x.ElementAt(0).GetType().GetProperty("Resultado1").GetValue(x.ElementAt(0)).ToString(),
                        Resultado2 = int.Parse(x.ElementAt(0).GetType().GetProperty("Resultado2").GetValue(x.ElementAt(0)).ToString()),
                        Resultado3 = x.ElementAt(1).GetType().GetProperty("Resultado1").GetValue(x.ElementAt(1)).ToString(),
                        Resultado4 = int.Parse(x.ElementAt(1).GetType().GetProperty("Resultado2").GetValue(x.ElementAt(1)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado1 = "0100", Resultado2 = 2, Resultado3 = "0100", Resultado4 = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByUnaLlaveYSum()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "0100", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByDosLlavesYCount()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave1, {5} as Llave2, {6} as Contador",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1, @event.Message.#1.PrimaryAccountNumber as grupo2",
                                                                                            "key.grupo1",
                                                                                            "grupo2",
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = x.First().GetType().GetProperty("Llave1").GetValue(x.First()).ToString(),
                        Llave2 = x.First().GetType().GetProperty("Llave2").GetValue(x.First()).ToString(),
                        Contador = int.Parse(x.First().GetType().GetProperty("Contador").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave1 = "0100", Llave2 = "9999941616073663", Contador = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByDosLlavesYSum()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave1, {5} as Llave2, {6} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1, @event.Message.#1.PrimaryAccountNumber as grupo2",
                                                                                            "key.grupo1",
                                                                                            "grupo2",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = x.First().GetType().GetProperty("Llave1").GetValue(x.First()).ToString(),
                        Llave2 = x.First().GetType().GetProperty("Llave2").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave1 = "0100", Llave2 = "9999941616073663", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByUnaLlaveYCountSinWhere()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {1} group by {2} select {3} as Llave, {4} as Contador",
                                                                                            "SpaceObservable1",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1",
                                                                                            "key.grupo1",
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.First().GetType().GetProperty("Llave").GetValue(x.First()).ToString(),
                        Contador = int.Parse(x.First().GetType().GetProperty("Contador").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "0100", Contador = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByUnaLlaveYSumSinWhere()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {1} group by {2} select {3} as Llave, {4} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1",
                                                                                            "key.grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.ElementAt(0).GetType().GetProperty("Llave").GetValue(x.ElementAt(0)).ToString(),
                        Sumatoria = decimal.Parse(x.ElementAt(0).GetType().GetProperty("Sumatoria").GetValue(x.ElementAt(0)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "0100", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByUnaLlaveYSumSinWhere2()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {1} group by {2} select {3} as Llave",
                                                                                            "SpaceObservable1",
                                                                                            "'00:00:10'",
                                                                                            "@event.Message.#0.#0 as grupo1",
                                                                                            "key.grupo1",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave = x.ElementAt(0).GetType().GetProperty("Llave").GetValue(x.ElementAt(0)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "0100" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByDosLlavesYCountSinWhere()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {1} group by {2} select {3} as Llave1, {4} as Llave2, {5} as Contador",
                                                                                            "SpaceObservable1",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1, @event.Message.#1.PrimaryAccountNumber as grupo2",
                                                                                            "key.grupo1",
                                                                                            "key.grupo2",
                                                                                            "count()")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = x.First().GetType().GetProperty("Llave1").GetValue(x.First()).ToString(),
                        Llave2 = x.First().GetType().GetProperty("Llave2").GetValue(x.First()).ToString(),
                        Contador = int.Parse(x.First().GetType().GetProperty("Contador").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave1 = "0100", Llave2 = "9999941616073663", Contador = 2 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaGroupByDosLlavesYSumSinWhere()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {1} group by {2} select {3} as Llave1, {4} as Llave2, {5} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1, @event.Message.#1.PrimaryAccountNumber as grupo2",
                                                                                            "key.grupo1",
                                                                                            "key.grupo2",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = x.First().GetType().GetProperty("Llave1").GetValue(x.First()).ToString(),
                        Llave2 = x.First().GetType().GetProperty("Llave2").GetValue(x.First()).ToString(),
                        Sumatoria = decimal.Parse(x.First().GetType().GetProperty("Sumatoria").GetValue(x.First()).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave1 = "0100", Llave2 = "9999941616073663", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaLlaveSinGroupBySinWhere()
        {
            try
            {
                // no es posible utilizar key sin un group by
                EQLPublicParser parser = new EQLPublicParser(
                    string.Format("from {0} select {1} as Llave",
                                                                "SpaceObservable1",
                                                                "key")
                                                                );
                List<PlanNode> plan = parser.Parse();

                ObservableConstructor te = new ObservableConstructor();
                Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

                Assert.Inconclusive();
            }
            catch (CompilationException e)
            {
                // prueba exitosa porque es un error poner key sin group by
            }
            catch (Exception e)
            {
                if (!(e.InnerException is CompilationException))
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void ConsultaLlaveSinGroupByConWhere()
        {
            try
            {
                // no es posible utilizar key sin un group by
                EQLPublicParser parser = new EQLPublicParser(
                    string.Format("from {0} where {1} select {2} as Llave",
                                                                "SpaceObservable1",
                                                                "@event.Message.#0.MessageType == \"0100\"",
                                                                "key")
                                                                );
                List<PlanNode> plan = parser.Parse();

                ObservableConstructor te = new ObservableConstructor();
                Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

                Assert.Inconclusive();
            }
            catch (CompilationException e)
            {
                // prueba exitosa porque es un error poner key sin group by
            }
            catch (Exception e)
            {
                if (!(e.InnerException is CompilationException))
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void ConsultaCountSinGroupBySinWhere()
        {
            try
            {
                // no es posible utilizar key sin un group by
                EQLPublicParser parser = new EQLPublicParser(
                    string.Format("from {0} select {1} as Llave",
                                                                "SpaceObservable1",
                                                                "count()")
                                                                );
                List<PlanNode> plan = parser.Parse();

                ObservableConstructor te = new ObservableConstructor();
                Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

                Assert.Inconclusive();
            }
            catch (CompilationException e)
            {
                // prueba exitosa porque es un error poner key sin group by
            }
            catch (Exception e)
            {
                if (!(e.InnerException is CompilationException))
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void ConsultaSumSinGroupBySinWhere()
        {
            try
            {
                // no es posible utilizar key sin un group by
                EQLPublicParser parser = new EQLPublicParser(
                    string.Format("from {0} select {1} as Llave",
                                                                "SpaceObservable1",
                                                                "sum((decimal)@event.Message.#1.TransactionAmount)")
                                                                );
                List<PlanNode> plan = parser.Parse();

                ObservableConstructor te = new ObservableConstructor();
                Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

                Assert.Inconclusive();
            }
            catch (CompilationException e)
            {
                // prueba exitosa porque es un error poner key sin group by
            }
            catch (Exception e)
            {
                if (!(e.InnerException is CompilationException))
                {
                    Assert.Fail();
                }
            }
        }

        /*
        [TestMethod]
        public void ConsultaGroupByUnaLlaveYSumErrorDeCasteoCampoNoNumerico()
        {
            // esta es para probar las excepciones 
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1",
                                                                                            "key.grupo1",
                                                                                            "sum((decimal)@event.Message.#1.CardAcceptorNameLocation)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                {
                    return x.First();
                }
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);


            // ReactiveAssert.AssertEqual<string>(new string[] { (results.Messages.First().Value.Exception).InnerException.InnerException.Message }, "Specified cast is not valid.");

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext<object>(new Exception("Specified cast is not valid."))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
        */

        [TestMethod]
        public void ConsultaGroupByUnaLlaveYSumCampoNoNumerico()
        {
            try
            {
                EQLPublicParser parser = new EQLPublicParser(
                    string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria",
                                                                                                "SpaceObservable1",
                                                                                                "@event.Message.#0.MessageType == \"0100\"",
                                                                                                "'00:00:00:01'",
                                                                                                "@event.Message.#0.MessageType as grupo1",
                                                                                                "key.grupo1",
                                                                                                "sum(@event.Message.#1.CardAcceptorNameLocation)")
                                                                                                );
                List<PlanNode> plan = parser.Parse();

                ObservableConstructor te = new ObservableConstructor();
                Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

                Assert.Inconclusive();
            }
            catch (CompilationException e)
            {
                // prueba exitosa porque es un error poner key sin group by
            }
            catch (Exception e)
            {
                if (!(e.InnerException is CompilationException))
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod]
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion1()
        {
            string command = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave1, {5} as Llave2, {6} as Contador, {7} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\" and @event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1, @event.Message.#1.PrimaryAccountNumber as grupo2",
                                                                                            "key.grupo1",
                                                                                            "key.grupo2",
                                                                                            "count()",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)");
            EQLPublicParser parser = new EQLPublicParser(command);
            List<PlanNode> plan = parser.Parse();

            Assert.AreEqual<string>(command, plan.First().NodeText);
        }

        [TestMethod]
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion2()
        {
            string command = string.Format("from {0} apply window of {1} group by {2} select {3} as Llave1, {4} as Llave2, {5} as Contador, {6} as Sumatoria",
                                                                                            "SpaceObservable1",
                                                                                            "'00:00:00:01'",
                                                                                            "@event.Message.#0.MessageType as grupo1, @event.Message.#1.PrimaryAccountNumber as grupo2",
                                                                                            "key.grupo1",
                                                                                            "key.grupo2",
                                                                                            "count()",
                                                                                            "sum((decimal)@event.Message.#1.TransactionAmount)");
            EQLPublicParser parser = new EQLPublicParser(command);
            List<PlanNode> plan = parser.Parse();

            Assert.AreEqual<string>(command, plan.First().NodeText);
        }

        [TestMethod]
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion3()
        {
            string command = string.Format("from {0} where {1} select {2} as PrimaryAccountNumber, {3} as CardAcceptorNameLocation, {4} as TransactionAmount, {5} as TransactionCurrencyCode",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\" and @event.Message.#0.MessageType == \"0100\"",
                                                                                            "@event.Message.#0.PrimaryAccountNumber",
                                                                                            "@event.Message.#0.CardAcceptorNameLocation",
                                                                                            "@event.Message.#0.TransactionAmount",
                                                                                            "@event.Message.#0.TransactionCurrencyCode");
            EQLPublicParser parser = new EQLPublicParser(command);
            List<PlanNode> plan = parser.Parse();

            Assert.AreEqual<string>(command, plan.First().NodeText);
        }

        [TestMethod]
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion4()
        {
            string command = string.Format("from {0} select {1} as PrimaryAccountNumber, {2} as CardAcceptorNameLocation, {3} as TransactionAmount, {4} as TransactionCurrencyCode",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.PrimaryAccountNumber",
                                                                                            "@event.Message.#0.CardAcceptorNameLocation",
                                                                                            "@event.Message.#0.TransactionAmount",
                                                                                            "@event.Message.#0.TransactionCurrencyCode");
            EQLPublicParser parser = new EQLPublicParser(command);
            List<PlanNode> plan = parser.Parse();

            Assert.AreEqual<string>(command, plan.First().NodeText);
        }
    }
}
