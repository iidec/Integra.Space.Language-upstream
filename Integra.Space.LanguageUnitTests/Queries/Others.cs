﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Collections.Generic;
using Integra.Space.Language.Runtime;
using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class Others
    {
        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosProyeccionMixta()
        {
            EQLPublicParser parser = new EQLPublicParser(
                "from SpaceObservable1 apply window of '00:00:01' select sum((decimal)@event.Message.#1.#4) as suma, @event.Message.#1.#4 as monto, \"campoXX\" as campo"
                                                                                            );
            List<PlanNode> plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());
            
            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        suma1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        suma2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { suma1 = (decimal)3, monto1 = (decimal)1, campo1 = "campoXX", suma2 = (decimal)3, monto2 = (decimal)1, campo2 = "campoXX" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosProyeccionMixta()
        {
            EQLPublicParser parser = new EQLPublicParser(
                "from SpaceObservable1 where 1 == 1 apply window of '00:00:01' select sum((decimal)@event.Message.#1.#4) as suma, @event.Message.#1.#4 as monto, \"campoXX\" as campo"
                                                                                            );
            List<PlanNode> plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());
            
            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        suma1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        suma2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { suma1 = (decimal)3, monto1 = (decimal)1, campo1 = "campoXX", suma2 = (decimal)3, monto2 = (decimal)1, campo2 = "campoXX" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventosOrderByDescProyeccionMixta()
        {
            EQLPublicParser parser = new EQLPublicParser(
                "from SpaceObservable1 apply window of '00:00:01' select sum((decimal)@event.Message.#1.#4) as suma, @event.Message.#1.#4 as monto, \"campoXX\" as campo order by suma"
                                                                                            );
            List<PlanNode> plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());
            
            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        suma1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        suma2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { suma1 = (decimal)3, monto1 = (decimal)1, campo1 = "campoXX", suma2 = (decimal)3, monto2 = (decimal)1, campo2 = "campoXX" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowSelectDosEventosOrderByDescProyeccionMixta()
        {
            EQLPublicParser parser = new EQLPublicParser(
                "from SpaceObservable1 where 1 == 1 apply window of '00:00:01' select sum((decimal)@event.Message.#1.#4) as suma, @event.Message.#1.#4 as monto, \"campoXX\" as campo order by suma"
                                                                                            );
            List<PlanNode> plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());
            
            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        suma1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        suma2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        monto2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        campo2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { suma1 = (decimal)3, monto1 = (decimal)1, campo1 = "campoXX", suma2 = (decimal)3, monto2 = (decimal)1, campo2 = "campoXX" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowSelectDosEventos()
        {
            EQLPublicParser parser = new EQLPublicParser(
                string.Format("from {0} apply window of {2} select {3} as monto",
                                                                                            "SpaceObservable1",
                                                                                            "@event.Message.#0.MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos EventObject                                                                                        
                                                                                            "(decimal)@event.Message.#1.#4")
                                                                                            );
            List<PlanNode> plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());
            
            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        Resultado2 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString())
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
        public void ConsultaSelect()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select @event.Message.Body.#43 as campo1");
            List<PlanNode> plan = parser.Evaluate();

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = dsf });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());
            
            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => result(input.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("campo1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(100, Notification.CreateOnNext((object)(new { Resultado1 = "Shell El Rodeo1GUATEMALA    GT" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
    }
}