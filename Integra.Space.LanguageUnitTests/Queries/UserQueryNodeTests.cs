﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Collections.Generic;
using Integra.Space.Language.Exceptions;
using System.Reflection;
using Integra.Space.Compiler;
using System.Reflection.Emit;
using Ninject;
using Integra.Space.LanguageUnitTests.TestObject;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class UserQueryNodeTests
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

        [TestMethod]
        public void ConsultaProyeccionCampoNuloConWhere()
        {
            string eql = string.Format("from {0} where {1} select {2} as CampoNulo into SourceXYZ",
                                                                "SourceParaPruebas1",
                                                                "MessageType == \"0100\"",
                                                                "Campo_que_no_existe");
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
                        CampoNulo = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("CampoNulo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0))
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
            string eql = string.Format("from {0} select {1} as CampoNulo into SourceXYZ",
                                                                "SourceParaPruebas1",
                                                                "@event.Message.#0.[\"Campo que no existe\"]");
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
                        CampoNulo = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("CampoNulo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0))
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Contador into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1",
                                                                                            "grupo1",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Contador = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Contador").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select {3} as Resultado into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select {3} as Resultado into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "MessageType");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Resultado2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString()
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
            string eql = string.Format("from {0} apply window of {2} select {3} as Resultado1, {4} as Resultado2 into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "MessageType",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Resultado2 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        Resultado3 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("Resultado1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString(),
                        Resultado4 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("Resultado2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select {3} as Resultado into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "max((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select {3} as Resultado into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "min((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
        public void ConsultaSoloApplyWindowDosEventosOrderByDesc()
        {
            string eql = string.Format("from {0} apply window of {2} select {3} as monto order by desc monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
            string eql = string.Format("from {0} select top 1 {3} as monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "(double)TransactionAmount");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} select top 1 {3} as monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select top 1 {3} as monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select top 1 {3} as monto order by desc monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select top 1 {3} as monto order by asc monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select top 1 {3} as monto order by monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto order by desc monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto order by asc monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "(double)TransactionAmount");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto order by monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} select top 1 {3} as monto into SourceXYZ",
                                                                                            "SourceParaPruebas",
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
                        Resultado1 = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("monto").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by desc Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2HONDURAS     HN", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumTopOrderByAsc()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by asc Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\" and TransactionAmount between 0m and 4m /*TransactionAmount > 0m and TransactionAmount < 4m*/",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by desc Sumatoria, Llave into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2HONDURAS     HN", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumTopOrderByAsc()
        {
            string eql = string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by asc Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} group by {3} select top 1 {4} as Llave, {5} as Sumatoria order by Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} select {3} as monto order by desc monto into SourceXYZ",
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
            string eql = string.Format("from {0} apply window of {2} select {3} as monto order by asc monto into SourceXYZ",
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
        public void ConsultaApplyWindowSelectDosEventosOrderBy()
        {
            string eql = string.Format("from {0} apply window of {2} select {3} as monto order by monto into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "(double)TransactionAmount");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
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
        public void ConsultaWhereApplyWindowSelectDosEventosOrderByDesc()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} select {3} as monto order by desc monto into SourceXYZ",
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
            string eql = string.Format("from {0} where {1} apply window of {2} select {3} as monto order by asc monto into SourceXYZ",
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
        public void ConsultaWhereApplyWindowSelectDosEventosOrderBy()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} select {3} as monto order by monto into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "(double)TransactionAmount");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
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
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumOrderByDesc()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by desc Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2HONDURAS     HN", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectUnaLlaveYSumOrderByAsc()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by asc Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by desc Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
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
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo2HONDURAS     HN", Sumatoria = 2m }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaApplyWindowGroupBySelectUnaLlaveYSumOrderByAsc()
        {
            string eql = string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by asc Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria order by Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "CardAcceptorNameLocation as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Llave = "Shell El Rodeo1GUATEMALA    GT", Sumatoria = 2m }))),
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
            string eql = string.Format("from {0} where {1} apply window of {2} select {3} as Resultado into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} select {3} as Resultado into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "MessageType");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Resultado2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("Resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString()
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
            string eql = string.Format("from {0} where {1} apply window of {2} select {3} as Resultado1, {4} as Resultado2 into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1                                                                                        
                                                                                            "MessageType",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Resultado1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Resultado2 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Resultado2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        Resultado3 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("Resultado1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString(),
                        Resultado4 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("Resultado2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave, {5} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave1, {5} as Llave2, {6} as Contador into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1, PrimaryAccountNumber as grupo2",
                                                                                            "grupo1",
                                                                                            "grupo2",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Llave2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Contador = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Contador").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave1, {5} as Llave2, {6} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas1",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1, PrimaryAccountNumber as grupo2",
                                                                                            "grupo1",
                                                                                            "grupo2",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Llave2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {1} group by {2} select {3} as Llave, {4} as Contador into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1",
                                                                                            "grupo1",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Contador = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Contador").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {1} group by {2} select {3} as Llave, {4} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {1} group by {2} select {3} as Llave into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "'00:00:10'",
                                                                                            "MessageType as grupo1",
                                                                                            "grupo1",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()
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
            string eql = string.Format("from {0} apply window of {1} group by {2} select {3} as Llave1, {4} as Llave2, {5} as Contador into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1, PrimaryAccountNumber as grupo2",
                                                                                            "grupo1",
                                                                                            "grupo2",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Llave2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Contador = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Contador").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
            string eql = string.Format("from {0} apply window of {1} group by {2} select {3} as Llave1, {4} as Llave2, {5} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1, PrimaryAccountNumber as grupo2",
                                                                                            "grupo1",
                                                                                            "grupo2",
                                                                                            "sum((double)TransactionAmount)");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        Llave1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave1").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Llave2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Llave2").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        Sumatoria = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("Sumatoria").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
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
        public void ConsultaCountSinGroupBySinWhere()
        {
            try
            {
                // no es posible utilizar key sin un group by
                QueryParser parser = new QueryParser(
                    string.Format("from {0} select {1} as Llave into SourceXYZ",
                                                                "SourceParaPruebas",
                                                                "count()")
                                                                );
                PlanNode plan = parser.Evaluate().Item1;

                CodeGenerator te = new CodeGenerator(this.GetCodeGeneratorConfig(new DefaultSchedulerFactory()));
                Func<IObservable<TestObject1>, IObservable<object>> result = (Func<IObservable<TestObject1>, IObservable<object>>)te.CompileDelegate(plan);

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
                QueryParser parser = new QueryParser(
                    string.Format("from {0} select {1} as Llave into SourceXYZ",
                                                                "SourceParaPruebas",
                                                                "sum((double)TransactionAmount)")
                                                                );
                PlanNode plan = parser.Evaluate().Item1;

                CodeGenerator te = new CodeGenerator(this.GetCodeGeneratorConfig(new DefaultSchedulerFactory()));
                Func<IObservable<TestObject1>, IObservable<object>> result = (Func<IObservable<TestObject1>, IObservable<object>>)te.CompileDelegate(plan);

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
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1",
                                                                                            "key.grupo1",
                                                                                            "sum((double)CardAcceptorNameLocation)")
                                                                                            );
            List<PlanNode> plan = parser.Parse();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IObservable<TestObject1>, IObservable<object>> result = te.Compile<IObservable<TestObject1>, IObservable<object>>(plan.First());

            TestScheduler dsf.TestScheduler = new TestScheduler();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                {
                    return ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0);
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
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion1()
        {
            string command = string.Format("from {0} where {1} apply window of {2} group by {3} select {4} as Llave1, {5} as Llave2, {6} as Contador, {7} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\" and MessageType == \"0100\"",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1, PrimaryAccountNumber as grupo2",
                                                                                            "grupo1",
                                                                                            "grupo2",
                                                                                            "count()",
                                                                                            "sum((double)TransactionAmount)");
            QueryParser parser = new QueryParser(command);
            PlanNode plan = parser.Evaluate().Item1;

            Assert.AreEqual<string>(command, plan.NodeText);
        }

        [TestMethod]
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion2()
        {
            string command = string.Format("from {0} apply window of {1} group by {2} select {3} as Llave1, {4} as Llave2, {5} as Contador, {6} as Sumatoria into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "'00:00:00:01'",
                                                                                            "MessageType as grupo1, PrimaryAccountNumber as grupo2",
                                                                                            "grupo1",
                                                                                            "grupo2",
                                                                                            "count()",
                                                                                            "sum((double)TransactionAmount)");
            QueryParser parser = new QueryParser(command);
            PlanNode plan = parser.Evaluate().Item1;

            Assert.AreEqual<string>(command, plan.NodeText);
        }

        [TestMethod]
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion3()
        {
            string command = string.Format("from {0} where {1} select {2} as PrimaryAccountNumber, {3} as CardAcceptorNameLocation, {4} as TransactionAmount, {5} as TransactionCurrencyCode into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "MessageType == \"0100\" and MessageType == \"0100\"",
                                                                                            "PrimaryAccountNumber",
                                                                                            "CardAcceptorNameLocation",
                                                                                            "TransactionAmount",
                                                                                            "TransactionCurrencyCode");
            QueryParser parser = new QueryParser(command);
            PlanNode plan = parser.Evaluate().Item1;

            Assert.AreEqual<string>(command, plan.NodeText);
        }

        [TestMethod]
        public void CreacionCadenaDeConsultaDesdePlanDeEjecucion4()
        {
            string command = string.Format("from {0} select {1} as PrimaryAccountNumber, {2} as CardAcceptorNameLocation, {3} as TransactionAmount, {4} as TransactionCurrencyCode into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "PrimaryAccountNumber",
                                                                                            "CardAcceptorNameLocation",
                                                                                            "TransactionAmount",
                                                                                            "TransactionCurrencyCode");
            QueryParser parser = new QueryParser(command);
            PlanNode plan = parser.Evaluate().Item1;

            Assert.AreEqual<string>(command, plan.NodeText);
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectDosEventosTopOrderByAsc()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} as pais select top 3 {4} as pais, {5} as conteo order by asc pais, conteo into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "CardAcceptorNameLocation != null",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1
                                                                                            "right((string)CardAcceptorNameLocation, 2)",
                                                                                            "pais",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        pais1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("pais").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        conteo1 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("conteo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        pais2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("pais").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString(),
                        conteo2 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("conteo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString()),
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { pais1 = "GT", conteo1 = 3, pais2 = "HN", conteo2 = 5 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectDosEventosTopOrderBy()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} as pais select top 3 {4} as pais, {5} as conteo order by pais, conteo into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "CardAcceptorNameLocation != null",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1
                                                                                            "right((string)CardAcceptorNameLocation, 2)",
                                                                                            "pais",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        pais1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("pais").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        conteo1 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("conteo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        pais2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("pais").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString(),
                        conteo2 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("conteo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString()),
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { pais1 = "GT", conteo1 = 3, pais2 = "HN", conteo2 = 5 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectDosEventosTopOrderByDesc()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} as pais select top 3 {4} as pais, {5} as conteo order by desc pais, conteo into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "CardAcceptorNameLocation != null",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1
                                                                                            "right((string)CardAcceptorNameLocation, 2)",
                                                                                            "pais",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                    (object)(new
                    {
                        pais1 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("pais").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString(),
                        conteo1 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("conteo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                        pais2 = ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("pais").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString(),
                        conteo2 = int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1).GetType().GetProperty("conteo").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(1)).ToString()),
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { pais1 = "HN", conteo1 = 5, pais2 = "GT", conteo2 = 3 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectDosEventosTopOrderByDescLowerWithNullValue()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} as comercio select top 3 {4} as comercio, {5} as conteo order by desc conteo into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "CardAcceptorNameLocation != null and lower((string)null) like \"%shell%\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1
                                                                                            "CardAcceptorNameLocation",
                                                                                            "comercio",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                {
                    if (x == null)
                    {
                        return null;
                    }
                    return (object)(new
                    {
                        Resultado = ((Array)x.GetType().GetProperty("Result").GetValue(x)).Length
                    });
                }),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 0 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectDosEventosTopOrderByDescUpperWithNullValue()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} as comercio select top 3 {4} as comercio, {5} as conteo order by desc conteo into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "CardAcceptorNameLocation != null and upper((string)null) like \"%shell%\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1
                                                                                            "CardAcceptorNameLocation",
                                                                                            "comercio",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                {
                    if (x == null)
                    {
                        return null;
                    }
                    return (object)(new
                    {
                        Resultado = ((Array)x.GetType().GetProperty("Result").GetValue(x)).Length
                    });
                }),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 0 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectDosEventosTopOrderByDescLeftWithNullValue()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} as comercio select top 3 {4} as comercio, {5} as conteo order by desc conteo into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "CardAcceptorNameLocation != null and left((string)null, 5) like \"%shell%\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1
                                                                                            "CardAcceptorNameLocation",
                                                                                            "comercio",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                {
                    if (x == null)
                    {
                        return null;
                    }
                    return (object)(new
                    {
                        Resultado = ((Array)x.GetType().GetProperty("Result").GetValue(x)).Length
                    });
                }),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 0 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void ConsultaWhereApplyWindowGroupBySelectDosEventosTopOrderByDescRightWithNullValue()
        {
            string eql = string.Format("from {0} where {1} apply window of {2} group by {3} as comercio select top 3 {4} as comercio, {5} as conteo order by desc conteo into SourceXYZ",
                                                                                            "SourceParaPruebas",
                                                                                            "CardAcceptorNameLocation != null and right((string)null, 3) like \"%shell%\"",
                                                                                            "'00:00:01'", // hay un comportamiento inesperado cuando el segundo parametro es 2 y se envian dos TestObject1
                                                                                            "CardAcceptorNameLocation",
                                                                                            "comercio",
                                                                                            "count()");
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1(transactionAmount: 2m, cardAcceptorNameLocation: "Shell El Rodeo2HONDURAS     HN"))),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input)
                .Select(x =>
                {
                    if (x == null)
                    {
                        return null;
                    }
                    return (object)(new
                    {
                        Resultado = ((Array)x.GetType().GetProperty("Result").GetValue(x)).Length
                    });
                }),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { Resultado = 0 }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
    }
}
