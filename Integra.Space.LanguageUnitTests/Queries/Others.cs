using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Reactive.Linq;
using System.Linq;
using System.Reflection;
using Integra.Space.Compiler;
using Integra.Space.Database;
using System.Reflection.Emit;
using Ninject;
using Integra.Space.LanguageUnitTests.TestObject;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class Others
    {
        private CodeGeneratorConfiguration GetCodeGeneratorConfig(DefaultSchedulerFactory dsf)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            Login login = new SpaceDbContext().Logins.First();
            StandardKernel kernel = new StandardKernel();
            kernel.Bind<ISourceTypeFactory>().ToConstructor(x => new SourceTypeFactory());
            CodeGeneratorConfiguration config = new CodeGeneratorConfiguration(
                login,
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
        public void ConsultaApplyWindowSelectDosEventosProyeccionMixta()
        {
            string eql = "from SourceParaPruebas1 apply window of '00:00:01' select sum((double)TransactionAmount) as suma, TransactionAmount as monto, \"campoXX\" as campo into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
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
            string eql = "from SourceParaPruebas1 where 1 == 1 apply window of '00:00:01' select sum((double)TransactionAmount) as suma, TransactionAmount as monto, \"campoXX\" as campo into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
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
            string eql = "from SourceParaPruebas1 apply window of '00:00:01' select sum((double)TransactionAmount) as suma, TransactionAmount as monto, \"campoXX\" as campo order by suma into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
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
            string eql = "from SourceParaPruebas1 where 1 == 1 apply window of '00:00:01' select sum((double)TransactionAmount) as suma, TransactionAmount as monto, \"campoXX\" as campo order by suma into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
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
            string eql = string.Format("from {0} apply window of {2} select {3} as monto into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "(double)TransactionAmount");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
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
            string eql = "from SourceParaPruebas1 select CardAcceptorNameLocation as campo1 into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
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
