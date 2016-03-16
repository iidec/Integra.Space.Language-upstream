using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Integra.Messaging;
using System.Linq;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class JoinTests
    {
        internal PlanNode GetKeyComparer(int sourcePosition)
        {
            PlanNode idPropertyLeftKey = new PlanNode();
            idPropertyLeftKey.NodeType = PlanNodeTypeEnum.Identifier;
            idPropertyLeftKey.Properties.Add("Value", "_1");
            idPropertyLeftKey.Properties.Add("DataType", typeof(object).ToString());

            PlanNode idParamLeftKey = new PlanNode();
            idParamLeftKey.NodeType = PlanNodeTypeEnum.Identifier;
            idParamLeftKey.Properties.Add("Value", sourcePosition);
            idParamLeftKey.Properties.Add("DataType", typeof(object).ToString());

            PlanNode leftEvent = new PlanNode();
            leftEvent.NodeType = PlanNodeTypeEnum.Event;
            leftEvent.Children = new List<PlanNode>();
            leftEvent.Children.Add(idParamLeftKey);

            PlanNode message = new PlanNode();
            message.NodeType = PlanNodeTypeEnum.ObjectMessage;
            message.Children = new List<PlanNode>();
            message.Children.Add(leftEvent);

            PlanNode idPartOrFieldObject = new PlanNode();
            idPartOrFieldObject.NodeType = PlanNodeTypeEnum.Identifier;
            idPartOrFieldObject.Properties.Add("Value", "0");
            idPartOrFieldObject.Properties.Add("DataType", typeof(string).ToString());

            PlanNode auxPart = new PlanNode();
            auxPart.Properties.Add(SR.DataTypeProperty, typeof(MessagePart));
            auxPart.NodeType = PlanNodeTypeEnum.ObjectPart;
            auxPart.Children = new List<PlanNode>();
            auxPart.Children.Add(message);
            auxPart.Children.Add(idPartOrFieldObject);

            PlanNode idFieldObject = new PlanNode();
            idFieldObject.NodeType = PlanNodeTypeEnum.Identifier;
            idFieldObject.Properties.Add("Value", "0");
            idFieldObject.Properties.Add("DataType", typeof(string).ToString());

            PlanNode auxField = new PlanNode();
            auxField.Properties.Add(SR.DataTypeProperty, typeof(MessageField));
            auxField.NodeType = PlanNodeTypeEnum.ObjectField;
            auxField.Children = new List<PlanNode>();
            auxField.Children.Add(auxPart);
            auxField.Children.Add(idFieldObject);

            PlanNode leftObjectValue = new PlanNode();
            leftObjectValue.Properties.Add(SR.DataTypeProperty, typeof(object).ToString());
            leftObjectValue.NodeType = PlanNodeTypeEnum.ObjectValue;
            leftObjectValue.Children = new List<PlanNode>();
            leftObjectValue.Children.Add(auxField);

            PlanNode leftTupleProjection = new PlanNode();
            leftTupleProjection.NodeType = PlanNodeTypeEnum.TupleProjection;
            leftTupleProjection.Children = new List<PlanNode>();
            leftTupleProjection.Children.Add(idPropertyLeftKey);
            leftTupleProjection.Children.Add(leftObjectValue);

            return leftTupleProjection;
        }

        [TestMethod]
        public void JoinEQLTest()
        {
            string eql = "cross JOIN SpaceObservable1 as t1 WHERE t1.@event.Message.#1.#2 == \"9999941616073663_1\" " +
                                "WITH SpaceObservable1 as t2 WHERE t2.@event.Message.#1.#2 == \"9999941616073663_2\" " +
                                //"ON t1.@event.Adapter.Name == t2.@event.Adapter.Name " + // and (decimal)t1.@event.Message.#1.#4 == (decimal)t2.@event.Message.#1.#4 and right((string)t1.@event.Message.#1.#43, 5) == right((string)t2.@event.Message.#1.#43, 5)
                                "ON t1.@event.Message.#0.#0 == t2.@event.Message.#1.#143 " +
                                "TIMEOUT '00:00:01' " +
                                "EVENTLIFETIME '00:00:10' " +
                                //"WHERE  t1.@event.Message.#0.#0 == \"0100\" " +
                                "SELECT t1.@event.Message.#1.#2 as c1, t2.@event.Message.#1.#2 as c2 ";

            EQLPublicParser parser = new EQLPublicParser(eql);
            List<PlanNode> plan = parser.Evaluate();

            ObservableConstructor te = new ObservableConstructor(new CompileContext() { PrintLog = true, QueryName = string.Empty, Scheduler = new DefaultSchedulerFactory() });
            Func<IQbservable<EventObject>, IQbservable<EventObject>, IObservable<object>> result = te.Compile<IQbservable<EventObject>, IQbservable<EventObject>, IObservable<object>>(plan.First());

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input1 = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObservable<EventObject> input2 = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest2)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObserver<object> results = scheduler.Start(
                () => result(input1.AsQbservable(), input2.AsQbservable())
                .Select(x =>
                    (object)(new
                    {
                        result = decimal.Parse(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0).GetType().GetProperty("suma").GetValue(((Array)x.GetType().GetProperty("Result").GetValue(x)).GetValue(0)).ToString())
                    })
                ),
                created: 10,
                subscribed: 50,
                disposed: 40000);
            
            scheduler.AdvanceTo(TimeSpan.FromSeconds(30).Ticks);
            System.Threading.Tasks.Task.Delay(4000);

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<object>>[] {
                    new Recorded<Notification<object>>(200, Notification.CreateOnNext((object)(new { suma1 = (decimal)3, monto1 = (decimal)1, campo1 = "campoXX", suma2 = (decimal)3, monto2 = (decimal)1, campo2 = "campoXX" }))),
                    new Recorded<Notification<object>>(200, Notification.CreateOnCompleted<object>())                    
                }, results.Messages);

            Console.WriteLine("");
        }
    }
}
