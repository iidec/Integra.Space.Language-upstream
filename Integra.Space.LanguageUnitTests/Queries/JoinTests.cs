using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language;
using Integra.Space.Language.Runtime;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive;
using Microsoft.Reactive.Testing;
using Integra.Messaging;

namespace Integra.Space.LanguageUnitTests.Queries
{
    [TestClass]
    public class JoinTests
    {
        [TestMethod]
        public void JoinTest1()
        {
            string leftSourceName = "left";
            string rightSourceName = "right";

            PlanNode idLeftSource = new PlanNode();
            idLeftSource.NodeType = PlanNodeTypeEnum.Identifier;
            idLeftSource.Properties.Add("Value", leftSourceName);
            idLeftSource.Properties.Add("DataType", typeof(object).ToString());

            PlanNode from1 = new PlanNode();
            from1.NodeType = PlanNodeTypeEnum.ObservableFrom;
            from1.Properties.Add("SourcePosition", 0);
            from1.Children = new List<PlanNode>();
            from1.Children.Add(idLeftSource);

            PlanNode idRightSource = new PlanNode();
            idRightSource.NodeType = PlanNodeTypeEnum.Identifier;
            idRightSource.Properties.Add("Value", rightSourceName);
            idRightSource.Properties.Add("DataType", typeof(object).ToString());

            PlanNode from2 = new PlanNode();
            from2.NodeType = PlanNodeTypeEnum.ObservableFrom;
            from2.Properties.Add("SourcePosition", 0);
            from2.Children = new List<PlanNode>();
            from2.Children.Add(idRightSource);

            PlanNode newScope0 = new PlanNode();
            newScope0.NodeType = PlanNodeTypeEnum.NewScope;
            newScope0.Properties.Add("ScopeParameter", new ScopeParameter(leftSourceName, typeof(IEnumerable<EventObject>)));
            newScope0.Children = new List<PlanNode>();
            newScope0.Children.Add(from1);
            newScope0.Children.Add(from2);

            /* INICIA NODOS NECESARIOS LEFT DURATION */
            /******************************************************************************************************************************************/
            PlanNode never = new PlanNode();
            never.NodeType = PlanNodeTypeEnum.ObservableNever;

            never.Children = new List<PlanNode>(); // esto es solo para pruebas
            never.Children.Add(newScope0); // esto es solo para pruebas

            PlanNode timespanValue = new PlanNode();
            timespanValue.NodeType = PlanNodeTypeEnum.Constant;
            timespanValue.Properties.Add("Value", TimeSpan.FromSeconds(1));
            timespanValue.Properties.Add("DataType", typeof(TimeSpan).ToString());
            timespanValue.Properties.Add("IsConstant", true);

            PlanNode timeout = new PlanNode();
            timeout.NodeType = PlanNodeTypeEnum.ObservableTimeout;
            timeout.Properties.Add("ReturnObservable", true);
            timeout.Children = new System.Collections.Generic.List<PlanNode>();
            timeout.Children.Add(never);
            timeout.Children.Add(timespanValue);
            /******************************************************************************************************************************************/
            PlanNode fromForLambdaLeftWhere = new PlanNode();
            fromForLambdaLeftWhere.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode getMatchedProperty = new PlanNode();
            getMatchedProperty.NodeType = PlanNodeTypeEnum.Property;
            getMatchedProperty.Properties.Add("Property", "Matched");
            getMatchedProperty.Children = new List<PlanNode>();
            getMatchedProperty.Children.Add(fromForLambdaLeftWhere);

            PlanNode negate = new PlanNode();
            negate.NodeType = PlanNodeTypeEnum.Not;
            negate.Children = new List<PlanNode>();
            negate.Children.Add(getMatchedProperty);
            /******************************************************************************************************************************************/
            PlanNode fromForLambdaLeft1 = new PlanNode();
            fromForLambdaLeft1.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;
            fromForLambdaLeft1.Properties.Add("ParameterName", leftSourceName);

            PlanNode newScopeLeftWhere = new PlanNode();
            newScopeLeftWhere.NodeType = PlanNodeTypeEnum.NewScope;
            newScopeLeftWhere.Properties.Add("ScopeParameter", new ScopeParameter(leftSourceName, typeof(EventObject)));
            newScopeLeftWhere.Children = new List<PlanNode>();
            newScopeLeftWhere.Children.Add(fromForLambdaLeft1);

            PlanNode leftWhere = new PlanNode();
            leftWhere.NodeType = PlanNodeTypeEnum.EnumerableWhere;
            leftWhere.Children = new List<PlanNode>();
            leftWhere.Children.Add(newScopeLeftWhere);
            leftWhere.Children.Add(negate);
            /******************************************************************************************************************************************/
            PlanNode newScopenumerableLeftSelect = new PlanNode();
            newScopenumerableLeftSelect.NodeType = PlanNodeTypeEnum.NewScope;
            newScopenumerableLeftSelect.Properties.Add("ScopeParameter", new ScopeParameter(leftSourceName, typeof(EventObject)));
            newScopenumerableLeftSelect.Children = new List<PlanNode>();
            newScopenumerableLeftSelect.Children.Add(leftWhere);

            PlanNode fromForLambdaLeftSelect = new PlanNode();
            fromForLambdaLeftSelect.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;

            PlanNode leftDurationProjection = new PlanNode();
            leftDurationProjection.NodeType = PlanNodeTypeEnum.JoinProjection;
            leftDurationProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.JoinLeftDuration);
            leftDurationProjection.Children = new List<PlanNode>();
            leftDurationProjection.Children.Add(fromForLambdaLeftSelect);
            /******************************************************************************************************************************************/
            PlanNode leftSelect = new PlanNode();
            leftSelect.NodeType = PlanNodeTypeEnum.EnumerableSelectForEnumerable;
            leftSelect.Children = new List<PlanNode>();
            leftSelect.Children.Add(newScopenumerableLeftSelect);
            leftSelect.Children.Add(leftDurationProjection);

