using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Compiler;
using System.Reflection.Emit;
using Ninject;
using Integra.Space.LanguageUnitTests.TestObject;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class LogicExpressionNodeTests
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
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);

            FakePipeline fp = new FakePipeline();
            Assembly assembly = fp.ProcessWithQueryParser(context, eql, dsf);

            Type[] types = assembly.GetTypes();
            Type queryInfo = assembly.GetTypes().First(x => x.GetInterface("IQueryInformation") == typeof(IQueryInformation));
            IQueryInformation queryInfoObject = (IQueryInformation)Activator.CreateInstance(queryInfo);
            Type queryType = queryInfoObject.GetQueryType();
            object queryObject = Activator.CreateInstance(queryType);
            MethodInfo result = queryObject.GetType().GetMethod("MainFunction");

            return ((IObservable<object>)result.Invoke(queryObject, new object[] { input.AsObservable(), dsf.TestScheduler }));
        }

        [TestMethod]
        public void EventosIgualdadAndEventosIgualdad()
        {
            string eql = "from SourceParaPruebas1 where Campo104 == Campo104 and Campo104 == Campo104 select true as resultado into SourceXYZ";
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
        public void EventosIgualdadAndEventosIgualdadFalse()
        {
            string eql = "from SourceParaPruebas1 where not (Campo104 == Campo104 or Campo104 == Campo104) select true as resultado into SourceXYZ";
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
        public void EventosIgualdadOrEventosIgualdad()
        {
            string eql = "from SourceParaPruebas1 where Campo104 == Campo104 or Campo104 == Campo104 select true as resultado into SourceXYZ";
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
        public void EventosIgualdadOrEventosIgualdadFalse()
        {
            string eql = "from SourceParaPruebas1 where not (Campo104 == Campo104 or Campo104 == Campo104) select true as resultado into SourceXYZ";
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
        public void TrueAndTrue()
        {
            string eql = "from SourceParaPruebas1 where true and true select true as resultado into SourceXYZ";
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
        public void FalseAndFalse()
        {
            string eql = "from SourceParaPruebas1 where false and false select true as resultado into SourceXYZ";
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
        public void TrueAndFalse()
        {
            string eql = "from SourceParaPruebas1 where true and false select true as resultado into SourceXYZ";
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
        public void FalseAndTrue()
        {
            string eql = "from SourceParaPruebas1 where false and true select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where true or true select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where true or false select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();


            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where false or true select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where false or false select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where not(false or false) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where not(true or true) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where not(false and false) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where not(true and true) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where not (true) or false and true select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
            string eql = "from SourceParaPruebas1 where not (true or false and true) select true as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
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
