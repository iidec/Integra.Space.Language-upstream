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
using System.Reflection;
using Integra.Space.LanguageUnitTests.TestObject;
using System.Reflection.Emit;
using Integra.Space.Compiler;
using Ninject;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class ComparativeExpressionNodeTests : ReactiveTest
    {
        private CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            SpaceAssemblyBuilder sasmBuilder = new SpaceAssemblyBuilder("Test");
            AssemblyBuilder asmBuilder = sasmBuilder.CreateAssemblyBuilder();
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGeneratorConfiguration config = new CodeGeneratorConfiguration(
                dsf,
                asmBuilder,
                kernel,
                printLog: printLog,
                debugMode: debugMode,
                measureElapsedTime: measureElapsedTime,
                isTestMode: isTestMode,
                queryName: "QueryTest"
                );

            return config;
        }

        private IObservable<object> Process<T>(string eql, DefaultSchedulerFactory dsf, ITestableObservable<T> input)
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

            return ((IObservable<object>)result.Invoke(queryObject, new object[] { input.AsObservable(), dsf.TestScheduler }));
        }

        [TestMethod()]
        public void IsNumericTypeFunctionTestTrue()
        {
            ComparativeExpressionASTNode comp = new ComparativeExpressionASTNode();

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
            ComparativeExpressionASTNode comp = new ComparativeExpressionASTNode();

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
            string eql = "from SourceParaPruebas1 where 1 == 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                        .Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
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
            string eql = "from SourceParaPruebas1 where +1 == +1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NegativosIgualdad()
        {
            string eql = "from SourceParaPruebas1 where -1 == -1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 0,
                subscribed: 10,
                disposed: 400
                );

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(10, 200)
                });
        }

        [TestMethod]
        public void BooleanosIgualdad()
        {
            string eql = "from SourceParaPruebas1 where true == true select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateColdObservable(
                OnNext(TimeSpan.FromSeconds(1).Ticks, new TestObject1())
                , OnCompleted<TestObject1>(TimeSpan.FromSeconds(2).Ticks)
                );

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                        .Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()))
                        ,
                created: TimeSpan.FromSeconds(1).Ticks,
                subscribed: TimeSpan.FromSeconds(2).Ticks,
                disposed: TimeSpan.FromSeconds(10).Ticks
                );

            ReactiveAssert.AreElementsEqual(new[] {
                    OnNext(TimeSpan.FromSeconds(3).Ticks, true)
                    , OnCompleted<bool>(TimeSpan.FromSeconds(4).Ticks)
                }, results.Messages);

            ReactiveAssert.AreElementsEqual(new Subscription[] {
                    new Subscription(TimeSpan.FromSeconds(2).Ticks, TimeSpan.FromSeconds(4).Ticks)
                }, input.Subscriptions);
        }

        [TestMethod]
        public void FechasIgualdad()
        {
            string eql = "from SourceParaPruebas1 where '01/01/2014' == '01/01/2014' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdad()
        {
            string eql = "from SourceParaPruebas1 where \"a\" == \"a\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadLeftFunction1()
        {
            string eql = "from SourceParaPruebas1 where left(\"ca\", 1) == \"c\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadLeftFunction2()
        {
            string eql = "from SourceParaPruebas1 where left(\"xyzxxx\", 3) == \"xyz\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadLeftFunction3()
        {
            string eql = "from SourceParaPruebas1 where left((string)CardAcceptorNameLocation, 5) == \"Shell\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadRightFunction1()
        {
            string eql = "from SourceParaPruebas1 where right(\"ab\", 1) == \"b\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadRightFunction2()
        {
            string eql = "from SourceParaPruebas1 where right(\"xxxabcd\", 4) == \"abcd\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadRightFunction3()
        {
            string eql = "from SourceParaPruebas1 where right((string)CardAcceptorNameLocation, 2) == \"GT\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadUpperFunction1()
        {
            string eql = "from SourceParaPruebas1 where upper(\"ca\") == \"CA\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadUpperFunction2()
        {
            string eql = "from SourceParaPruebas1 where upper((string)CardAcceptorNameLocation) == \"SHELL EL RODEO1GUATEMALA    GT\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadLowerFunction1()
        {
            string eql = "from SourceParaPruebas1 where lower(\"CA\") == \"ca\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void CadenasIgualdadLowerFunction2()
        {
            string eql = "from SourceParaPruebas1 where lower((string)CardAcceptorNameLocation) == \"shell el rodeo1guatemala    gt\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NulosIgualdad()
        {
            string eql = "from SourceParaPruebas1 where null == null select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventoCampoInexistenteIgualdadNulo()
        {
            string eql = "from SourceParaPruebas1 where @event.Message.Body.#103.[\"Campo que no existe\"] == null select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NuloIgualdadEventoCampoInexistente()
        {
            string eql = "from SourceParaPruebas1 where null == @event.Message.Body.#103.[\"Campo que no existe\"] select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventosIgualdad1()
        {
            string eql = "from SourceParaPruebas1 where Campo104 == Campo104 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventosIgualdad2()
        {
            string eql = "from SourceParaPruebas1 where Campo104 == Campo104 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventosIgualdad3()
        {
            string eql = "from SourceParaPruebas1 where MessageType == MessageType select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventosIgualdad4()
        {
            string eql = "from SourceParaPruebas1 where MessageType == \"0100\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();            
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);
            QueryParser parser = new QueryParser(eql);
            PlanNode executionPlan = parser.Evaluate().Item1;
            CodeGenerator te = new CodeGenerator(context);
            Assembly assembly = te.Compile(executionPlan);

            Type[] types = assembly.GetTypes();
            /*Type inputType = types.First(x => x.BaseType == typeof(InputBase));*/
            Type queryInfo = types.First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            List<Tuple<string, object>> listOfProperties = new List<Tuple<string, object>>();
            listOfProperties.Add(Tuple.Create<string, object>("MessageType","0100"));
            listOfProperties.Add(Tuple.Create<string, object>("PrimaryAccountNumber", "9999941616073663"));
            listOfProperties.Add(Tuple.Create<string, object>("ProcessingCode", "302000"));
            listOfProperties.Add(Tuple.Create<string, object>("TransactionAmount", 1m));
            listOfProperties.Add(Tuple.Create<string, object>("DateTimeTransmission", "0508152549"));
            listOfProperties.Add(Tuple.Create<string, object>("SystemTraceAuditNumber", "212868"));
            listOfProperties.Add(Tuple.Create<string, object>("LocalTransactionTime", "152549"));
            listOfProperties.Add(Tuple.Create<string, object>("LocalTransactionDate", "0508"));
            listOfProperties.Add(Tuple.Create<string, object>("SettlementDate", "0508"));
            listOfProperties.Add(Tuple.Create<string, object>("MerchantType", "6011"));
            listOfProperties.Add(Tuple.Create<string, object>("AcquiringInstitutionCountryCode", "320"));
            listOfProperties.Add(Tuple.Create<string, object>("PointOfServiceEntryMode", "051"));
            listOfProperties.Add(Tuple.Create<string, object>("PointOfServiceConditionCode", "02"));
            listOfProperties.Add(Tuple.Create<string, object>("AcquiringInstitutionIdentificationCode", "491381"));
            listOfProperties.Add(Tuple.Create<string, object>("Track2Data", "9999941616073663D18022011583036900000"));
            listOfProperties.Add(Tuple.Create<string, object>("RetrievalReferenceNumber", "412815212868"));
            listOfProperties.Add(Tuple.Create<string, object>("CardAcceptorTerminalIdentification", "2906    "));
            listOfProperties.Add(Tuple.Create<string, object>("CardAcceptorIdentificationCode", "Shell El Rodeo "));
            listOfProperties.Add(Tuple.Create<string, object>("CardAcceptorNameLocation", "Shell El Rodeo1GUATEMALA    GT"));
            listOfProperties.Add(Tuple.Create<string, object>("TransactionCurrencyCode", "320"));
            listOfProperties.Add(Tuple.Create<string, object>("AccountIdentification1", "00001613000000000001"));
            listOfProperties.Add(Tuple.Create<string, object>("Campo104", -1));
            listOfProperties.Add(Tuple.Create<string, object>("Campo105", (uint)1));
            
            List<Tuple<long, object[]>> l = new List<Tuple<long, object[]>>();
            l.Add(Tuple.Create<long, object[]>(100, listOfProperties.Select(x => x.Item2).ToArray()));

            /*
            dynamic item1 = Activator.CreateInstance(types[0]);
            item1.MessageType = "0100";
            MethodInfo onNextMethod = typeof(Notification).GetMethod("CreateOnNext").MakeGenericMethod(types[0]);
            MethodInfo onCompletedMethod = typeof(Notification).GetMethod("CreateOnCompleted").MakeGenericMethod(types[0]);
            object inputItem1 = Activator.CreateInstance(typeof(Recorded<>).MakeGenericType(typeof(Notification<>).MakeGenericType(types[0])), new[] { 100, onNextMethod.Invoke(null, new[] { item1 }) });
            object inputItem2 = Activator.CreateInstance(typeof(Recorded<>).MakeGenericType(typeof(Notification<>).MakeGenericType(types[0])), new[] { 200, onCompletedMethod.Invoke(null, new object[] { }) });
            Array notificationsArray = Array.CreateInstance(inputItem1.GetType(), 2);
            notificationsArray.SetValue(inputItem1, 0);
            notificationsArray.SetValue(inputItem2, 1);
            object hotObservable = dsf.TestScheduler.GetType().GetMethod("CreateHotObservable").MakeGenericMethod(types[0]).Invoke(dsf.TestScheduler, new[] { notificationsArray });
            object hotObservableAsObservable = typeof(System.Reactive.Linq.Observable).GetMethod("AsObservable").MakeGenericMethod(types[0]).Invoke(null, new[] { hotObservable });
            */
            
            object hotObservable = this.GetType().GetMethod("NewMethod").MakeGenericMethod(typeof(TestObject1)).Invoke(this, new object[] { l, 200, dsf });
            object hotObservableAsObservable = typeof(Observable).GetMethod("AsObservable").MakeGenericMethod(typeof(TestObject1)).Invoke(null, new[] { hotObservable });
            IObservable<object> resultObservable = ((IObservable<object>)result.Invoke(queryObject, new object[] { hotObservableAsObservable, dsf.TestScheduler }));

            ITestableObserver<bool> results = dsf.TestScheduler.Start(
                () => resultObservable.Select(x => bool.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<bool>>[] {
                    new Recorded<Notification<bool>>(100, Notification.CreateOnNext(true)),
                    new Recorded<Notification<bool>>(200, Notification.CreateOnCompleted<bool>())
                });

            ReactiveAssert.AreElementsEqual((IList<Subscription>)hotObservable.GetType().GetProperty("Subscriptions").GetValue(hotObservable, new object[] { }), new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void EventosIgualdad5()
        {
            string eql = "from SourceParaPruebas1 where MessageType == \"0100\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public ITestableObservable<T> NewMethod<T>(IEnumerable<Tuple<long, object[]>> messages, long onCompletedTime, DefaultSchedulerFactory dsf) where T : EventBase
        {
            List<Recorded<Notification<T>>> listOfNotifications = new List<Recorded<Notification<T>>>();
            foreach (Tuple<long, object[]> message in messages)
            {
                T deserializedObject = (T)Activator.CreateInstance(typeof(T), message.Item2);
                listOfNotifications.Add(new Recorded<Notification<T>>(message.Item1, Notification.CreateOnNext(deserializedObject)));
            }

            listOfNotifications.Add(new Recorded<Notification<T>>(onCompletedTime, Notification.CreateOnCompleted<T>()));
            return dsf.TestScheduler.CreateHotObservable(listOfNotifications.ToArray());            
        }

        [TestMethod]
        public void EventoConConstanteIgualdad1()
        {
            string eql = "from SourceParaPruebas1 where MessageType == \"0100\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventoConConstanteIgualdad2()
        {
            string eql = "from SourceParaPruebas1 where @event.Adapter.Name == \"Anonimo\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void ConstanteConEventoIgualdad1()
        {
            string eql = "from SourceParaPruebas1 where \"0100\" == MessageType select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventoConConstanteIgualdad3()
        {
            string eql = "from SourceParaPruebas1 where TransactionAmount == 1m select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void ConstanteConEventoIgualdad2()
        {
            string eql = "from SourceParaPruebas1 where 1m == TransactionAmount select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NotFalseIgualdad()
        {
            string eql = "from SourceParaPruebas1 where not (false == true) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NotTrueIgualdad()
        {
            string eql = "from SourceParaPruebas1 where not (true == false) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NotEventosIgualdad()
        {
            string eql = "from SourceParaPruebas1 where not(Campo104 == Campo104) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NotEventosDesigualdad()
        {
            string eql = "from SourceParaPruebas1 where not(Campo104 != Campo104) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMenorQue()
        {
            string eql = "from SourceParaPruebas1 where 1 < 2 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMenorQueFalse()
        {
            string eql = "from SourceParaPruebas1 where '02/03/2015' < '01/01/2014' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMenorQueConstante()
        {
            string eql = "from SourceParaPruebas1 where 1-1 < 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMenorQue()
        {
            string eql = "from SourceParaPruebas1 where 10.21 < 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMenorQueConstante()
        {
            string eql = "from SourceParaPruebas1 where hour('02/03/2015') - hour('01/01/2014') < 1000 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMenorIgualQue1()
        {
            string eql = "from SourceParaPruebas1 where 1 <= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMenorIgualQue2()
        {
            string eql = "from SourceParaPruebas1 where 1 <= 2 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMenorIgualQueFalse()
        {
            string eql = "from SourceParaPruebas1 where 2 <= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMenorIgualQue1()
        {
            string eql = "from SourceParaPruebas1 where '02/03/2014' <= '01/01/2015' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();


            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMenorIgualQue2()
        {
            string eql = "from SourceParaPruebas1 where '02/03/2014' <= '02/03/2014' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMenorIgualQueFalse()
        {
            string eql = "from SourceParaPruebas1 where '02/03/2015' <= '01/01/2014' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMenorIgualQueConstante1()
        {
            string eql = "from SourceParaPruebas1 where 1-1 <= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMenorIgualQueConstante2()
        {
            string eql = "from SourceParaPruebas1 where 2-1 <= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMenorIgualQueConstanteFalse()
        {
            string eql = "from SourceParaPruebas1 where 4-1 <= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMenorIgualQue1()
        {
            string eql = "from SourceParaPruebas1 where 10.21 <= 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMenorIgualQue2()
        {
            string eql = "from SourceParaPruebas1 where 10.22 <= 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMenorIgualQueFalse()
        {
            string eql = "from SourceParaPruebas1 where 10.23 <= 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMenorIgualQueConstante1()
        {
            string eql = "from SourceParaPruebas1 where hour('02/03/2015') - hour('01/01/2014') <= 1000 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMenorIgualQueConstante2()
        {
            string eql = "from SourceParaPruebas1 where year('02/03/2015') - year('01/01/1015') <= 1000 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMenorIgualQueConstanteFalse()
        {
            string eql = "from SourceParaPruebas1 where year('02/03/2015') - year('01/01/1010') <= 1000 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMayorIgualQue1()
        {
            string eql = "from SourceParaPruebas1 where 1 >= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMayorIgualQue2()
        {
            string eql = "from SourceParaPruebas1 where 2 >= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMayorIgualQueFalse()
        {
            string eql = "from SourceParaPruebas1 where 1 >= 2 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMayorIgualQue1()
        {
            string eql = "from SourceParaPruebas1 where '01/01/2015' >= '02/03/2014' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();


            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMayorIgualQue2()
        {
            string eql = "from SourceParaPruebas1 where '02/03/2014' >= '02/03/2014' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();


            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMayorIgualQueFalse()
        {
            string eql = "from SourceParaPruebas1 where '02/03/2014' >= '01/01/2015' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMayorIgualQueConstante1()
        {
            string eql = "from SourceParaPruebas1 where 3-1 >= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMayorIgualQueConstante2()
        {
            string eql = "from SourceParaPruebas1 where 2-1 >= 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMayorIgualQueConstanteFalse()
        {
            string eql = "from SourceParaPruebas1 where 4-1 >= 10 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();


            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMayorIgualQue1()
        {
            string eql = "from SourceParaPruebas1 where 10.23 >= 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMayorIgualQue2()
        {
            string eql = "from SourceParaPruebas1 where 10.22 >= 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMayorIgualQueFalse()
        {
            string eql = "from SourceParaPruebas1 where 10.21 >= 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMayorIgualQueConstante1()
        {
            string eql = "from SourceParaPruebas1 where year('02/03/2015') - year('01/01/1008') >= 1000 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMayorIgualQueConstante2()
        {
            string eql = "from SourceParaPruebas1 where year('02/03/2015') - year('01/01/1015') >= 1000 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMayorIgualQueConstanteFalse()
        {
            string eql = "from SourceParaPruebas1 where year('02/03/2015') - year('01/01/1018') >= 1000 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void NumerosMayorQue()
        {
            string eql = "from SourceParaPruebas1 where 2 > 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void FechasMayorQueFalse()
        {
            string eql = "from SourceParaPruebas1 where '01/01/2014' > '02/03/2015' select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaConstantesMayorQueConstante()
        {
            string eql = "from SourceParaPruebas1 where 20-10 > 1 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void DecimalesMayorQue()
        {
            string eql = "from SourceParaPruebas1 where 10.23 > 10.22 select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void RestaHorasMayorQueConstante()
        {
            string eql = "from SourceParaPruebas1 where 1000 >  hour('02/03/2015') - hour('01/01/2014') select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void ConstantLikeIzquierdaTrue()
        {
            string eql = "from SourceParaPruebas1 where \"cadena\" like \"%ena\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void ConstantLikeIzquierdaFalse()
        {
            string eql = "from SourceParaPruebas1 where \"cadena\" like \"%ena2\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void ConstantLikeDerechaTrue()
        {
            string eql = "from SourceParaPruebas1 where \"cadena\" like \"cad%\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void ConstantLikeDerechaFalse()
        {
            string eql = "from SourceParaPruebas1 where \"cadena\" like \"c3ad%\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventLikeDerechaTrue()
        {
            string eql = "from SourceParaPruebas1 where PrimaryAccountNumber like \"99999%\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventLikeIzquierdaTrue()
        {
            string eql = "from SourceParaPruebas1 where PrimaryAccountNumber like \"%663\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
        public void EventLikeDualTrue()
        {
            string eql = "from SourceParaPruebas1 where PrimaryAccountNumber like \"%4161%\" select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
    }
}