            PlanNode toArray = new PlanNode();
            toArray.NodeType = PlanNodeTypeEnum.EnumerableToArray;
            toArray.Children = new List<PlanNode>();
            toArray.Children.Add(leftSelect);
            /******************************************************************************************************************************************/
            PlanNode newScope1 = new PlanNode();
            newScope1.NodeType = PlanNodeTypeEnum.NewScope;
            newScope1.Properties.Add("ScopeParameter", new ScopeParameter("Exception", typeof(TimeoutException)));
            newScope1.Children = new List<PlanNode>();
            newScope1.Children.Add(timeout);

            PlanNode catchNode = new PlanNode();
            catchNode.NodeType = PlanNodeTypeEnum.ObservableCatch;
            catchNode.Children = new System.Collections.Generic.List<PlanNode>();
            catchNode.Children.Add(newScope1);
            catchNode.Children.Add(toArray);
            /******************************************************************************************************************************************/
            /* TERMINA NODOS NECESARIOS LEFT DURATION */

            PlanNode newScope2 = new PlanNode();
            newScope2.NodeType = PlanNodeTypeEnum.NewScope;
            newScope2.Properties.Add("ScopeParameter", new ScopeParameter(leftSourceName, typeof(Tuple<EventObject, EventObject>[])));
            newScope2.Children = new List<PlanNode>();
            newScope2.Children.Add(catchNode);

            PlanNode subscription = new PlanNode();
            subscription.NodeType = PlanNodeTypeEnum.Subscription;
            subscription.Children = new List<PlanNode>();
            subscription.Children.Add(newScope2);

            PlanNode create = new PlanNode();
            create.NodeType = PlanNodeTypeEnum.ObservableCreate;
            create.Children = new System.Collections.Generic.List<PlanNode>();
            create.Children.Add(subscription);

