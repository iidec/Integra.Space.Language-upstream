using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using Microsoft.Reactive.Testing;
using System.Linq;
using System.Reactive.Linq;
using System.Data.Entity;
using System.Reactive;
using System.Collections.Generic;
using Integra.Space.Compiler;
using System.Reflection.Emit;
using Ninject;
using Integra.Space.LanguageUnitTests.TestObject;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class MetadataQueriesTests
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
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            bool isTestMode = true;
            CodeGeneratorConfiguration context = this.GetCodeGeneratorConfig(dsf);

            FakePipeline fp = new FakePipeline();
            Delegate d = fp.ProcessWithCommandParser(context, eql, new TestRuleValidator());

            if (isTestMode)
            {
                return (IObservable<object>)d.DynamicInvoke(input.AsObservable(), dsf.TestScheduler);
            }
            else
            {
                return (IObservable<object>)d.DynamicInvoke(input.AsObservable());
            }
        }

        [TestMethod]
        public void TestQueryForMetadata()
        {
            string command = "from sys.Servers as x where (string)x.ServerId == \"59e858fc-c84d-48a7-8a98-c0e7adede20a\" select 1 as servId order by servId into FuenteX";
            command = "from sys.Servers as x where (string)x.ServerId == \"59e858fc-c84d-48a7-8a98-c0e7adede20a\" apply window of '00:00:01' select ServerId as servId, max(1) as maxTest order by desc servId, maxTest into FuenteX";
            //command = "from sys.Servers select ServerId as servId";
            //command = "from sys.servers as x select x.ServerId as servId into FuenteX";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            List<TestServerObject> servers = new List<TestServerObject>
            {
                new TestServerObject("59e858fc-c84d-48a7-8a98-c0e7adede20a", "Server1")
            };

            ITestableObservable<TestServerObject> input = dsf.TestScheduler.CreateColdObservable(
                this.GetRecoredList<TestServerObject>(servers)
            );

            ITestableObserver<object> results = dsf.TestScheduler.Start(
            () => this.Process<TestServerObject>(command, dsf, input)
            .Select(x =>
                (object)(new
                {
                    servId = (string)((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("servId").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)),
                    maxTest = (int)((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("maxTest").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0))
                })
            ),
            created: 10,
            subscribed: 50,
            disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(250, Notification.CreateOnNext((object)(new { servId = "59e858fc-c84d-48a7-8a98-c0e7adede20a", maxTest = 1 }))),
                    new Recorded<Notification<object>>(250, Notification.CreateOnCompleted<object>())
                });
        }

        private Recorded<Notification<TItem>>[] GetRecoredList<TItem>(IEnumerable<TItem> objects)
        {
            List<Recorded<Notification<TItem>>> resultList = new List<Recorded<Notification<TItem>>>();
            foreach (TItem item in objects)
            {
                resultList.Add(new Recorded<Notification<TItem>>(100, Notification.CreateOnNext(item)));
            }

            resultList.Add(new Recorded<Notification<TItem>>(200, Notification.CreateOnCompleted<TItem>()));

            return resultList.ToArray();
        }
    }
}
