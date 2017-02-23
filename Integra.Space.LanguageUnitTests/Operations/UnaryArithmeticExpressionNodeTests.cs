using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Integra.Space.Compiler;
using System.Reflection.Emit;
using Ninject;
using Integra.Space.LanguageUnitTests.TestObject;

namespace Integra.Space.LanguageUnitTests.Operations
{
    [TestClass]
    public class UnaryArithmeticExpressionNodeTests
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
                isTestMode: isTestMode
                );

            return config;
        }

        private IObservable<object> Process<T>(string eql, DefaultSchedulerFactory dsf, ITestableObservable<T> input)
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
        public void UnaryNegativeInteger()
        {
            string eql = "from SourceParaPruebas select -1 as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<int> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => int.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<int>>[] {
                    new Recorded<Notification<int>>(100, Notification.CreateOnNext(-1)),
                    new Recorded<Notification<int>>(200, Notification.CreateOnCompleted<int>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void UnaryNegativeDouble()
        {
            string eql = "from SourceParaPruebas select -10.21 as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<double> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => double.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<double>>[] {
                    new Recorded<Notification<double>>(100, Notification.CreateOnNext(-10.21)),
                    new Recorded<Notification<double>>(200, Notification.CreateOnCompleted<double>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void UnaryNegativeDecimal()
        {
            string eql = "from SourceParaPruebas select -1m as resultado into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<TestObject1> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<TestObject1>>(100, Notification.CreateOnNext(new TestObject1())),
                new Recorded<Notification<TestObject1>>(200, Notification.CreateOnCompleted<TestObject1>())
                );

            ITestableObserver<decimal> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("resultado").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<decimal>>[] {
                    new Recorded<Notification<decimal>>(100, Notification.CreateOnNext(-1m)),
                    new Recorded<Notification<decimal>>(200, Notification.CreateOnCompleted<decimal>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
    }
}