            ObservableConstructor oc = new ObservableConstructor(new CompileContext() { QueryName = string.Empty, Scheduler = DefaultSchedulerFactory.Current, PrintLog = false });
            Func<IObservable<EventObject>> funcResult = oc.Compile<IObservable<EventObject>>(create);

            funcResult()
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                });

            System.Threading.Tasks.Task.Delay(3000);

            Console.WriteLine();

            TestScheduler scheduler = new TestScheduler();

            scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
            ITestableObserver<EventObject> results = scheduler.Start(
                () => funcResult(),
                created: 10,
                subscribed: 20,
                disposed: 4000);

            //scheduler.Sleep(TimeSpan.FromSeconds(3).Ticks);

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<EventObject>>[] {
                    new Recorded<Notification<EventObject>>(1000, Notification.CreateOnNext(new EventObject(null))),
                    new Recorded<Notification<EventObject>>(1000, Notification.CreateOnCompleted<EventObject>())
                }, results.Messages);
        }

        [TestMethod]
        public void Test2()
        {
            string leftSourceName = "left";
            string rightSourceName = "right";

            PlanNode idLeftSource = new PlanNode();
            idLeftSource.NodeType = PlanNodeTypeEnum.Identifier;
            idLeftSource.Properties.Add("Value", leftSourceName);
            idLeftSource.Properties.Add("DataType", typeof(object).ToString());

            PlanNode from1 = new PlanNode();
            from1.NodeType = PlanNodeTypeEnum.ObservableFrom;
            from1.Properties.Add("SourcePosition", 0);
            from1.Children = new List<PlanNode>();
            from1.Children.Add(idLeftSource);

            PlanNode idRightSource = new PlanNode();
            idRightSource.NodeType = PlanNodeTypeEnum.Identifier;
            idRightSource.Properties.Add("Value", rightSourceName);
            idRightSource.Properties.Add("DataType", typeof(object).ToString());

            PlanNode from2 = new PlanNode();
            from2.NodeType = PlanNodeTypeEnum.ObservableFrom;
            from2.Properties.Add("SourcePosition", 0);
            from2.Children = new List<PlanNode>();
            from2.Children.Add(idRightSource);

            PlanNode newScope0 = new PlanNode();
            newScope0.NodeType = PlanNodeTypeEnum.NewScope;
            newScope0.Properties.Add("ScopeParameter", new ScopeParameter(leftSourceName, typeof(IEnumerable<EventObject>)));
            newScope0.Children = new List<PlanNode>();
            newScope0.Children.Add(from1);
            newScope0.Children.Add(from2);

            /* INICIA NODOS NECESARIOS RESULT SELECTOR */
            /******************************************************************************************************************************************/
            PlanNode fromForLambdaLeft2 = new PlanNode();
            fromForLambdaLeft2.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;
            fromForLambdaLeft2.Properties.Add("ParameterName", leftSourceName);
            /******************************************************************************************************************************************/
            PlanNode leftTupleProjection = this.GetKeyComparer(leftSourceName);
            PlanNode rightTupleProjection = this.GetKeyComparer(leftSourceName);

            PlanNode leftProjection = new PlanNode();
            leftProjection.NodeType = PlanNodeTypeEnum.Projection;
            leftProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.KeySelectorProjection);
            leftProjection.Properties.Add("OverrideGetHashCodeMethod", true);
            leftProjection.Children = new List<PlanNode>();
            leftProjection.Children.Add(leftTupleProjection);

            PlanNode rightProjection = new PlanNode();
            rightProjection.NodeType = PlanNodeTypeEnum.Projection;
            rightProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.KeySelectorProjection);
            rightProjection.Properties.Add("OverrideGetHashCodeMethod", true);
            rightProjection.Children = new List<PlanNode>();
            rightProjection.Children.Add(rightTupleProjection);

            PlanNode onSection = new PlanNode();
            onSection.NodeType = PlanNodeTypeEnum.On;
            onSection.Children = new List<PlanNode>();
            onSection.Children.Add(leftProjection);
            onSection.Children.Add(rightProjection);
            /******************************************************************************************************************************************/
            PlanNode enumerableJoinProjection = new PlanNode();
            enumerableJoinProjection.NodeType = PlanNodeTypeEnum.JoinProjection;
            enumerableJoinProjection.Properties.Add("ProjectionType", PlanNodeTypeEnum.JoinResultSelector);
            enumerableJoinProjection.Children = new List<PlanNode>();
            /******************************************************************************************************************************************/
            PlanNode enumerableJoin = new PlanNode();
            enumerableJoin.NodeType = PlanNodeTypeEnum.EnumerableJoin;
            enumerableJoin.Children = new List<PlanNode>();
            enumerableJoin.Children.Add(newScope0);
            enumerableJoin.Children.Add(onSection);
            enumerableJoin.Children.Add(enumerableJoinProjection);
            /******************************************************************************************************************************************/
            /* TERMINA NODOS NECESARIOS RESULT SELECTOR */

            PlanNode newScope2 = new PlanNode();
            newScope2.NodeType = PlanNodeTypeEnum.NewScope;
            newScope2.Properties.Add("ScopeParameter", new ScopeParameter(leftSourceName, typeof(Tuple<EventObject, EventObject>[])));
            newScope2.Children = new List<PlanNode>();
            newScope2.Children.Add(enumerableJoin);

            PlanNode subscription = new PlanNode();
            subscription.NodeType = PlanNodeTypeEnum.Subscription;
            subscription.Children = new List<PlanNode>();
            subscription.Children.Add(newScope2);

            PlanNode create = new PlanNode();
            create.NodeType = PlanNodeTypeEnum.ObservableCreate;
            create.Children = new System.Collections.Generic.List<PlanNode>();
            create.Children.Add(subscription);

            ObservableConstructor oc = new ObservableConstructor(new CompileContext() { QueryName = string.Empty, Scheduler = DefaultSchedulerFactory.Current, PrintLog = false });
            Func<IObservable<EventObject>, IObservable<EventObject>, IObservable<Tuple<EventObject, EventObject>>> funcResult = oc.Compile<IObservable<EventObject>, IObservable<EventObject>, IObservable<Tuple<EventObject, EventObject>>>(create);

            Console.WriteLine();

            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> leftInput = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            ITestableObservable<EventObject> rightInput = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );
            
            ITestableObserver<Tuple<EventObject, EventObject>> results = scheduler.Start(
                () => funcResult(leftInput, rightInput),
                created: 10,
                subscribed: 20,
                disposed: 4000);

            ReactiveAssert.AreElementsEqual(new Recorded<Notification<Tuple<EventObject, EventObject>>>[] {
                    new Recorded<Notification<Tuple<EventObject, EventObject>>>(100, Notification.CreateOnNext(Tuple.Create<EventObject, EventObject>(TestObjects.EventObjectTest1, TestObjects.EventObjectTest1))),
                    new Recorded<Notification<Tuple<EventObject, EventObject>>>(200, Notification.CreateOnCompleted<Tuple<EventObject, EventObject>>())
                }, results.Messages);
        }

        internal PlanNode GetKeyComparer(string sourceName)
        {
            PlanNode idPropertyLeftKey = new PlanNode();
            idPropertyLeftKey.NodeType = PlanNodeTypeEnum.Identifier;
            idPropertyLeftKey.Properties.Add("Value", "_1");
            idPropertyLeftKey.Properties.Add("DataType", typeof(object).ToString());

            PlanNode idParamLeftKey = new PlanNode();
            idParamLeftKey.NodeType = PlanNodeTypeEnum.Identifier;
            idParamLeftKey.Properties.Add("Value", sourceName);
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
    }
}
