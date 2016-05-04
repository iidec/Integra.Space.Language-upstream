using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using System.Reactive.Linq;
using System.Collections.Generic;
using Microsoft.Reactive.Testing;
using System.Reactive;

namespace Integra.Space.LanguageUnitTests.Events
{
    [TestClass]
    public class EventValuesNodeTests
    {
        List<EventObject> eventList = new List<EventObject>();

        [TestMethod]
        public void EventPropertyAgentNameValue()
        {
            eventList.Add(TestObjects.EventObjectTest1);

            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select @event.agent.Name as nombre");
            List<PlanNode> plan = parser.Evaluate();
            
            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<string> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("nombre").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
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
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select @event.Adapter.Name as nombre");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<string> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("nombre").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
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
            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select @event.Agent.MachineName as machineName");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<string> results = scheduler.Start(
                () => result(input.AsQbservable()).Select(x => ((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("machineName").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString()),
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
            eventList.Add(TestObjects.EventObjectTest1);

            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select @event.Agent.Timestamp as ts");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            result(eventList.ToObservable().AsQbservable()).Subscribe(x => {
                Assert.IsInstanceOfType(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("ts").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)), typeof(DateTime), "El plan obtenido difiere del plan esperado.");
            });
        }

        [TestMethod]
        public void EventPropertyAdapterTimeStampType()
        {
            eventList.Add(TestObjects.EventObjectTest1);

            EQLPublicParser parser = new EQLPublicParser("from SpaceObservable1 select @event.adapter.Timestamp as ts");
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() {  PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IObservable<object>>(plan.First());

            result(eventList.ToObservable().AsQbservable()).Subscribe(x => {
                Assert.IsInstanceOfType(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("ts").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)), typeof(DateTime), "El plan obtenido difiere del plan esperado.");
            });
        }
    }
}
