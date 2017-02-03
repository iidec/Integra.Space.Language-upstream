using Integra.Space.Compiler;
using Integra.Space.LanguageUnitTests.TestObject;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class ArithmeticExpressionNodeTests
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
        public void RestaPositivosIgualdad()
        {
            string eql = "from SourceParaPruebas where 1 - 1 == 0 select true as resultado into SourceXYZ";
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
        public void RestaNegativosIgualdad()
        {
            string eql = "from SourceParaPruebas where -1 - -1 == 0 select true as resultado into SourceXYZ";
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
        public void RestaPositivoNegativoIgualdad()
        {
            string eql = "from SourceParaPruebas where 1 - -1 == 2 select true as resultado into SourceXYZ";
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
        public void RestaNegativoPositivoIgualdad()
        {
            string eql = "from SourceParaPruebas where -1 - 1 == -2 select true as resultado into SourceXYZ";
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
        public void RestaHourFunctionIgualdadWithoutHour()
        {
            string eql = "from SourceParaPruebas where hour('02/01/2014') - hour('01/01/2014') == 0 select true as resultado into SourceXYZ";
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
        public void RestaHourFunctionIgualdadWithHour()
        {
            string eql = "from SourceParaPruebas where hour('02/01/2014 12:11:10') - hour('01/01/2014 11:11:10') == 1 select true as resultado into SourceXYZ";
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
        public void RestaYearFunctionIgualdad()
        {
            string eql = "from SourceParaPruebas where year('02/03/2015') - year('01/01/2014') == 1 select true as resultado into SourceXYZ";
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
