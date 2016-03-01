using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Integra.Space.Language.Runtime;
using System.Dynamic;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using Microsoft.Reactive.Testing;
using System.Reactive.Concurrency;

namespace Integra.Space.LanguageUnitTests.TypeBuilder
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            QueryResultWriter qrw1 = new QueryResultWriter();
            List<FieldNode> l = new List<FieldNode>();
            l.Add(new FieldNode("x", typeof(string)));
            l.Add(new FieldNode("y", typeof(int)));
            l.Add(new FieldNode("z", typeof(bool)));

            var eventResultType = LanguageTypeBuilder.CompileResultType(l, typeof(EventResult));
            var objectResultType1 = Activator.CreateInstance(eventResultType);
            var objectResultType2 = Activator.CreateInstance(eventResultType);
            var objectResultType3 = Activator.CreateInstance(eventResultType);

            objectResultType1.GetType().GetMethod("Serialize").Invoke(objectResultType1, new[] { qrw1 });
            
            QueryResultWriter qrw = new QueryResultWriter();

            Type resultType = ResultTypeBuilder.CreateResultType("UnQueryX", eventResultType);
            Console.WriteLine("");
            Array x = Array.CreateInstance(eventResultType, 3);
            x.SetValue(objectResultType1, 0);
            x.SetValue(objectResultType2, 1);
            x.SetValue(objectResultType3, 2);
            var result = Activator.CreateInstance(resultType, "QueryResultXYZ", DateTime.Now,  x);

            result.GetType().GetMethod("Serialize").Invoke(result, new[] { qrw });

            Console.WriteLine("JSON: {0}", qrw.JsonResult);
        }

        private class FieldTest
        {
            public string FieldName { get; set; }
            public Type FieldType { get; set; }
        }

        [TestMethod]
        public void TestJoin()
        {
            TestScheduler scheduler = new TestScheduler();

            ITestableObservable<EventObject> input = scheduler.CreateHotObservable(
                new Recorded<Notification<EventObject>>(100, Notification.CreateOnNext(TestObjects.EventObjectTest1)),
                new Recorded<Notification<EventObject>>(200, Notification.CreateOnCompleted<EventObject>())
                );

            var testDuration = Observable.Never<Unit>()
                .Timeout(TimeSpan.FromMilliseconds(2000))
                .Catch(Observable.Return<Unit>(Unit.Default));

            ReplaySubject<int> r1 = new ReplaySubject<int>();
            ReplaySubject<string> r2 = new ReplaySubject<string>();

            r1
            .Publish()
            .RefCount()
            .Subscribe<int>(e =>
            {
                Observable.Return(e)
                .Join(
                    r2,
                    _ => testDuration,
                    _ => testDuration,
                    (l, r) => Tuple.Create(l, r)
                    )
                //.Take(1)
                .Timeout(TimeSpan.FromMilliseconds(3000), Observable.Return(Tuple.Create<int, string>(e, null)), TaskPoolScheduler.Default)
                .Subscribe(result =>
                {
                    Console.WriteLine(result);
                },
                () =>
                {
                    // Console.WriteLine("Complete!");
                });
            });
        }
    }
}
