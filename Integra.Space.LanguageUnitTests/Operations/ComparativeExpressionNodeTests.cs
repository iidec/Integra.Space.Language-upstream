using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language.ASTNodes.Operations;
using System.Globalization;
using Integra.Space.Language;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Reactive.Linq;
using Integra.Space.Language.Runtime;
using Integra.Space.Event;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class ComparativeExpressionNodeTests
    {
        [TestMethod()]
        public void IsNumericTypeFunctionTestTrue()
        {
            ComparativeExpressionNode comp = new ComparativeExpressionNode();

            decimal a = 1;
            bool result = comp.IsNumericType(a.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            float b = 1;
            result = comp.IsNumericType(b.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            double c = 1.1;
            result = comp.IsNumericType(c.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            int d = 1;
            result = comp.IsNumericType(d.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            Int16 e = 1;
            result = comp.IsNumericType(e.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            Int32 f = 1;
            result = comp.IsNumericType(f.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            Int64 g = 1;
            result = comp.IsNumericType(g.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            uint h = 1;
            result = comp.IsNumericType(h.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            long i = 1;
            result = comp.IsNumericType(i.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            ulong j = 1;
            result = comp.IsNumericType(j.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            short k = 1;
            result = comp.IsNumericType(k.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            ushort l = 1;
            result = comp.IsNumericType(l.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            byte m = 1;
            result = comp.IsNumericType(m.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            sbyte n = 1;
            result = comp.IsNumericType(n.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");

            object o = 1;
            result = comp.IsNumericType(o.GetType());
            Assert.IsTrue(result, "No tomó el numero como válido");
        }

        [TestMethod()]
        public void IsNumericTypeTestFalse()
        {
            ComparativeExpressionNode comp = new ComparativeExpressionNode();

            string a = "cadena";
            bool result = comp.IsNumericType(a.GetType());
            Assert.IsFalse(result, "Tomó una cadena como numero válido");

            bool b = true;
            result = comp.IsNumericType(b.GetType());
            Assert.IsFalse(result, "Tomó un booleano como numero válido");

            DateTime d;
            DateTime.TryParseExact("01/01/2014", "dd/MM/yyyy", null, DateTimeStyles.None, out d);
            result = comp.IsNumericType(d.GetType());
            Assert.IsFalse(result, "Tomó un datetime como numero válido");

            object e = "cadena";
            result = comp.IsNumericType(e.GetType());
            Assert.IsFalse(result, "Tomó un object cadena como numero válido");

            object f = true;
            result = comp.IsNumericType(f.GetType());
            Assert.IsFalse(result, "Tomó un object booleano como numero válido");

            DateTime g;
            DateTime.TryParseExact("01/01/2014", "dd/MM/yyyy", null, DateTimeStyles.None, out g);
            object h = g;
            result = comp.IsNumericType(h.GetType());
            Assert.IsFalse(result, "Tomó un object datetime como numero válido");
        }

        [TestMethod]
        public void PositivosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 == 1 select true as resultado");
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
        public void PositivosConSignoIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where +1 == +1 select true as resultado");
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
        public void NegativosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where -1 == -1 select true as resultado");
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
        public void BooleanosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where true == true select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, Scheduler = new TestSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<IEnumerable<object>>> result = te.Compile<IQbservable<EventObject>, IObservable<IEnumerable<object>>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateColdObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(2000, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<bool> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => bool.Parse(x.First().GetType().GetProperty("resultado").GetValue(x.First()).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 4000);
            
            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded <Notification<bool>>[] {
                    new Recorded<Notification<bool>>(150, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(2050, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 2050)
                });
        }

        [TestMethod]
        public void FechasIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '01/01/2014' == '01/01/2014' select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty });
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
        public void CadenasIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where \"a\" == \"a\" select true as resultado");
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
        public void CadenasIgualdadLeftFunction1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where left(\"ca\", 1) == \"c\" select true as resultado");
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
        public void CadenasIgualdadLeftFunction2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where left(\"xyzxxx\", 3) == \"xyz\" select true as resultado");
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
        public void CadenasIgualdadLeftFunction3()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where left((string)@event.Message.#1.CardAcceptorNameLocation, 5) == \"Shell\" select true as resultado");
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
        public void CadenasIgualdadRightFunction1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where right(\"ab\", 1) == \"b\" select true as resultado");
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
        public void CadenasIgualdadRightFunction2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where right(\"xxxabcd\", 4) == \"abcd\" select true as resultado");
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
        public void CadenasIgualdadRightFunction3()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where right((string)@event.Message.#1.CardAcceptorNameLocation, 2) == \"GT\" select true as resultado");
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
        public void CadenasIgualdadUpperFunction1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where upper(\"ca\") == \"CA\" select true as resultado");
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
        public void CadenasIgualdadUpperFunction2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where upper((string)@event.Message.#1.CardAcceptorNameLocation) == \"SHELL EL RODEO1GUATEMALA    GT\" select true as resultado");
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
        public void CadenasIgualdadLowerFunction1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where lower(\"CA\") == \"ca\" select true as resultado");
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
        public void CadenasIgualdadLowerFunction2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where lower((string)@event.Message.#1.CardAcceptorNameLocation) == \"shell el rodeo1guatemala    gt\" select true as resultado");
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
        public void NulosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where null == null select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty });
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
        public void EventoCampoInexistenteIgualdadNulo()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Body.#103.[\"Campo que no existe\"] == null select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty });
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
        public void NuloIgualdadEventoCampoInexistente()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where null == @event.Message.Body.#103.[\"Campo que no existe\"] select true as resultado");
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
        public void EventosIgualdad1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"] select true as resultado");
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
        public void EventosIgualdad2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Body.[\"Campo103\"].#1 == @event.Message.Body.#103.[\"Campo103.1\"] select true as resultado");
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
        public void EventosIgualdad3()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Header.MessageType == @event.Message.Header.MessageType select true as resultado");
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
        public void EventoConConstanteIgualdad1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Header.MessageType == \"0100\" select true as resultado");
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
        public void EventoConConstanteIgualdad2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Adapter.Name == \"Anonimo\" select true as resultado");
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
        public void ConstanteConEventoIgualdad1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where \"0100\" == @event.Message.Header.MessageType select true as resultado");
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
        public void EventoConConstanteIgualdad3()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.Body.TransactionAmount == 1m select true as resultado");
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
        public void ConstanteConEventoIgualdad2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1m == @event.Message.Body.TransactionAmount select true as resultado");
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
        public void NotFalseIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not (false == true) select true as resultado");
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
        public void NotTrueIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not (true == false) select true as resultado");
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
        public void NotEventosIgualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not(@event.Message.Body.#103.[\"Campo103.1\"] == @event.Message.Body.#103.[\"Campo103.1\"]) select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NotEventosDesigualdad()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where not(@event.Message.Body.#103.[\"Campo103.1\"] != @event.Message.Body.#103.[\"Campo103.1\"]) select true as resultado");
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
        public void NumerosMenorQue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 < 2 select true as resultado");
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
        public void FechasMenorQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '02/03/2015' < '01/01/2014' select true as resultado");
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor();
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
        public void RestaConstantesMenorQueConstante()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1-1 < 1 select true as resultado");
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
        public void DecimalesMenorQue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.21 < 10.22 select true as resultado");
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
        public void RestaHorasMenorQueConstante()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where hour('02/03/2015') - hour('01/01/2014') < 1000 select true as resultado");
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
        public void NumerosMenorIgualQue1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 <= 1 select true as resultado");
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
        public void NumerosMenorIgualQue2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 <= 2 select true as resultado");
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
        public void NumerosMenorIgualQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 2 <= 1 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void FechasMenorIgualQue1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '02/03/2014' <= '01/01/2015' select true as resultado");
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
        public void FechasMenorIgualQue2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '02/03/2014' <= '02/03/2014' select true as resultado");
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
        public void FechasMenorIgualQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '02/03/2015' <= '01/01/2014' select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaConstantesMenorIgualQueConstante1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1-1 <= 1 select true as resultado");
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
        public void RestaConstantesMenorIgualQueConstante2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 2-1 <= 1 select true as resultado");
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
        public void RestaConstantesMenorIgualQueConstanteFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 4-1 <= 1 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void DecimalesMenorIgualQue1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.21 <= 10.22 select true as resultado");
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
        public void DecimalesMenorIgualQue2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.22 <= 10.22 select true as resultado");
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
        public void DecimalesMenorIgualQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.23 <= 10.22 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaHorasMenorIgualQueConstante1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where hour('02/03/2015') - hour('01/01/2014') <= 1000 select true as resultado");
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
        public void RestaHorasMenorIgualQueConstante2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where year('02/03/2015') - year('01/01/1015') <= 1000 select true as resultado");
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
        public void RestaHorasMenorIgualQueConstanteFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where year('02/03/2015') - year('01/01/1010') <= 1000 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NumerosMayorIgualQue1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 >= 1 select true as resultado");
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
        public void NumerosMayorIgualQue2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 2 >= 1 select true as resultado");
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
        public void NumerosMayorIgualQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1 >= 2 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void FechasMayorIgualQue1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '01/01/2015' >= '02/03/2014' select true as resultado");
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
        public void FechasMayorIgualQue2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '02/03/2014' >= '02/03/2014' select true as resultado");
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
        public void FechasMayorIgualQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '02/03/2014' >= '01/01/2015' select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaConstantesMayorIgualQueConstante1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 3-1 >= 1 select true as resultado");
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
        public void RestaConstantesMayorIgualQueConstante2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 2-1 >= 1 select true as resultado");
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
        public void RestaConstantesMayorIgualQueConstanteFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 4-1 >= 10 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void DecimalesMayorIgualQue1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.23 >= 10.22 select true as resultado");
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
        public void DecimalesMayorIgualQue2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.22 >= 10.22 select true as resultado");
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
        public void DecimalesMayorIgualQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.21 >= 10.22 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaHorasMayorIgualQueConstante1()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where year('02/03/2015') - year('01/01/1008') >= 1000 select true as resultado");
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
        public void RestaHorasMayorIgualQueConstante2()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where year('02/03/2015') - year('01/01/1015') >= 1000 select true as resultado");
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
        public void RestaHorasMayorIgualQueConstanteFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where year('02/03/2015') - year('01/01/1018') >= 1000 select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void NumerosMayorQue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 2 > 1 select true as resultado");
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
        public void FechasMayorQueFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where '01/01/2014' > '02/03/2015' select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void RestaConstantesMayorQueConstante()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 20-10 > 1 select true as resultado");
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
        public void DecimalesMayorQue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 10.23 > 10.22 select true as resultado");
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
        public void RestaHorasMayorQueConstante()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where 1000 >  hour('02/03/2015') - hour('01/01/2014') select true as resultado");
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
        public void ConstantLikeIzquierdaTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where \"cadena\" like \"%ena\" select true as resultado");
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
        public void ConstantLikeIzquierdaFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where \"cadena\" like \"%ena2\" select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConstantLikeDerechaTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where \"cadena\" like \"cad%\" select true as resultado");
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
        public void ConstantLikeDerechaFalse()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where \"cadena\" like \"c3ad%\" select true as resultado");
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
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void EventLikeDerechaTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.#1.#2 like \"99999%\" select true as resultado");
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
        public void EventLikeIzquierdaTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.#1.#2 like \"%663\" select true as resultado");
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
        public void EventLikeDualTrue()
        {
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 where @event.Message.#1.#2 like \"%4161%\" select true as resultado");
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
