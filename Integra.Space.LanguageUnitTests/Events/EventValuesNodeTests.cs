using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using System.Reactive.Linq;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using System.Reactive;
using System.Reflection;

namespace Integra.Space.LanguageUnitTests.Events
{
    [TestClass]
    public class EventValuesNodeTests
    {
        List<EventObject> eventList = new List<EventObject>();
        
        private IObservable<object> Process(string eql, DefaultSchedulerFactory dsf, ITestableObservable<EventObject> input)
        {
            bool printLog = false;
            bool debugMode = false;
            bool measureElapsedTime = false;
            CompilerConfiguration context = new CompilerConfiguration() { PrintLog = printLog, QueryName = string.Empty, Scheduler = dsf, DebugMode = debugMode, MeasureElapsedTime = measureElapsedTime, IsTestMode = true };

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
        public void EventPropertyAgentNameValue()
        {
            eventList.Add(TestObjects.EventObjectTest1);

            string eql = "from SpaceObservable1 select @event.agent.Name as nombre into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<string> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("nombre").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<string>>[] {
                    new Recorded<Notification<string>>(100, Notification.CreateOnNext("Anonimo")),
                    new Recorded<Notification<string>>(200, Notification.CreateOnCompleted<string>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void EventPropertyAdapterNameValue()
        {
            string eql = "from SpaceObservable1 select @event.Adapter.Name as nombre into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<string> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("nombre").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<string>>[] {
                    new Recorded<Notification<string>>(100, Notification.CreateOnNext("Anonimo")),
                    new Recorded<Notification<string>>(200, Notification.CreateOnCompleted<string>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }

        [TestMethod]
        public void EventPropertyAgentMachineNameValue()
        {
            string eql = "from SpaceObservable1 select @event.Agent.MachineName as machineName into SourceXYZ";
            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<string> results = dsf.TestScheduler.Start(
                () => this.Process(eql, dsf, input).Select(x => ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("machineName").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
                created: 10,
                subscribed: 50,
                disposed: 400);

            ReactiveAssert.AreElementsEqual(results.Messages, new Recorded<Notification<string>>[] {
                    new Recorded<Notification<string>>(100, Notification.CreateOnNext("Anonimo")),
                    new Recorded<Notification<string>>(200, Notification.CreateOnCompleted<string>())
                });

            ReactiveAssert.AreElementsEqual(input.Subscriptions, new Subscription[] {
                    new Subscription(50, 200)
                });
        }
        
        [TestMethod]
        public void EventPropertyAgentTimeStampType()
        {
            string eql = "from SpaceObservable1 select @event.Agent.Timestamp as ts into SourceXYZ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();
            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            this.Process(eql, dsf, input).Subscribe(x => {
                Assert.IsInstanceOfType(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("ts").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)), typeof(DateTime), "El plan obtenido difiere del plan esperado.");
            });
        }

        [TestMethod]
        public void EventPropertyAdapterTimeStampType()
        {
            string eql = "from SpaceObservable1 select @event.adapter.Timestamp as ts into SourceXYZ";

            DefaultSchedulerFactory dsf = new DefaultSchedulerFactory();

            ITestableObservable<EventObject> input = dsf.TestScheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            this.Process(eql, dsf, input).Subscribe(x => {
                Assert.IsInstanceOfType(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("ts").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)), typeof(DateTime), "El plan obtenido difiere del plan esperado.");
            });
        }
    }
}
